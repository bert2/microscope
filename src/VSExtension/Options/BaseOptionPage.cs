// Code taken from https://github.com/microsoft/VSSDK-Extensibility-Samples/blob/master/Options/src/Options/BaseOptionPage.cs

#pragma warning disable IDE0021 // Use expression body for constructors
#pragma warning disable IDE0022 // Use expression body for methods
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable RCS1169 // Make field read-only.
#pragma warning disable VSTHRD102 // Implement internal logic asynchronously

namespace Microscope.VSExtension.Options {
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// A base class for a DialogPage to show in Tools -> Options.
    /// </summary>
    public class BaseOptionPage<T> : DialogPage where T : BaseOptionModel<T>, new() {
        private BaseOptionModel<T> _model;

        public BaseOptionPage() {
#pragma warning disable VSTHRD104 // Offer async methods
            _model = ThreadHelper.JoinableTaskFactory.Run(BaseOptionModel<T>.CreateAsync);
#pragma warning restore VSTHRD104 // Offer async methods
        }

        public override object AutomationObject => _model;

        public override void LoadSettingsFromStorage() {
            _model.Load();
        }

        public override void SaveSettingsToStorage() {
            _model.Save();
        }
    }
}
