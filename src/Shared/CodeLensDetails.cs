#nullable enable

namespace Microscope.Shared {
    using System.Collections.Generic;

    public class CodeLensDetails {
        public List<Instruction> Instructions { get; set; }
        public CodeLensDetails(List<Instruction> instructions) => Instructions = instructions;
    }
}
