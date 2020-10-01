#nullable enable

namespace Microscope.CodeAnalysis {
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    public static class ToCodeLensDataExt {
        public static Shared.CodeLensData ToCodeLensData(this Collection<Instruction> instrs) {
            var mappedInstrs = new List<Shared.Instruction>(instrs.Count);
            var boxOpsCount = 0;
            var callvirtOpsCount = 0;

            foreach (var instr in instrs) {
                mappedInstrs.Add(new Shared.Instruction(
                    label:   instr.PrintLabel(),
                    opCode:  instr.OpCode.Name,
                    operand: instr.PrintOperand()));

                if (instr.OpCode.Code == Code.Box)
                    boxOpsCount++;
                else if (instr.OpCode.Code == Code.Callvirt && instr.Previous.OpCode.Code != Code.Constrained)
                    callvirtOpsCount++;
            }

            return new Shared.CodeLensData(mappedInstrs, boxOpsCount, callvirtOpsCount);
        }

        /// <summary>
        /// Code adapted from Mono.Cecil.Cil.Instruction.ToString().
        /// </summary>
        private static string PrintOperand(this Instruction instr) {
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
