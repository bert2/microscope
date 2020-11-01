#nullable enable

namespace Microscope.CodeAnalysis {
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil.Cil;

    public static class PrintInstructionExt {
        public static string PrintLabel(this Instruction instr) => $"IL_{instr.Offset:X4}";

        // Code adapted from Mono.Cecil.Cil.Instruction.ToString().
        public static string PrintOperand(this Instruction instr) {
            var operand = instr.Operand;

            if (operand == null) return "";

            switch (instr.OpCode.OperandType) {
                case OperandType.InlineBrTarget:
                case OperandType.ShortInlineBrTarget:
                    return ((Instruction)operand).PrintLabel();
                case OperandType.InlineSwitch:
                    return ((Instruction[])operand).Select(PrintLabel).Join(", ");
                case OperandType.InlineString:
                    return $"\"{operand}\"";
                default:
                    return operand.ToString();
            }
        }

        private static string Join(this IEnumerable<string> strs, string sep) => string.Join(sep, strs);
    }
}
