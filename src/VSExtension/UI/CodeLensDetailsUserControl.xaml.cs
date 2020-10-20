#nullable enable

namespace Microscope.VSExtension.UI {
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
                HasCompilerGeneratedTypes = instructions.Count >= 15,
                CompilerGeneratedInstrucions = new[] {
                    new {
                        DeclaringTypeName = "foo",
                        Methods = new[] {
                            new {
                                MethodName = "bar",
                                Instructions = new[] {
                                    new ViewModel("IL_0001", "conv.ovf.u8.un", "some operand", "some docs"),
                                    new ViewModel("IL_0002", "conv.ovf.u2.un", "another operand, but longer than the other one", "some docs"),
                                    new ViewModel("IL_0002", "ret", "another operand, but longer than the other one", "some docs")
                                }
                            }
                        }
                    },
                    new {
                        DeclaringTypeName = "qux",
                        Methods = new[] {
                            new {
                                MethodName = "baz",
                                Instructions = new[] {
                                    new ViewModel("IL_0001", "conv.ovf.u4.un", "some operand", "some docs"),
                                    new ViewModel("IL_0002", "ret", "another operand, but longer than the other one", "some docs")
                                }
                            },
                            new {
                                MethodName = "fonk",
                                Instructions = new[] {
                                    new ViewModel("IL_0001", "nop", "some operand", "some docs"),
                                    new ViewModel("IL_0002", "constrained.", "another operand, but longer than the other one", "some docs"),
                                }
                            }
                        }
                    }
                }.ToList(),
            };

            if (!data.HasCompilerGeneratedTypes) data.CompilerGeneratedInstrucions.Clear();

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
