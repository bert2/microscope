#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    using static Microscope.Shared.Logging;

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#100", "#101", "ede7ef0f-017b-4b33-b7b3-c7480d3cc712", IconResourceID = 400)]
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class MicroscopePackage : AsyncPackage {
        public const string PackageGuidString = "b798b46c-201a-470c-9e3e-fa0abb23dfa7";

        static MicroscopePackage() {
            Log();
        }

        public MicroscopePackage() {
            Log();
        }
    }
}
