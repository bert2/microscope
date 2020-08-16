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

        static InstructionsProvider() {
            Log();
        }

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

        public async Task<List<Instruction>> GetInstructions(Guid projectGuid, string member, CancellationToken ct) {
            try {
                Log($"IL requested for {member} in project {projectGuid}");

                var projectId = workspace.GetProjectId(projectGuid)
                    ?? throw new InvalidOperationException($"Project with GUID {projectGuid} not found in VisualStudioWorkspace.");
                var solution = workspace.CurrentSolution;
                var project = solution.GetProject(projectId)
                    ?? throw new InvalidOperationException($"Project {projectId.Id} not found in solution {solution.FilePath}.");
                var compilation = await project.GetCompilationAsync(ct).ConfigureAwait(false)
                    ?? throw new InvalidOperationException($"Project {project.FilePath} does not support compilation.");

                using var peStream = new MemoryStream();
                var result = compilation.Emit(peStream);
                if (!result.Success) throw new InvalidOperationException($"Failed to compile project {projectId}.");
                _ = peStream.Seek(0, SeekOrigin.Begin);

                var lastDot = member.LastIndexOf('.');
                var typeName = member.Substring(0, lastDot);
                var memberName = member.Substring(lastDot + 1, member.Length - lastDot - 1);

                var assembly = AssemblyDefinition.ReadAssembly(peStream);
                var type = assembly.MainModule.Types.SingleOrDefault(type => type.FullName == typeName)
                    ?? throw new InvalidOperationException($"Type {typeName} could not be found in project {project.FilePath}.");

                var method = type.Methods.SingleOrDefault(m => m.Name == memberName)
                    ?? throw new InvalidOperationException($"Method {memberName} could not be found in type {typeName}.");

                return method.Body?
                    .Instructions
                    .Select(i => new Instruction(i.Offset, i.OpCode.Name, i.Operand?.ToString()))
                    .ToList()
                    ?? new List<Instruction>();
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }
    }
}
