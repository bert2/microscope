#nullable enable

namespace Microscope.Shared {
    using System;

    public interface IRemoteVisualStudio {
        void RegisterCodeLensDataPoint(Guid id);
    }
}
