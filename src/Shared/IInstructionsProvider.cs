#nullable enable

namespace Microscope.Shared {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IInstructionsProvider {
        Task<CodeLensData> GetInstructions(Guid projGuid, string methodName, CancellationToken ct);
    }
}
