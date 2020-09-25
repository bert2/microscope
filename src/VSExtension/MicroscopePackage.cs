#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Microscope.Shared;
    using Microscope.VSExtension.Options;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    using static Microscope.Shared.Logging;

    using Task = System.Threading.Tasks.Task;

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [Guid(PackageGuids.PackageIdString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(DialogPageProvider.General), "microscope", "General", 0, 0, true , new string[] { "instructions", "CIL", "MSIL", "intermediate language", "CodeLens" })]
    [ProvideProfile(typeof(DialogPageProvider.General), "microscope", Vsix.Name, 0, 0, true)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class MicroscopePackage : AsyncPackage {
        protected override async Task InitializeAsync(CancellationToken ct, IProgress<ServiceProgressData> progress) {
            try {
                await base.InitializeAsync(ct, progress);
                await JoinableTaskFactory.SwitchToMainThreadAsync(ct);
                _ = CodeLensConnectionHandler.AcceptCodeLensConnections();
                await RefreshCommand.Initialize(this, CodeLensConnectionHandler.RefreshCodeLensDataPoint).Caf();
                await GoToDocumentationCommand.Initialize(this).Caf();
            } catch (Exception ex) {
                LogVS(ex);
                throw;
            }
        }
    }
}
