#nullable enable

namespace Microscope.CodeLensProvider {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.Language.CodeLens.Remoting;
    using Microsoft.VisualStudio.Threading;

    using static Microscope.Shared.Logging;

    public class CodeLensDataPoint : IAsyncCodeLensDataPoint {
        private readonly ICodeLensCallbackService callbackService;
        private List<Instruction>? instructions;

        public CodeLensDescriptor Descriptor { get; }

        public event AsyncEventHandler? InvalidatedAsync;

        public CodeLensDataPoint(ICodeLensCallbackService callbackService, CodeLensDescriptor descriptor) {
            this.callbackService = callbackService;
            Descriptor = descriptor;
        }

        public async Task<CodeLensDataPointDescriptor> GetDataAsync(CodeLensDescriptorContext descriptorContext, CancellationToken token) {
            try {
                instructions = await callbackService.InvokeAsync<List<Instruction>>(
                        this,
                        nameof(IInstructionsProvider.GetInstructions),
                        new object[] {
                            Descriptor.ProjectGuid,
                            descriptorContext.Get<string>("FullyQualifiedName")
                        },
                        token
                    ).ConfigureAwait(false);

                return new CodeLensDataPointDescriptor {
                    Description = $"{instructions.Count} instructions",
                    TooltipText = "{0} boxings, {0} unconstrained virtual calls",
                    ImageId = null,
                    IntValue = null
                };
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }

        public Task<CodeLensDetailsDescriptor> GetDetailsAsync(CodeLensDescriptorContext descriptorContext, CancellationToken token) {
            try {
                Log();

                return Task.FromResult(new CodeLensDetailsDescriptor {
                    Headers = new[] {
                    new CodeLensDetailHeaderDescriptor {
                        UniqueName = "Label",
                        DisplayName = "Label",
                        Width = .1
                    },
                    new CodeLensDetailHeaderDescriptor {
                        UniqueName = "OpCode",
                        DisplayName = "Op Code",
                        Width = .15
                    },
                    new CodeLensDetailHeaderDescriptor {
                        UniqueName = "Operand",
                        DisplayName = "Operand",
                        Width = .75
                    }
                },
                    Entries = instructions
                        .Select(instr => new CodeLensDetailEntryDescriptor {
                            Fields = new[] {
                                new CodeLensDetailEntryField { Text = instr.Label },
                                new CodeLensDetailEntryField { Text = instr.OpCode },
                                new CodeLensDetailEntryField { Text = instr.Operand }
                            },
                            Tooltip = $"{instr.Label}: {instr.OpCode} {instr.Operand}"
                        })
                        .ToList()
                });
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }
    }
}
