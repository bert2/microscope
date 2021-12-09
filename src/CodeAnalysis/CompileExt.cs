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
        public static async Task<AssemblyDefinition?> Compile(
            this Project proj,
            Stream peStream,
            OptimizationLevel? optLvl,
            CancellationToken ct) {
            var compilation = await proj
                .WithOptimizationLevel(optLvl)
                .GetCompilationAsync(ct).Caf()
                ?? throw new InvalidOperationException($"Project {proj.FilePath} does not support compilation.");

            var result = compilation.Emit(peStream);
            if (!result.Success) return null;

            _ = peStream.Seek(0, SeekOrigin.Begin);
            return AssemblyDefinition.ReadAssembly(peStream);
        }

        private static Project WithOptimizationLevel(this Project proj, OptimizationLevel? optLvl) {
            if (!optLvl.HasValue) return proj;

            var buildCfg = proj.CompilationOptions?
                .WithOptimizationLevel(optLvl.Value)
                ?? throw new InvalidOperationException($"Failed to set build config {optLvl.Value} on project {proj.FilePath}.");

            return proj.WithCompilationOptions(buildCfg);
        }
    }
}
