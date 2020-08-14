#nullable enable

namespace Microscope.Shared {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IInstructionsProvider {
        Task<int> CountInstructions(Guid projectGuid, string member, CancellationToken ct);
    }
}
