#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;

    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.LanguageServices;

    public static class GetDocumentExt {
        private static readonly FieldInfo projectToGuidMapField = typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl",
                throwOnError: true)
            .GetField("_projectToGuidMap", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly MethodInfo getDocumentIdInCurrentContextMethod = typeof(Workspace).GetMethod(
            "GetDocumentIdInCurrentContext",
            BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            types: new[] { typeof(DocumentId) },
            modifiers: null);

        /// <summary>
        /// Code adapted from Microsoft.VisualStudio.LanguageServices.CodeLens.CodeLensCallbackListener.TryGetDocument()
        /// </summary>
        public static Document GetDocument(this VisualStudioWorkspace workspace, string filePath, Guid projGuid) {
            var projectToGuidMap = (ImmutableDictionary<ProjectId, Guid>)projectToGuidMapField.GetValue(workspace);
            var sln = workspace.CurrentSolution;

            var candidateId = sln
                .GetDocumentIdsWithFilePath(filePath)
                // VS will create multiple `ProjectId`s for projects with multiple target frameworks.
                // We simply take the first one we find.
                .FirstOrDefault(candidateId => projectToGuidMap.GetValueOrDefault(candidateId.ProjectId) == projGuid)
                ?? throw new InvalidOperationException($"File {filePath} (project: {projGuid}) not found in solution {sln.FilePath}.");

            var currentContextId = workspace.GetDocumentIdInCurrentContext(candidateId);
            return sln.GetDocument(currentContextId)
                ?? throw new InvalidOperationException($"Document {currentContextId} not found in solution {sln.FilePath}.");
        }

        public static DocumentId? GetDocumentIdInCurrentContext(this Workspace workspace, DocumentId? documentId)
            => (DocumentId?)getDocumentIdInCurrentContextMethod.Invoke(workspace, new[] { documentId });
    }
}
