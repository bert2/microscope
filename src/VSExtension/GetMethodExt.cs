#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.CodeAnalysis;

    using Mono.Cecil;

    public static class GetMethodExt {
        public static MethodDefinition GetMethod(this AssemblyDefinition assembly, string methodFullName, ISymbol symbol) {
            //var lastDot = methodFullName.LastIndexOf('.');
            //var typeName = methodFullName.Substring(0, lastDot);
            //var memberName = methodFullName.Substring(lastDot + 1, methodFullName.Length - lastDot - 1);

            //var methodXmlId = symbol.GetDocumentationCommentId();
            //var typeXmlId = symbol.ContainingType.GetDocumentationCommentXml();

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
