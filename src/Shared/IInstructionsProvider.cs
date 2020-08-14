#nullable enable

using System;

namespace Microscope.Shared {
    public interface IInstructionsProvider {
        int CountInstructions(Guid project, string member);
    }
}
