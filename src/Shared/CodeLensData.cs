#nullable enable

namespace Microscope.Shared {
    using System.Collections.Generic;

    public class CodeLensData {
        public List<Instruction> Instructions { get; set; }
        public int BoxOpsCount { get; set; }
        public int CallvirtOpsCount { get; set; }
        public CodeLensData(List<Instruction> instructions, int boxOpsCount, int callvirtOpsCount) {
            Instructions = instructions;
            BoxOpsCount = boxOpsCount;
            CallvirtOpsCount = callvirtOpsCount;
        }
    }
}
