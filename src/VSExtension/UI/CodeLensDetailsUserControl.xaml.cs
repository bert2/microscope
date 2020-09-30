#nullable enable

namespace Microscope.VSExtension.UI {
    using System.Windows.Controls;
    using Microscope.Shared;

    public partial class CodeLensDetailsUserControl : UserControl {
        public CodeLensDetailsUserControl(CodeLensDetails details) {
            InitializeComponent();
            listView.ItemsSource = details.Instructions;
        }
    }
}
