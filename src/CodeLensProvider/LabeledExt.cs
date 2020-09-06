#nullable enable

namespace Microscope.CodeLensProvider {
    public static class LabeledExt {
        public static string Labeled(this int n, string singular, string? plural = null) =>
            (n, plural) switch {
                (1, _)    => $"{n} {singular}",
                (_, null) => $"{n} {singular}s",
                (_, _)    => $"{n} {plural}"
            };
    }
}
