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

        // Logs go to: C:\Users\<user>\AppData\Local\Temp\microscope.log
        private static readonly string logFile = $"{Path.GetTempPath()}/microscope.log";

        [Conditional("DEBUG")]
        public static void Log(
            object? data = null,
            [CallerFilePath] string? file = null,
            [CallerMemberName] string? method = null) {
            lock (@lock) {
                File.AppendAllText(
                    logFile,
                    $"{DateTime.Now:HH:mm:ss.fff} "
                    + $"{Process.GetCurrentProcess().Id,5} "
                    + $"{Thread.CurrentThread.ManagedThreadId,3} "
                    + $"{Path.GetFileNameWithoutExtension(file)}.{method}()"
                    + $"{(data == null ? "" : $": {data}")}\n");
            }
        }
    }
}
