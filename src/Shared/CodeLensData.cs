#nullable enable

namespace Microscope.Shared {
    public class CodeLensData {
        public int? InstructionsCount { get; set; }
        public int? BoxOpsCount { get; set; }
        public int? CallvirtOpsCount { get; set; }
        public string? ErrorMessage { get; set; }

        public bool IsFailure => ErrorMessage != null;

        public CodeLensData(int? instructionsCount, int? boxOpsCount, int? callvirtOpsCount, string? errorMessage) {
            InstructionsCount = instructionsCount;
            BoxOpsCount = boxOpsCount;
            CallvirtOpsCount = callvirtOpsCount;
            ErrorMessage = errorMessage;
        }

        public static CodeLensData Success(int instructionsCount, int boxOpsCount, int callvirtOpsCount)
            => new CodeLensData(instructionsCount, boxOpsCount, callvirtOpsCount, null);

        public static CodeLensData Failure(string message) => new CodeLensData(null, null, null, message);

        public static CodeLensData CompilerError() => Failure("Cannot retrieve instructions due to compiler errors.");
    }
}
