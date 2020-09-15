#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Microscope.Shared;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    using static Microscope.Shared.Logging;

    using Task = System.Threading.Tasks.Task;

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [Guid(PackageGuids.PackageIdString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class MicroscopePackage : AsyncPackage {
        protected override async Task InitializeAsync(CancellationToken ct, IProgress<ServiceProgressData> progress) {
            try {
                await base.InitializeAsync(ct, progress);
                await JoinableTaskFactory.SwitchToMainThreadAsync(ct);
                await RefreshCommand.Initialize(this, CodeLensConnectionHandler.RefreshCodeLensDataPoint).Caf();
                _ = CodeLensConnectionHandler.AcceptCodeLensConnections();
            } catch (Exception ex) {
                LogVS(ex);
                throw;
            }
        }
    }
}
