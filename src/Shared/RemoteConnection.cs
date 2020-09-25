#nullable enable

namespace Microscope.Shared {
    using System;

    public static class PipeName {
        // Pipe needs to be scoped by PID so multiple VS instances don't compete for connecting CodeLenses.
        public static string Get(int pid) => string.Format(@"microscope\{0}", pid);
    }

    public interface IRemoteCodeLens {
        void Refresh();
    }

    public interface IRemoteVisualStudio {
        void RegisterCodeLensDataPoint(Guid id);
    }
}
