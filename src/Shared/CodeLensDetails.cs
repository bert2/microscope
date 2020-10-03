#nullable enable

namespace Microscope.Shared {
    using System;

    // Only holds the ID the `CodeLensDataPoint` for which the details have been requested.
    // The actual instructions to display are stored in the VS process and can be accessed
    // using the ID.
    public class CodeLensDetails {
        public Guid DataPointId { get; set; }
        public CodeLensDetails(Guid dataPointId) => DataPointId = dataPointId;
    }
}
