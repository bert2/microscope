#nullable enable

namespace Microscope.VSExtension.UI {
    using System.Windows.Controls;
    using Microscope.Shared;

    public partial class CodeLensDetailsUserControl : UserControl {
        public CodeLensDetailsUserControl(CodeLensDetails details) {
            InitializeComponent();

            foreach (var instr in details.Instructions)
                instr.Documentation = Documentation.For(instr);

            listView.ItemsSource = details.Instructions;
        }
    }
}
