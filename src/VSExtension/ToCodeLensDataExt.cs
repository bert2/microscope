namespace Microscope.VSExtension {
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil.Cil;

    public static class ToCodeLensDataExt {
        public static Shared.Instruction ToCodeLensData(this Instruction instr)
            => new Shared.Instruction(
                label:   instr.PrintLabel(),
                opCode:  instr.OpCode.Name,
                operand: instr.PrintOperand());

        /// <summary>
        /// Code taken from Mono.Cecil.Cil.Instruction.ToString().
        /// </summary>
        public static string PrintOperand(this Instruction instr) {
            var operand = instr.Operand;

            if (operand == null) {
                return "";
            }

            switch (instr.OpCode.OperandType) {
                case OperandType.InlineBrTarget:
                case OperandType.ShortInlineBrTarget:
                    return operand.Cast<Instruction>().PrintLabel();
                case OperandType.InlineSwitch:
                    return operand.Cast<Instruction[]>().Select(PrintLabel).Join(", ");
                case OperandType.InlineString:
                    return $"\"{operand}\"";
                default:
                    return operand.ToString();
            }
        }

        private static string PrintLabel(this Instruction instr) => $"IL_{instr.Offset:X4}";

        private static string Join(this IEnumerable<string> strs, string sep) => string.Join(sep, strs);

        private static T Cast<T>(this object o) => (T)o;
    }
}
