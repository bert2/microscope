#nullable enable

namespace Microscope.Shared {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IInstructionsProvider {
        Task<CodeLensData> LoadInstructions(Guid dataPointId, Guid projGuid, string filePath, int textStart, int textLen, CancellationToken ct);

        int GetVisualStudioPid();

        Task<bool> IsMicroscopeEnabled();
    }
}
