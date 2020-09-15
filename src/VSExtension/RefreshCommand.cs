#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.ComponentModel.Design;

    using Microscope.Shared;

    using Microsoft;
    using Microsoft.VisualStudio.Shell;

    using static Microscope.Shared.Logging;

    using Task = System.Threading.Tasks.Task;

    internal sealed class RefreshCommand {
        public static async Task Initialize(AsyncPackage pkg, Func<Guid, Task> refreshCodeLens) {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(pkg.DisposalToken);

            var cmdService = await pkg.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            Assumes.Present(cmdService);

            var cmdId = new CommandID(PackageGuids.CmdSetId, PackageIds.RefreshCommandId);
            var cmd = new OleMenuCommand(
                (_, e) => ThreadHelper.JoinableTaskFactory.RunAsync(async delegate {
                    await Execute(refreshCodeLens, (OleMenuCmdEventArgs)e).Caf();
                }),
                cmdId);
            cmdService.AddCommand(cmd);
        }

        private static async Task Execute(Func<Guid, Task> refreshCodeLens, OleMenuCmdEventArgs e) {
            try {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var arg = e.InValue as string
                    ?? throw new InvalidOperationException("RefreshCommand requires an argument.");

                if (!Guid.TryParse(arg, out var dataPointId))
                    throw new InvalidOperationException("RefreshCommand requires an argument of type Guid.");

                await refreshCodeLens(dataPointId).Caf();
            } catch (Exception ex) {
                LogVS(ex);
                throw;
            }
        }
    }
}
