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
                    new ViewModel("IL_0001", "nop", "some operand", "some docs", "FooType", "BarMethod"),
                    new ViewModel("IL_0002", "ret", "another operand, but longer than the other one", "some docs", "FooType", "BarMethod"),
                    new ViewModel("IL_0001", "nop", "some operand", "some docs", "QuxType", "BazMethod"),
                    new ViewModel("IL_0002", "ret", "This is a really long operand. One which should cause the list view inside the tree view to be wider than the code lense details view. I'm doing this to test if there will be a second horizontal scollbar inside the TreeView.", "some docs", "QuxType", "BazMethod"),
                    new ViewModel("IL_0001", "nop", "some operand", "some docs", "QuxType", "OnkMethod"),
                    new ViewModel("IL_0002", "ret", "another operand, but longer than the other one", "some docs", "QuxType", "OnkMethod")
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
