#nullable enable

namespace CodeCleanupOnSave {
    using System;
    using System.ComponentModel.Composition;

    using Microscope.VSExtension;
    using Microscope.VSExtension.Options;

    using Microsoft.VisualStudio.Commanding;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
    using Microsoft.VisualStudio.Utilities;

    using static Microscope.Shared.Logging;

    [Export(typeof(ICommandHandler))]
    [Name(nameof(SaveCommandHandler))]
    [ContentType("CSharp")]
    [ContentType("Basic")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class SaveCommandHandler : ICommandHandler<SaveCommandArgs> {
        public string DisplayName => nameof(SaveCommandHandler);

        public bool ExecuteCommand(SaveCommandArgs args, CommandExecutionContext ctx) {
            try {
                var opts = GeneralOptions.Instance;

                if (opts.Enabled && opts.RefreshOnSave) {
                    _ = ThreadHelper.JoinableTaskFactory.RunAsync(async delegate {
                        // CodeLenses usually only live as long as the document is open so we just refresh all the connected ones.
                        await CodeLensConnectionHandler.RefreshAllCodeLensDataPoints();
                    });
                }

                return true;
            } catch (Exception ex) {
                LogVS(ex);
                throw;
            }
        }

        public CommandState GetCommandState(SaveCommandArgs args) => CommandState.Available;
    }
}
