#nullable enable

namespace Microscope.CodeAnalysis {
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Microscope.CodeAnalysis.Model;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public static class CollectCompilerGeneratedInstructionsExt {
        public static List<GeneratedType> CollectCompilerGeneratedInstructions(this IEnumerable<Instruction> instructions)
            => instructions
                .Select(instr => instr.Operand)
                .OfType<IMemberDefinition>()
                .Select(operand => operand.DeclaringType)
                .Distinct()
                .Where(IsCompilerGenerated)
                .Select(GeneratedType.From)
                .ToList();

        private static bool IsCompilerGenerated(TypeDefinition type)
            => type.HasCustomAttributes
            && type.CustomAttributes.Any(attr => attr.AttributeType.FullName == typeof(CompilerGeneratedAttribute).FullName);
    }
}
