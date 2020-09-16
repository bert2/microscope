#nullable enable

namespace Microscope.Shared {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IInstructionsProvider {
        Task<CodeLensData> GetInstructions(Guid projGuid, string filePath, int textStart, int textLen, string methodLongName, CancellationToken ct);

        int GetVisualStudioPid();
    }
}
