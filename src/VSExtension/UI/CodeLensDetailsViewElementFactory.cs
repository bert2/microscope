#nullable enable

namespace Microscope.VSExtension.UI {
    using Microscope.Shared;

    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    using System.ComponentModel.Composition;
    using System.Windows;

    [Export(typeof(IViewElementFactory))]
    [Name("CodeLens details view element factory")]
    [TypeConversion(from: typeof(CodeLensDetails), to: typeof(FrameworkElement))]
    [Order]
    public class CodeLensDetailsViewElementFactory : IViewElementFactory {
        public TView? CreateViewElement<TView>(ITextView textView, object model) where TView : class
            => new CodeLensDetailsUserControl((CodeLensDetails)model) as TView;
    }
}
