#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;

    using Microscope.Shared;

    using Microsoft;
    using Microsoft.VisualStudio.Shell;

    using static Microscope.Shared.Logging;

    using Task = System.Threading.Tasks.Task;

    internal sealed class GoToDocumentationCommand {
        public static async Task Initialize(AsyncPackage pkg) {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(pkg.DisposalToken);

            var cmdService = await pkg.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            Assumes.Present(cmdService);

            var cmdId = new CommandID(PackageGuids.CmdSetId, PackageIds.GoToDocumentationCommandId);
            var cmd = new OleMenuCommand(
                (_, e) => ThreadHelper.JoinableTaskFactory.RunAsync(async delegate {
                    await Execute((OleMenuCmdEventArgs)e).Caf();
                }),
                cmdId);
            cmdService.AddCommand(cmd);
        }

        private static async Task Execute(OleMenuCmdEventArgs e) {
            try {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var opCode = e.InValue as string
                    ?? throw new InvalidOperationException($"{nameof(GoToDocumentationCommand)} requires an argument.");

                opCode = opCode.Replace('.', '_');

                _ = Process.Start($"https://docs.microsoft.com/dotnet/api/system.reflection.emit.opcodes.{opCode}");
            } catch (Exception ex) {
                LogVS(ex);
                throw;
            }
        }
    }
}
