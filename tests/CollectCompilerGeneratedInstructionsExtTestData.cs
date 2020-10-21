#nullable enable

namespace Microscope.Tests {
    using System.Linq;

    public class CollectCompilerGeneratedInstructionsExtTestData {
        public bool Lambda() => new int[0].Any(x => x > 0);
    }
}
