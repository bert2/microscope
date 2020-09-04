#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;

    using Microsoft.CodeAnalysis;

    using Mono.Cecil;
    using Mono.Collections.Generic;

    public static class GetMethodExt {
        public static MethodDefinition GetMethod(this AssemblyDefinition assembly, IMethodSymbol methodSymbol) {
            var nameParts = methodSymbol.ContainingSymbol.GetFullNameParts();
            var @namespace = nameParts.PopNamespace();
            var typeName = nameParts.PopTypeName();
            var type = assembly.MainModule.GetType(@namespace, typeName)
                ?? throw new InvalidOperationException($"Type {@namespace}.{typeName} could not be found in assembly {assembly.FullName}.");
            type = nameParts.PopNestedTypes(type);

            var candidates = type.Methods
                .Where(m
                    => m.Name == methodSymbol.MetadataName
                    && m.Parameters.Count == methodSymbol.Parameters.Length)
                .ToArray();

            return candidates.Length == 1
                ? candidates[0]
                : candidates
                    .SingleOrDefault(m => m.Parameters.AllMatch(methodSymbol.Parameters))
                    ?? throw new InvalidOperationException($"Method {methodSymbol} could not be found in type {type.FullName}.");
        }

        private static bool AllMatch(this Collection<ParameterDefinition> candidateParams, ImmutableArray<IParameterSymbol> targetParams)
            => candidateParams
                .Zip(targetParams, (c, t) => (candidate: c, target: t))
                .All(Match);

        private static bool Match((ParameterDefinition candidate, IParameterSymbol target) pair)
            => pair.candidate.Name == pair.target.MetadataName
            && pair.candidate.ParameterType.Name == pair.target.Type.MetadataName
            && pair.candidate.ParameterType.FullName == pair.target.Type.ToFullName(nestedSep: '/');

        private static Stack<ISymbol> GetFullNameParts(this ISymbol symbol) {
            var nameParts = new Stack<ISymbol>(capacity: 10);

            for (var s = symbol; !s.IsGlobalNamespace(); s = s.ContainingSymbol) {
                nameParts.Push(s);
            }

            return nameParts;
        }

        private static bool IsGlobalNamespace(this ISymbol s) => s is INamespaceSymbol ns && ns.IsGlobalNamespace;

        private static string ToFullName(this ITypeSymbol type, char nestedSep = '+') {
            if (type.TypeKind == TypeKind.TypeParameter) return type.MetadataName;

            var nameParts = type.GetFullNameParts();
            var fullName = nameParts.PopNamespace(new StringBuilder(100), appendDot: true);
            _ = fullName.Append(nameParts.PopTypeName());

            while (nameParts.Count > 0) {
                _ = fullName.Append(nestedSep).Append(nameParts.PopTypeName());
            }

            return fullName.ToString();
        }

        private static string PopNamespace(this Stack<ISymbol> nameParts) => nameParts
            .PopNamespace(new StringBuilder(capacity: 100))
            .ToString();

        private static StringBuilder PopNamespace(this Stack<ISymbol> nameParts, StringBuilder sb, bool appendDot = false) {
            if (nameParts.Peek().Kind != SymbolKind.Namespace) return sb;

            _ = sb.Append(nameParts.Pop().Name);

            while (nameParts.Peek().Kind == SymbolKind.Namespace) {
                _ = sb.Append('.').Append(nameParts.Pop().Name);
            }

            return appendDot ? sb.Append('.') : sb;
        }

        private static string PopTypeName(this Stack<ISymbol> nameParts) => nameParts.Pop().MetadataName;

        private static TypeDefinition PopNestedTypes(this Stack<ISymbol> nameParts, TypeDefinition type) {
            while (nameParts.Count > 0) {
                var nestedTypeSymbol = nameParts.Pop();
                type = type.NestedTypes.SingleOrDefault(t => t.Name == nestedTypeSymbol.MetadataName)
                    ?? throw new InvalidOperationException($"Type {type.FullName} does not have a nested type named {nestedTypeSymbol.MetadataName}.");
            }

            return type;
        }
    }
}
