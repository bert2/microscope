#nullable enable

namespace Microscope.Shared {
    public class Instruction {
        public int Offset { get; set; }
        public string OpCode { get; set; }
        public string? Operand { get; set; }
        public Instruction(int offset, string opCode, string? operand) {
            Offset = offset;
            OpCode = opCode;
            Operand = operand;
        }
    }
}
