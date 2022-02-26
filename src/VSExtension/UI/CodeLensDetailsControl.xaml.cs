#nullable enable

namespace Microscope.VSExtension.UI {
    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Microscope.CodeAnalysis.Model;
    using Microscope.Shared;

    public partial class CodeLensDetailsControl : UserControl {
        public CodeLensDetailsControl(CodeLensDetails details) {
            InitializeComponent();
            DataContext = CodeLensConnectionHandler.GetDetailsData(details.DataPointId);
        }

        private void OnInstructionDoubleClick(object sender, MouseButtonEventArgs args) {
            if (sender is Control c && c.DataContext is InstructionData instr) {
                GoToDocumentation(instr);
                args.Handled = true;
            }
        }

        private void GoToDocumentation(InstructionData instr) {
            var opCode = instr.OpCode.TrimEnd('.').Replace('.', '_');
            var url = $"https://docs.microsoft.com/dotnet/api/system.reflection.emit.opcodes.{opCode}";
            _ = Process.Start(url);
        }
    }
}
