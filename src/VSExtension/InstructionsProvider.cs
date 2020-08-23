#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;
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

        public async Task<CodeLensData> GetInstructions(Guid projGuid, string filePath, int textStart, int textLen, string methodLongName, CancellationToken ct) {
            try {
                Log($"IL requested for {methodLongName} in project {projGuid}");

                var sln = workspace.CurrentSolution;
                var projId = workspace.GetProjectId(projGuid)
                    ?? throw new InvalidOperationException($"Project GUID {projGuid} not found in solution {sln.FilePath}.");
                var proj = sln.GetProject(projId)
                    ?? throw new InvalidOperationException($"Project {projId.Id} not found in solution {sln.FilePath}.");
                using var peStream = new MemoryStream();
                var assembly = await proj.Compile(peStream, ct).ConfigureAwait(false);

                var method = assembly.TryGetMethod(methodLongName);

                if (method is null) {
                    Log($"Loading IL of {methodLongName} via its symbol.");
                    var (docId, syntaxNode) = await GetDocumentIdAndNodeAsync(
                            sln,
                            projGuid,
                            filePath,
                            new TextSpan(textStart, textLen),
                            ct)
                        .ConfigureAwait(false);

                    if (docId == null) throw new InvalidOperationException("Document not found.");

                    var doc = sln.GetDocument(docId)
                        ?? throw new InvalidOperationException($"Document with id {docId} not found.");

                    var semanticModel = await doc.GetSemanticModelAsync(ct).ConfigureAwait(false)
                        ?? throw new InvalidOperationException("Failed to get SemanticModel.");

                    var symbol = semanticModel.GetDeclaredSymbol(syntaxNode, ct)
                        ?? throw new InvalidOperationException("Failed to get Symbol.");

                    var methodSymbol = symbol as IMethodSymbol
                        ?? throw new InvalidOperationException("Symbol is not a method.");

                    method = assembly.GetMethod(methodSymbol);
                }

                return method
                    .Body?
                    .Instructions
                    .ToCodeLensData()
                    ?? CodeLensData.Empty();
            } catch (Exception ex) {
                Log(ex);
                return CodeLensData.Failure(ex.ToString());
            }
        }

        /// <summary>
        /// Code taken from Microsoft.VisualStudio.LanguageServices.CodeLens.CodeLensCallbackListener.GetDocumentIdAndNodeAsync()
        /// </summary>
        private async Task<(DocumentId, SyntaxNode)> GetDocumentIdAndNodeAsync(
            Solution solution, Guid projectGuid, string filePath, TextSpan span, CancellationToken cancellationToken) {
            if (!TryGetDocument(solution, projectGuid, filePath, out var document)) {
                return default;
            }

            var root = await document!.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            // TODO: This check avoids ArgumentOutOfRangeException but it's not clear if this is the right solution
            // https://github.com/dotnet/roslyn/issues/44639
            if (!root.FullSpan.Contains(span)) {
                return default;
            }

            return (document.Id, root.FindNode(span));
        }

        /// <summary>
        /// Code taken from Microsoft.VisualStudio.LanguageServices.CodeLens.CodeLensCallbackListener.TryGetDocument()
        /// </summary>
        private bool TryGetDocument(Solution solution, Guid projectGuid, string filePath, out Document? document) {
            document = null;

            if (projectGuid == Microsoft.VisualStudio.VSConstants.CLSID.MiscellaneousFilesProject_guid) {
                return false;
            }

            foreach (var candidateId in solution.GetDocumentIdsWithFilePath(filePath)) {
                if (workspace.GetProjectGuid(candidateId.ProjectId) == projectGuid) {
                    var currentContextId = workspace.GetDocumentIdInCurrentContext(candidateId);
                    document = solution.GetDocument(currentContextId);
                    break;
                }
            }

            return document != null;
        }
    }
}
