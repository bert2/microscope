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

        public async Task<CodeLensData> GetInstructions(Guid projGuid, string filePath, int textStart, int textLen, string methodName, CancellationToken ct) {
            try {
                Log($"IL requested for {methodName} in project {projGuid}");

                var sln = workspace.CurrentSolution;

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

//                Log(@$"
//Symbol names for    {methodName}:
//    Name            {symbol.Name}
//    MetadataName    {symbol.MetadataName}
//    ToString()      {symbol}
//    FullQualFmt     {symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}
//    MiniQualFmt     {symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}
//    GetFull         {GetFullMetadataName(symbol)}
//    XmlDocId        {symbol.GetDocumentationCommentId()}
//Symbol names for ContainingSymbol:
//    Name            {symbol.ContainingSymbol.Name}
//    MetadataName    {symbol.ContainingSymbol.MetadataName}
//    ToString()      {symbol.ContainingSymbol}
//    FullQualFmt     {symbol.ContainingSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}
//    MiniQualFmt     {symbol.ContainingSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}
//    GetFull         {GetFullMetadataName(symbol.ContainingSymbol)}
//    XmlDocId        {symbol.ContainingSymbol.GetDocumentationCommentId()}");

                var projId = workspace.GetProjectId(projGuid)
                    ?? throw new InvalidOperationException($"Project with GUID {projGuid} not found in solution {sln.FilePath}.");
                var proj = sln.GetProject(projId)
                    ?? throw new InvalidOperationException($"Project {projId.Id} not found in solution {sln.FilePath}.");
                var compilation = await proj.GetCompilationAsync(ct).ConfigureAwait(false)
                    ?? throw new InvalidOperationException($"Project {proj.FilePath} does not support compilation.");

                using var peStream = new MemoryStream();
                var result = compilation.Emit(peStream);
                if (!result.Success) throw new InvalidOperationException($"Failed to compile project {proj.FilePath}.");
                _ = peStream.Seek(0, SeekOrigin.Begin);

                return AssemblyDefinition
                    .ReadAssembly(peStream)
                    .GetMethod(methodName, symbol)
                    .Body?
                    .Instructions
                    .ToCodeLensData()
                    ?? new CodeLensData(new List<Instruction>(), boxOpsCount: 0, callvirtOpsCount: 0);
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

        /// <summary>
        /// Code taken from https://stackoverflow.com/questions/27105909/get-fully-qualified-metadata-name-in-roslyn
        /// </summary>
        private static string GetFullMetadataName(ISymbol s) {
            if (s == null || IsRootNamespace(s)) {
                return string.Empty;
            }

            var sb = new StringBuilder(s.MetadataName);
            var last = s;

            s = s.ContainingSymbol;

            while (!IsRootNamespace(s)) {
                if (s is ITypeSymbol && last is ITypeSymbol) {
                    _ = sb.Insert(0, '+');
                } else {
                    _ = sb.Insert(0, '.');
                }

                _ = sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                //sb.Insert(0, s.MetadataName);
                s = s.ContainingSymbol;
            }

            return sb.ToString();
        }

        private static bool IsRootNamespace(ISymbol sym) => sym is INamespaceSymbol s && s.IsGlobalNamespace;
    }
}
