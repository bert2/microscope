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

        // Logs go to: C:\Users\<user>\AppData\Local\Temp\microscope.*.log
        // We're using one log file for each process to prevent concurrent file access.
        private static readonly string vsLogFile = $"{Path.GetTempPath()}/microscope.vs.log";
        private static readonly string clLogFile = $"{Path.GetTempPath()}/microscope.codelens.log";

        [Conditional("DEBUG")]
        public static void LogVS(
            object? data = null,
            [CallerFilePath] string? file = null,
            [CallerMemberName] string? method = null)
            => Log(vsLogFile, file!, method!, data);

        [Conditional("DEBUG")]
        public static void LogCL(
            object? data = null,
            [CallerFilePath] string? file = null,
            [CallerMemberName] string? method = null)
            => Log(clLogFile, file!, method!, data);

        public static void Log(
            string logFile,
            string callingFile,
            string callingMethod,
            object? data = null) {
            lock (@lock) {
                File.AppendAllText(
                    logFile,
                    $"{DateTime.Now:HH:mm:ss.fff} "
                    + $"{Process.GetCurrentProcess().Id,5} "
                    + $"{Thread.CurrentThread.ManagedThreadId,3} "
                    + $"{Path.GetFileNameWithoutExtension(callingFile)}.{callingMethod}()"
                    + $"{(data == null ? "" : $": {data}")}\n");
            }
        }
    }
}
