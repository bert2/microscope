#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Utilities;

    using static Microscope.Shared.Logging;

    [Export(typeof(ICodeLensCallbackListener))]
    [ContentType("CSharp")]
    public class InstructionsProvider : ICodeLensCallbackListener, IInstructionsProvider {
        private readonly VisualStudioWorkspace workspace;

        [ImportingConstructor]
        public InstructionsProvider(VisualStudioWorkspace workspace) => this.workspace = workspace;

        public async Task<CodeLensData> GetInstructions(Guid projGuid, string filePath, int textStart, int textLen, string methodLongName, CancellationToken ct) {
            try {
                Log($"IL requested for {methodLongName} in project {projGuid}");

                var doc = workspace.GetDocument(filePath, projGuid);
                var method = await doc.GetMethodSymbolAt(new TextSpan(textStart, textLen), ct).Caf();

                using var peStream = new MemoryStream();
                using var assembly = await doc.Project.Compile(peStream, ct).Caf();

                return assembly
                    .GetMethod(method)
                    .Body?
                    .Instructions
                    .ToCodeLensData()
                    ?? CodeLensData.Empty();
            } catch (Exception ex) {
                Log(ex);
                return CodeLensData.Failure(ex.ToString());
            }
        }
    }
}
