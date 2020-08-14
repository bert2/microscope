#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.ComponentModel.Composition;
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

        public Task<int> CountInstructions(Guid project, string member) {
            try {
                Log($"IL requested for {member} in project {project}");
                //workspace.CurrentSolution.GetProject(ProjectId.)
                return Task.FromResult(42);
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }
    }
}
