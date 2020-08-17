#nullable enable

namespace Microscope.VSExtension {
    using System.Reflection;

    using Microsoft.CodeAnalysis;

    public static class GetDocumentIdInCurrentContextExt {
        private static readonly MethodInfo getDocumentIdInCurrentContext = typeof(Workspace).GetMethod(
            "GetDocumentIdInCurrentContext",
            BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            types: new[] { typeof(DocumentId) },
            modifiers: null);

        public static DocumentId? GetDocumentIdInCurrentContext(this Workspace workspace, DocumentId? documentId)
            => (DocumentId?)getDocumentIdInCurrentContext.Invoke(workspace, new[] { documentId });
    }
}
