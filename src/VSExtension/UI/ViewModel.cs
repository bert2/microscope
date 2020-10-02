#nullable enable

namespace Microscope.VSExtension.UI {
    using Microscope.CodeAnalysis;

    public class ViewModel {
        public string Label { get; set; }
        public string OpCode { get; set; }
        public string Operand { get; set; }
        public string? Documentation { get; set; }

        public ViewModel(string label, string opCode, string operand, string? documentation) {
            Label = label;
            OpCode = opCode;
            Operand = operand;
            Documentation = documentation;
        }

        public static ViewModel From(Mono.Cecil.Cil.Instruction instr) => new ViewModel(
            label:         instr.PrintLabel(),
            opCode:        instr.OpCode.Name,
            operand:       instr.PrintOperand(),
            documentation: instr.OpCode.GetDocumentation());

        public override string ToString() => OpCode;
    }
}
