#nullable enable

namespace Microscope.Shared {
    public class CodeLensData {
        public int? InstructionsCount { get; set; }
        public int? BoxOpsCount { get; set; }
        public int? CallvirtOpsCount { get; set; }
        public int? MemberByteSize { get; set; }
        public string? ErrorMessage { get; set; }

        public bool IsFailure => ErrorMessage != null;

        public CodeLensData(int? instructionsCount, int? boxOpsCount, int? callvirtOpsCount, int? memberByteSize, string? errorMessage)
        {
            InstructionsCount = instructionsCount;
            BoxOpsCount = boxOpsCount;
            CallvirtOpsCount = callvirtOpsCount;
            MemberByteSize = memberByteSize;
            ErrorMessage = errorMessage;
        }

        public static CodeLensData Success(int instructionsCount, int boxOpsCount, int callvirtOpsCount, int memberByteSize)
            => new CodeLensData(instructionsCount, boxOpsCount, callvirtOpsCount, memberByteSize, null);

        public static CodeLensData Failure(string message) => new CodeLensData(null, null, null, null, message);

        public static CodeLensData BuildError() => Failure("Cannot retrieve instructions due to build errors.");

        public CodeLensData Merge(CodeLensData other) => (IsFailure, other.IsFailure) switch {
            (true , _    ) => this,
            (false, true ) => other,
            (false, false) => Success(
                InstructionsCount!.Value + other.InstructionsCount!.Value,
                BoxOpsCount!.Value + other.BoxOpsCount!.Value,
                CallvirtOpsCount!.Value + other.CallvirtOpsCount!.Value,
                MemberByteSize!.Value + other.MemberByteSize!.Value)
        };
    }
}
