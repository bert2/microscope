#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using Mono.Cecil.Cil;

    public readonly struct InstructionData {
        public string Label { get; }

        public string OpCode { get; }

        public string Operand { get; }

        public string? Documentation { get; }

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
