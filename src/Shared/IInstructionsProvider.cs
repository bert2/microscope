#nullable enable

namespace Microscope.Shared {
    using System;
    using System.Threading.Tasks;

    public interface IInstructionsProvider {
        Task<int> CountInstructions(Guid project, string member);
    }
}
