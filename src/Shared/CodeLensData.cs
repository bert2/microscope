#nullable enable

namespace Microscope.Shared {
    using System.Collections.Generic;

    public class CodeLensData {
        public List<Instruction>? Instructions { get; set; }

        public int BoxOpsCount { get; set; }

        public int CallvirtOpsCount { get; set; }

        public string? ErrorMessage { get; set; }

        public CodeLensData(List<Instruction> instructions, int boxOpsCount, int callvirtOpsCount) {
            Instructions = instructions;
            BoxOpsCount = boxOpsCount;
            CallvirtOpsCount = callvirtOpsCount;
        }

        public static CodeLensData Empty()
            => new CodeLensData(new List<Instruction>(), 0, 0);

        public static CodeLensData CompilerError() => Failure("Cannot retrieve instructions due to compiler errors.");

        public static CodeLensData Failure(string message)
            => new CodeLensData(default!, default, default) { ErrorMessage = message };
    }
}
