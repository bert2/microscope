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

    public class CodeLensDataPoint : IAsyncCodeLensDataPoint {
        private static readonly CodeLensDetailHeaderDescriptor[] detailHeaders = new[] {
            new CodeLensDetailHeaderDescriptor { UniqueName = "Label",   DisplayName = "Label",   Width = .1 },
            new CodeLensDetailHeaderDescriptor { UniqueName = "OpCode",  DisplayName = "Op Code", Width = .15 },
            new CodeLensDetailHeaderDescriptor { UniqueName = "Operand", DisplayName = "Operand", Width = .75 }
        };
        private readonly ICodeLensCallbackService callbackService;
        private volatile CodeLensData? data;

        public CodeLensDescriptor Descriptor { get; }

        public event AsyncEventHandler? InvalidatedAsync;

        public CodeLensDataPoint(ICodeLensCallbackService callbackService, CodeLensDescriptor descriptor) {
            this.callbackService = callbackService;
            Descriptor = descriptor;
        }

        public async Task<CodeLensDataPointDescriptor> GetDataAsync(
            CodeLensDescriptorContext descriptorContext,
            CancellationToken token) {
            try {
                data = await GetInstructions(descriptorContext, token).ConfigureAwait(false);

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

        public async Task<CodeLensDetailsDescriptor> GetDetailsAsync(
            CodeLensDescriptorContext descriptorContext,
            CancellationToken token) {
            try {
                Log();

                // HACK: when opening the details window, the data point is re-created leaving `data` uninitialized.
                // VS will call `GetDataAsync()` and `GetDetailsAsync()` concurrently. In case `data` is still `null`
                // we'll wait a bit for `GetData()` to finish. When it's still `null` afterwards, we'll load it again.
                if (data is null) await Task.Delay(100, token).ConfigureAwait(false);
                if (data is null) data = await GetInstructions(descriptorContext, token).ConfigureAwait(false);

                if (data.ErrorMessage != null) throw new InvalidOperationException($"Getting CodeLens data for {descriptorContext.Get<string>("FullyQualifiedName")} failed earlier.");

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
                        ctx.ApplicableSpan!.Value.Start,
                        ctx.ApplicableSpan!.Value.Length,
                        ctx.Get<string>("FullyQualifiedName")
                    },
                    ct)
                .ConfigureAwait(false);
    }
}
