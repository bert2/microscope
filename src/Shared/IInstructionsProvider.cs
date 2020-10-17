#nullable enable

namespace Microscope.Shared {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IInstructionsProvider {
        Task<bool> IsMicroscopeEnabled();

        int GetVisualStudioPid();

        Task<CodeLensData> LoadInstructions(Guid dataPointId, Guid projGuid, string filePath, int textStart, int textLen, CancellationToken ct);
    }
}
