#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.ComponentModel.Design;
    using System.Globalization;

    using Microsoft;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    using Task = System.Threading.Tasks.Task;

    internal sealed class RefreshCommand {
        public static async Task InitializeAsync(AsyncPackage pkg) {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(pkg.DisposalToken);

            var cmdService = await pkg.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            Assumes.Present(cmdService);

            var cmdId = new CommandID(PackageGuids.CmdSetId, PackageIds.RefreshCommandId);
            var cmd = new OleMenuCommand((_, e) => Execute(pkg, (OleMenuCmdEventArgs)e), cmdId);
            cmdService.AddCommand(cmd);
        }

        private static void Execute(AsyncPackage pkg, OleMenuCmdEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            
            var dataPointId = e.InValue as Guid? ?? throw new InvalidOperationException("wat.");

            _ = VsShellUtilities.ShowMessageBox(
                serviceProvider: pkg,
                message: string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", typeof(RefreshCommand).FullName),
                title: "Fudz",
                icon: OLEMSGICON.OLEMSGICON_INFO,
                msgButton: OLEMSGBUTTON.OLEMSGBUTTON_OK,
                defaultButton: OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
