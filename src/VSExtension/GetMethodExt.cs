#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.CodeAnalysis;

    using Mono.Cecil;

    public static class GetMethodExt {
        public static MethodDefinition? TryGetMethod(this AssemblyDefinition assembly, string methodLongName) {
            var lastDot = methodLongName.LastIndexOf('.');
            if (lastDot == -1) throw new InvalidOperationException($"Method long name {methodLongName} is missing the declaring type.");

            var typeName = methodLongName.Substring(0, lastDot);
            var type = assembly.MainModule.GetType(fullName: typeName);
            if (type is null) return null;

            var methodName = methodLongName.Substring(lastDot + 1);
            if (methodName == type.Name) methodName = ".ctor";

            var method = type.Methods
                .TrySingleOrDefault(m => m.Name == methodName);
            if (method is null) return null;

            var isTypeNameAmbiguous = assembly.MainModule.Types
                .Select(TypeNameWithoutSuffix)
                .TrySingleOrDefault(t => t == typeName)
                is null;

            return isTypeNameAmbiguous ? null : method;
        }

        private static string TypeNameWithoutSuffix(TypeDefinition t) {
            var suffixStart = t.Name.IndexOf('`');
            return suffixStart == -1 ? t.Name : t.Name.Substring(0, suffixStart);
        }

        public static T? TrySingleOrDefault<T>(this IEnumerable<T> xs, Func<T, bool> predicate) where T: class {
            var two = xs.Where(predicate).Take(2).ToArray();
            return two.Length == 1 ? two[0] : null;
        }

        public static MethodDefinition GetMethod(this AssemblyDefinition assembly, ISymbol symbol) {
            //var lastDot = methodFullName.LastIndexOf('.');
            //var typeName = methodFullName.Substring(0, lastDot);
            //var memberName = methodFullName.Substring(lastDot + 1, methodFullName.Length - lastDot - 1);

            //var methodXmlId = symbol.GetDocumentationCommentId();
            //var typeXmlId = symbol.ContainingType.GetDocumentationCommentXml();

            // optimizations:
            // - implement `AssemblyDefinition.TryGetMethod(string methodLongName)`
            // - use `MainModule.GetType(fullName: ...)` so Cecil can use its cache
            // - when looking up methods, try just with name first; check parameters only if there are multiple
            // - don't use `is` checks; use `ISymbol.Kind` instead

            var name = new Stack<ISymbol>(20);

            for (var s = symbol; !(s is INamespaceSymbol ns && ns.IsGlobalNamespace); s = s.ContainingSymbol) {
                name.Push(s);
            }

            var typeNameBuilder = new StringBuilder(200);

            while (name.Peek() is INamespaceSymbol) {
                _ = typeNameBuilder.Append(name.Pop().Name).Append('.');
            }

            var fullTypeName = typeNameBuilder.Append(name.Pop().MetadataName).ToString();

            var type = assembly.MainModule.Types.SingleOrDefault(type => type.FullName == fullTypeName)
                ?? throw new InvalidOperationException($"Type {fullTypeName} could not be found in assembly {assembly.FullName}.");

            while (name.Peek() is INamedTypeSymbol) {
                var nestedTypeName = name.Pop();
                type = type.NestedTypes.SingleOrDefault(t => t.Name == nestedTypeName.MetadataName);
            }

            var methodName = (IMethodSymbol)name.Pop();
            if (name.Count > 0) throw new InvalidOperationException($"Sanity check failed.");

            return type.Methods.SingleOrDefault(m
                => m.Name == methodName.MetadataName
                && m.Parameters.Count == methodName.Parameters.Length
                && m.GenericParameters.Count == methodName.TypeParameters.Length)
                ?? throw new InvalidOperationException($"Method {methodName} could not be found in type {type.FullName}.");

            //symbol.ContainingType.con
            //assembly.MainModule.Types

            //return null!;
            //var type = assembly.MainModule.Types.SingleOrDefault(type => type.FullName == typeName)
            //    ?? throw new InvalidOperationException($"Type {typeName} could not be found in assembly {assembly.FullName}.");
            //return type.Methods.SingleOrDefault(m => m.Name == memberName)
            //    ?? throw new InvalidOperationException($"Method {memberName} could not be found in type {typeName}.");
        }
    }
}
