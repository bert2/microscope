#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Utilities;

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

        public async Task<int> CountInstructions(Guid projectGuid, string member, CancellationToken ct) {
            try {
                Log($"IL requested for {member} in project {projectGuid}");

                var projectId = workspace.FindProject(projectGuid)
                    ?? throw new InvalidOperationException($"Project with GUID {projectGuid} not found in VisualStudioWorkspace.");
                var solution = workspace.CurrentSolution;
                var project = solution.GetProject(projectId)
                    ?? throw new InvalidOperationException($"Project {projectId.Id} not found in solution {solution.FilePath}.");
                var compilation = await project.GetCompilationAsync(ct).ConfigureAwait(false)
                    ?? throw new InvalidOperationException($"Project {projectId.Id} does not support compilation.");

                using var peStream = new MemoryStream();
                var result = compilation.Emit(peStream);

                Log(result.Success);

                return 42;
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }
    }
}
