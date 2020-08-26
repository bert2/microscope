#nullable enable

namespace Microscope.Shared {
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public static class Logging
    {
        private static readonly object @lock = new object();

        private static readonly string logPath = Path.GetTempPath();

        [Conditional("DEBUG")]
        public static void Log(
            object? data = null,
            [CallerFilePath] string? file = null,
            [CallerMemberName] string? method = null) {
            lock (@lock) {
                File.AppendAllText(
                    $"{logPath}/microscope.log",
                    $"{DateTime.Now:HH:mm:ss.fff} "
                    + $"{Process.GetCurrentProcess().Id,5} "
                    + $"{Thread.CurrentThread.ManagedThreadId,3} "
                    + $"{Path.GetFileNameWithoutExtension(file)}.{method}()"
                    + $"{(data == null ? "" : $": {data}")}\n");
            }
        }
    }
}
