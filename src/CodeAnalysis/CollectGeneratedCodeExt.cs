#nullable enable

namespace Microscope.CodeAnalysis {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Microscope.CodeAnalysis.Model;

    using Mono.Cecil;

    public static class CollectGeneratedCodeExt {
        public static IReadOnlyList<GeneratedType> CollectGeneratedCode(this MethodDefinition method)
            => method
                .CollectGeneratedCode(visited: new HashSet<TypeDefinition>())
                .Select(t => GeneratedType.From(t, method))
                .ToArray();

        private static IEnumerable<TypeDefinition> CollectGeneratedCode(this MethodDefinition method, ISet<TypeDefinition> visited)
            => method
                .Body?
                .Instructions
                .Select(instr => instr.Operand)
                .Where(IsMemberReference)
                .Select(DeclaringType)
                .Where(t => t != null)
                .Cast<TypeDefinition>()
                .Where(IsCompilerGenerated)
                .Where(t => visited.Add(t))
                .Aggregate(
                    Enumerable.Empty<TypeDefinition>(),
                    (ts, t) => ts.Append(t).Concat(t.CollectGeneratedCode(method, visited)))
                ?? Enumerable.Empty<TypeDefinition>();

        private static IEnumerable<TypeDefinition> CollectGeneratedCode(
            this TypeDefinition referencee,
            MethodDefinition referencer,
            ISet<TypeDefinition> visited)
            => referencee
                .Methods
                .Where(IsRelatedTo(referencer))
                .SelectMany(m => m.CollectGeneratedCode(visited));

        private static bool IsMemberReference(object operand) => operand switch {
            IMemberDefinition _ => true,
            MethodReference _   => true,
            _ => false
        };

        private static TypeDefinition? DeclaringType(object operand) => operand switch {
            IMemberDefinition d => d.DeclaringType,
            MethodReference r   => r.TryResolve()?.DeclaringType,
            _ => throw new ArgumentException($"Cannot get DeclaringType of operand {operand}.")
        };

        private static bool IsCompilerGenerated(TypeDefinition type)
            => type.HasCustomAttributes
            && type.CustomAttributes.Any(attr => attr.AttributeType.FullName == typeof(CompilerGeneratedAttribute).FullName);

        private static IMemberDefinition? TryResolve(this MemberReference reference) {
            try {
                return reference.Resolve();
            } catch (AssemblyResolutionException) {
                return null;
            }
        }

        // All lambdas in a class will be turned into methods on the same generated class `<>c`.
        // This filter ensures we only return lambda implementations related to our method.
        public static Func<MethodDefinition, bool> IsRelatedTo(MethodDefinition sourceMethod) => generatedMethod
            => !generatedMethod.Name.StartsWith("<")
            || generatedMethod.Name.StartsWith("<>")
            || generatedMethod.Name.StartsWith($"<{sourceMethod.Name}>");
    }
}
