#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.CodeAnalysis;

    using Mono.Cecil;

    public static class GetMethodDefinitionExt {
        public static MethodDefinition GetMethodDefinition(this AssemblyDefinition assembly, IMethodSymbol methodSymbol) {
            var nameParts = methodSymbol.ContainingSymbol.GetFullNameParts();
            var @namespace = nameParts.PopNamespace();
            var typeName = nameParts.PopTypeName();
            var type = assembly.MainModule.GetType(@namespace, typeName)
                ?? throw new InvalidOperationException($"Type {@namespace}.{typeName} could not be found in assembly {assembly.FullName}.");
            type = nameParts.PopNestedTypes(type);

            var candidates = type.Methods
                .Where(m
                    => methodSymbol.MetadataName      == m.Name
                    && methodSymbol.Parameters.Length == m.Parameters.Count)
                .ToArray();

            return candidates.Length == 1
                ? candidates[0]
                : candidates
                    .SingleOrDefault(m => methodSymbol.AllParametersMatch(m))
                    ?? throw new InvalidOperationException($"Method {methodSymbol} could not be found in type {type.FullName}.");
        }

        private static TypeDefinition PopNestedTypes(this Stack<ISymbol> nameParts, TypeDefinition type) {
            while (nameParts.Count > 0) {
                var nestedTypeName = nameParts.PopTypeName();
                type = type.NestedTypes.SingleOrDefault(t => t.Name == nestedTypeName)
                    ?? throw new InvalidOperationException($"Type {type.FullName} does not have a nested type named {nestedTypeName}.");
            }

            return type;
        }
    }
}
