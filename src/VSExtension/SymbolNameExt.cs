namespace Microscope.VSExtension {
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.CodeAnalysis;

    public static class SymbolNameExt {
        public static Stack<ISymbol> GetFullNameParts(this ISymbol symbol) {
            var nameParts = new Stack<ISymbol>(capacity: 10);

            for (var s = symbol; !s.IsGlobalNamespace(); s = s.ContainingSymbol) {
                nameParts.Push(s);
            }

            return nameParts;
        }

        public static string Namespace(this ISymbol symbol) => symbol.Kind == SymbolKind.TypeParameter
            ? ""
            : symbol.GetFullNameParts().PopNamespace();

        public static string PopNamespace(this Stack<ISymbol> nameParts) {
            if (nameParts.Peek().Kind != SymbolKind.Namespace) return "";

            var sb = new StringBuilder(nameParts.Pop().Name, capacity: 100);

            while (nameParts.Peek().Kind == SymbolKind.Namespace) {
                _ = sb.Append('.').Append(nameParts.Pop().Name);
            }

            return sb.ToString();
        }

        public static string PopTypeName(this Stack<ISymbol> nameParts) => nameParts.Pop().MetadataName;

        public static bool IsGlobalNamespace(this ISymbol s) => s is INamespaceSymbol ns && ns.IsGlobalNamespace;
    }
}
