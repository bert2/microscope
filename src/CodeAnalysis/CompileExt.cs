#nullable enable

namespace Microscope.CodeAnalysis {
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.CodeAnalysis;

    using Mono.Cecil;

    public static class CompileExt {
        public static async Task<AssemblyDefinition?> Compile(this Project proj, Stream peStream, CancellationToken ct) {
            var compilation = await proj.GetCompilationAsync(ct).Caf()
                ?? throw new InvalidOperationException($"Project {proj.FilePath} does not support compilation.");

            var result = compilation.Emit(peStream);
            if (!result.Success) return null;

            _ = peStream.Seek(0, SeekOrigin.Begin);
            return AssemblyDefinition.ReadAssembly(peStream);
        }
    }
}
