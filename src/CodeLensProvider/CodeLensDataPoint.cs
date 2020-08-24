#nullable enable

namespace Microscope.CodeLensProvider {
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.Language.CodeLens.Remoting;
    using Microsoft.VisualStudio.Threading;

    using static Microscope.Shared.Logging;

    public class CodeLensDataPoint : IAsyncCodeLensDataPoint, IDisposable {
        private static readonly CodeLensDetailHeaderDescriptor[] detailHeaders = new[] {
            new CodeLensDetailHeaderDescriptor { UniqueName = "Label",   DisplayName = "Label",   Width = .1 },
            new CodeLensDetailHeaderDescriptor { UniqueName = "OpCode",  DisplayName = "Op Code", Width = .15 },
            new CodeLensDetailHeaderDescriptor { UniqueName = "Operand", DisplayName = "Operand", Width = .75 }
        };

        private readonly ICodeLensCallbackService callbackService;
        private volatile CodeLensData? data;
        private readonly ManualResetEventSlim dataLoaded = new ManualResetEventSlim(false);

        public CodeLensDescriptor Descriptor { get; }

        public event AsyncEventHandler? InvalidatedAsync;

        public CodeLensDataPoint(ICodeLensCallbackService callbackService, CodeLensDescriptor descriptor) {
            this.callbackService = callbackService;
            Descriptor = descriptor;
        }

        public void Dispose() => dataLoaded.Dispose();

        public async Task<CodeLensDataPointDescriptor> GetDataAsync(CodeLensDescriptorContext context, CancellationToken ct) {
            try {
                data = await GetInstructions(context, ct).ConfigureAwait(false);
                dataLoaded.Set();

                var (description, tooltip) = data.ErrorMessage switch
                {
                    null => (
                        data.Instructions!.Count.Labeled("instruction"),
                        $"{data.BoxOpsCount.Labeled("boxing")}, {data.CallvirtOpsCount.Labeled("unconstrained virtual call")}"),
                    var err => ("- instructions", err)
                };

                return new CodeLensDataPointDescriptor {
                    Description = description,
                    TooltipText = tooltip,
                    ImageId = null,
                    IntValue = null
                };
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }

        public async Task<CodeLensDetailsDescriptor> GetDetailsAsync(CodeLensDescriptorContext context, CancellationToken ct) {
            try {
                Log();

                // When opening the details pane, the data point is re-created leaving `data` uninitialized. VS will
                // then call `GetDataAsync()` and `GetDetailsAsync()` concurrently.
                if (!dataLoaded.Wait(timeout: TimeSpan.FromSeconds(.5), ct))
                    data = await GetInstructions(context, ct).ConfigureAwait(false);

                if (data!.ErrorMessage != null)
                    throw new InvalidOperationException($"Getting CodeLens details for {context.FullName()} failed.");

                return new CodeLensDetailsDescriptor {
                    Headers = detailHeaders,
                    Entries = data
                        .Instructions
                        .Select(instr => new CodeLensDetailEntryDescriptor {
                            Fields = new[] {
                                new CodeLensDetailEntryField { Text = instr.Label },
                                new CodeLensDetailEntryField { Text = instr.OpCode },
                                new CodeLensDetailEntryField { Text = instr.Operand }
                            },
                            Tooltip = $"{instr.Label}: {instr.OpCode} {instr.Operand}"
                        })
                        .ToList()
                };
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }

        private async Task<CodeLensData> GetInstructions(CodeLensDescriptorContext ctx, CancellationToken ct)
            => await callbackService
                .InvokeAsync<CodeLensData>(
                    this,
                    nameof(IInstructionsProvider.GetInstructions),
                    new object[] {
                        Descriptor.ProjectGuid,
                        Descriptor.FilePath,
                        ctx.ApplicableSpan != null
                            ? ctx.ApplicableSpan.Value.Start
                            : throw new InvalidOperationException($"No ApplicableSpan given for {ctx.FullName()}."),
                        ctx.ApplicableSpan!.Value.Length,
                        ctx.FullName()
                    },
                    ct)
                .ConfigureAwait(false);
    }
}
