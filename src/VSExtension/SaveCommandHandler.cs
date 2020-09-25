namespace CodeCleanupOnSave {
    using System.ComponentModel.Composition;

    using Microscope.VSExtension;
    using Microscope.VSExtension.Options;

    using Microsoft.VisualStudio.Commanding;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ICommandHandler))]
    [Name(nameof(SaveCommandHandler))]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class SaveCommandHandler : ICommandHandler<SaveCommandArgs> {
        public string DisplayName => nameof(SaveCommandHandler);

        public bool ExecuteCommand(SaveCommandArgs args, CommandExecutionContext executionContext) {
            var opts = GeneralOptions.Instance;

            if (opts.Enabled && opts.RefreshOnSave) {
                // CodeLenses usually only live as long as the document is open so we just refresh all the connected ones.
                _ = CodeLensConnectionHandler.RefreshAllCodeLensDataPoints();
            }

            return true;
        }

        public CommandState GetCommandState(SaveCommandArgs args) => CommandState.Available;
    }
}
