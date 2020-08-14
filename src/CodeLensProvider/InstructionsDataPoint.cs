#nullable enable

namespace Microscope.CodeLensProvider {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.Language.CodeLens.Remoting;
    using Microsoft.VisualStudio.Threading;

    using static Microscope.Shared.Logging;

    public class InstructionsDataPoint : IAsyncCodeLensDataPoint {
        private readonly ICodeLensCallbackService callbackService;

        public CodeLensDescriptor Descriptor { get; }

        public event AsyncEventHandler? InvalidatedAsync;

        public InstructionsDataPoint(ICodeLensCallbackService callbackService, CodeLensDescriptor descriptor) {
            this.callbackService = callbackService;
            Descriptor = descriptor;
        }

        public async Task<CodeLensDataPointDescriptor> GetDataAsync(CodeLensDescriptorContext descriptorContext, CancellationToken token) {
            try {
                Log();

                var foo = await callbackService
                    .InvokeAsync<int>(this, nameof(ICodeLensContext.Foo), cancellationToken: token)
                    .ConfigureAwait(false);

                return new CodeLensDataPointDescriptor {
                    Description = $"{foo} instructions",
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
                        UniqueName = "Address",
                        DisplayName = "Address",
                        Width = .1
                    },
                    new CodeLensDetailHeaderDescriptor {
                        UniqueName = "Instruction",
                        DisplayName = "Instruction",
                        Width = .15
                    },
                    new CodeLensDetailHeaderDescriptor {
                        UniqueName = "Argument",
                        DisplayName = "Argument",
                        Width = .75
                    }
                },
                    Entries = new[] {
                    new CodeLensDetailEntryDescriptor {
                        Fields = new[] {
                            new CodeLensDetailEntryField { Text = "IL_0001" },
                            new CodeLensDetailEntryField { Text = "ldfld" },
                            new CodeLensDetailEntryField { Text = "UserQuery._queryCancelTokenOrigin" }
                        },
                        Tooltip = "ldfld short documentation bla bla"
                    },
                    new CodeLensDetailEntryDescriptor {
                        Fields = new[] {
                            new CodeLensDetailEntryField { Text = "IL_0006" },
                            new CodeLensDetailEntryField { Text = "callvirt" },
                            new CodeLensDetailEntryField { Text = "System.Lazy<System.Threading.CancellationToken>.get_Value" }
                        },
                        Tooltip = "callvirt short documentation bla bla"
                    }
                }
                });
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }
    }
}
