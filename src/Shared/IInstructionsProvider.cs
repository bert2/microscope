#nullable enable

namespace Microscope.Shared {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IInstructionsProvider {
        Task<List<Instruction>> GetInstructions(Guid projectGuid, string member, CancellationToken ct);
    }
}
