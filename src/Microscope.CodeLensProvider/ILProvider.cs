#nullable enable

namespace Microscope.CodeLensProvider {
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.Language.CodeLens.Remoting;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(ProviderId)]
    [LocalizedName(typeof(Resources), "Name")]
    [ContentType("code")]
    [Priority(201)]
    public class ILProvider : IAsyncCodeLensDataPointProvider {
        public const string ProviderId = "ILInstructions";

        private static void Log(object? data = null, [CallerMemberName] string? method = null) => System.IO.File.AppendAllText(
            @"C:\Users\bert\Desktop\microscope.log",
            $"{DateTime.Now:HH:mm:ss.fff} {method}{(data == null ? "" : $": {data}")}\n");

        public ILProvider() {
            var p = Process.GetCurrentProcess();
            Log($"host process: {p.ProcessName} (PID {p.Id})");
        }

        public Task<bool> CanCreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext descriptorContext,
            CancellationToken token)
            => Task.FromResult(true);

        public Task<IAsyncCodeLensDataPoint> CreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext descriptorContext,
            CancellationToken token)
            => Task.FromResult<IAsyncCodeLensDataPoint>(new ILDataPoint());
    }
}
