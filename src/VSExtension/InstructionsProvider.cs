#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Utilities;

    using Mono.Cecil;

    using static Microscope.Shared.Logging;

    [Export(typeof(ICodeLensCallbackListener))]
    [ContentType("CSharp")]
    public class InstructionsProvider : ICodeLensCallbackListener, IInstructionsProvider {
        private readonly VisualStudioWorkspace workspace;

        static InstructionsProvider() => Log();

        [ImportingConstructor]
        public InstructionsProvider(VisualStudioWorkspace workspace) {
            try {
                Log();
                this.workspace = workspace;
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }

        public async Task<CodeLensData> GetInstructions(Guid projGuid, string methodName, CancellationToken ct) {
            try {
                Log($"IL requested for {methodName} in project {projGuid}");

                var sln = workspace.CurrentSolution;
                var projId = workspace.GetProjectId(projGuid)
                    ?? throw new InvalidOperationException($"Project with GUID {projGuid} not found in in solution {sln.FilePath}.");
                var proj = sln.GetProject(projId)
                    ?? throw new InvalidOperationException($"Project {projId.Id} not found in solution {sln.FilePath}.");
                var compilation = await proj.GetCompilationAsync(ct).ConfigureAwait(false)
                    ?? throw new InvalidOperationException($"Project {proj.FilePath} does not support compilation.");

                using var peStream = new MemoryStream();
                var result = compilation.Emit(peStream);
                if (!result.Success) throw new InvalidOperationException($"Failed to compile project {proj.FilePath}.");

                _ = peStream.Seek(0, SeekOrigin.Begin);
                var assembly = AssemblyDefinition.ReadAssembly(peStream);

                var lastDot = methodName.LastIndexOf('.');
                var typeName = methodName.Substring(0, lastDot);
                var memberName = methodName.Substring(lastDot + 1, methodName.Length - lastDot - 1);

                var type = assembly.MainModule.Types.SingleOrDefault(type => type.FullName == typeName)
                    ?? throw new InvalidOperationException($"Type {typeName} could not be found in project {proj.FilePath}.");
                var method = type.Methods.SingleOrDefault(m => m.Name == memberName)
                    ?? throw new InvalidOperationException($"Method {memberName} could not be found in type {typeName}.");

                return method.Body?
                    .Instructions
                    .ToCodeLensData()
                    ?? new CodeLensData(new List<Instruction>(), boxOpsCount: 0, callvirtOpsCount: 0);
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }
    }
}
