#nullable enable

namespace Microscope.Shared {
    public class Instruction {
        public string Label { get; set; }
        public string OpCode { get; set; }
        public string Operand { get; set; }
        public Instruction(string label, string opCode, string operand) {
            Label = label;
            OpCode = opCode;
            Operand = operand;
        }
    }
}
