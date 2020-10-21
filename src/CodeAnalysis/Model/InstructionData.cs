#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using Mono.Cecil.Cil;

    public class InstructionData {
        public string Label { get; set; }

        public string OpCode { get; set; }

        public string Operand { get; set; }

        public string? Documentation { get; set; }

        public InstructionData(string label, string opCode, string operand, string? documentation) {
            Label = label;
            OpCode = opCode;
            Operand = operand;
            Documentation = documentation;
        }

        public static InstructionData From(Instruction instr) => new InstructionData(
            label:         instr.PrintLabel(),
            opCode:        instr.OpCode.Name,
            operand:       instr.PrintOperand(),
            documentation: instr.OpCode.GetDocumentation());

        public override string ToString() => OpCode;
    }
}
