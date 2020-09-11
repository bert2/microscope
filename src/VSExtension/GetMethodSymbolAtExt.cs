#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    public static class GetMethodSymbolAtExt {
        public static async Task<IMethodSymbol> GetMethodSymbolAt(this Document doc, TextSpan span, CancellationToken ct) {
            var rootNode = await doc.GetSyntaxRootAsync(ct).Caf()
                ?? throw new InvalidOperationException($"Document {doc.Name} does not have a syntax tree.");
            var syntaxNode = rootNode.FindNode(span);

            var semanticModel = await doc.GetSemanticModelAsync(ct).Caf()
                ?? throw new InvalidOperationException($"Document {doc.Name} does not have a semantic model.");

            var symbol = semanticModel.GetDeclaredSymbol(syntaxNode, ct)
                ?? throw new InvalidOperationException($"Node is not a symbol declaration: {syntaxNode}.");

            return symbol as IMethodSymbol
                ?? throw new InvalidOperationException($"Symbol {symbol} is not a method.");
        }
    }
}
