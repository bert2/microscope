#nullable enable

namespace Microscope.Shared {
    public static class Constants {
        // We need to use a different pipe for the VS experimental instance.
        // Otherwise both VS instances will compete for connecting CodeLenses.
        public const string MicroscopePipe =
#if DEBUG
            @"microscope\exp\vs-to-codelens";
#else
            @"microscope\vs-to-codelens";
#endif
    }
}
