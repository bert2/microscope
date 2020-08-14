﻿#nullable enable

namespace Microscope.Shared {
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class Logging
    {
        public static void Log(
            object? data = null,
            [CallerFilePath] string? file = null,
            [CallerMemberName] string? method = null)
            => File.AppendAllText(
                @"C:\Users\bert\Desktop\microscope.log",
                $"{DateTime.Now:HH:mm:ss.fff} "
                + $"{Process.GetCurrentProcess().Id} "
                + $"{Path.GetFileNameWithoutExtension(file)}.{method}()"
                + $"{(data == null ? "" : $": {data}")}\n");
    }
}