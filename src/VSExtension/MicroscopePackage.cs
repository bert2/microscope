#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    using static Microscope.Shared.Logging;

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [Guid("b798b46c-201a-470c-9e3e-fa0abb23dfa7")]
    //[ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class MicroscopePackage : AsyncPackage {
        static MicroscopePackage() => Log();

        public MicroscopePackage() => Log();
    }
}
