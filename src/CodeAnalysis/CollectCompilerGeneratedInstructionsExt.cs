#nullable enable

namespace Microscope.CodeAnalysis {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Microscope.CodeAnalysis.Model;

    using Mono.Cecil;

    public static class CollectCompilerGeneratedInstructionsExt {
        public static List<GeneratedType> CollectCompilerGeneratedInstructions(this MethodDefinition method)
            => method
                .Collect(new HashSet<TypeDefinition>())
                .Select(GeneratedType.From)
                .ToList();

        private static IEnumerable<TypeDefinition> Collect(this MethodDefinition method, ISet<TypeDefinition> visited) {
            if (!method.HasBody) return Enumerable.Empty<TypeDefinition>();

            var types = method
                .Body
                .Instructions
                .Select(instr => instr.Operand)
                .Where(IsReferencingMember)
                .Select(DeclaringType)
                .Where(IsCompilerGenerated)
                .Distinct()
                .Where(t => visited.Add(t));

            var result = new List<TypeDefinition>();

            foreach (var type in types) {
                result.Add(type);

                foreach (var m in type.Methods) {
                    if (!m.Name.StartsWith("<")
                        || m.Name.StartsWith("<>")
                        || m.Name.StartsWith($"<{method.Name}>")) {
                        result.AddRange(m.Collect(visited));
                    }
                }
            }

            return result;
        }

        private static bool IsReferencingMember(object operand) => operand switch {
            IMemberDefinition _ => true,
            MethodReference _   => true,
            _ => false
        };

        private static TypeDefinition DeclaringType(object operand) => operand switch {
            IMemberDefinition d => d.DeclaringType,
            MethodReference r   => r.Resolve().DeclaringType,
            _ => throw new ArgumentException($"Cannot get DeclaringType of operand {operand}.")
        };

        private static bool IsCompilerGenerated(TypeDefinition type)
            => type.HasCustomAttributes
            && type.CustomAttributes.Any(attr => attr.AttributeType.FullName == typeof(CompilerGeneratedAttribute).FullName);
    }
}
