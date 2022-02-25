#nullable enable

namespace Microscope.CodeAnalysis {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    public static class GetSymbolAtExt {
        public static async Task<ISymbol> GetSymbolAt(this Document doc, TextSpan span, CancellationToken ct) {
            var rootNode = await doc.GetSyntaxRootAsync(ct).Caf()
                ?? throw new InvalidOperationException($"Document {doc.Name} does not have a syntax tree.");
            var syntaxNode = rootNode.FindNode(span);

            var semanticModel = await doc.GetSemanticModelAsync(ct).Caf()
                ?? throw new InvalidOperationException($"Document {doc.Name} does not have a semantic model.");

            return semanticModel.GetDeclaredSymbol(syntaxNode, ct)
                ?? throw new InvalidOperationException($"Node is not a symbol declaration: {syntaxNode}.");
        }
    }
}
