#nullable enable

namespace Microscope.VSExtension.UI {
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Microscope.Shared;
    using Microsoft.VisualStudio.Shell;

    public partial class CodeLensDetailsUserControl : UserControl {
        public CodeLensDetailsUserControl(CodeLensDetails details) {
            InitializeComponent();

            var instructions = CodeLensConnectionHandler
                .GetInstructions(details.DataPointId)
                .Select(ViewModel.From)
                .ToList();

            var data = new {
                Instructions = instructions,
                HasCompilerGeneratedTypes = instructions.Count <= 15,
                CompilerGeneratedTypes = new[] {
                    new {
                        Name = "FooType",
                        Methods = new[] {
                            new {
                                Name = "BarMethod",
                                Instructions = new List<ViewModel> {
                                    new ViewModel("IL_0001", "nop", "", null),
                                    new ViewModel("IL_0002", "ret", "", null)
                                }
                            }
                        }.ToList()
                    },
                    new {
                        Name = "QuxType",
                        Methods = new[] {
                            new {
                                Name = "BazMethod",
                                Instructions = new List<ViewModel> {
                                    new ViewModel("IL_0001", "nop", "", null),
                                    new ViewModel("IL_0002", "ret", "", null)
                                }
                            },
                            new {
                                Name = "OnkMethod",
                                Instructions = new List<ViewModel> {
                                    new ViewModel("IL_0001", "nop", "", null),
                                    new ViewModel("IL_0002", "ret", "", null)
                                }
                            }
                        }.ToList()
                    }
                }.ToList(),
            };

            if (data.Instructions.Count > 15) data.CompilerGeneratedTypes.Clear();

            DataContext = data;
        }

        private void GoToDocumentation(object sender, MouseButtonEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (sender is TextBlock tb && tb.DataContext is ViewModel instr) {
                var opCode = instr.OpCode.TrimEnd('.').Replace('.', '_');
                var url = $"https://docs.microsoft.com/dotnet/api/system.reflection.emit.opcodes.{opCode}";
                _ = Process.Start(url);
                e.Handled = true;
            }
        }
    }
}
