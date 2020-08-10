#nullable enable

namespace Microscope.CodeLensProvider {
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.Language.CodeLens.Remoting;
    using Microsoft.VisualStudio.Threading;

    public class MSIL : IAsyncCodeLensDataPoint {
        public CodeLensDescriptor Descriptor { get; }

        public event AsyncEventHandler InvalidatedAsync;

        public Task<CodeLensDataPointDescriptor> GetDataAsync(CodeLensDescriptorContext descriptorContext, CancellationToken token)
            => Task.FromResult(new CodeLensDataPointDescriptor {
                Description = "desc",
                TooltipText = "tooltip",
                ImageId = null,
                IntValue = null
            });

        public Task<CodeLensDetailsDescriptor> GetDetailsAsync(CodeLensDescriptorContext descriptorContext, CancellationToken token)
            => Task.FromResult(new CodeLensDetailsDescriptor {
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
    }
}
