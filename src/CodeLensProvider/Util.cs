#nullable enable

namespace Microscope.CodeLensProvider {
    using Microsoft.VisualStudio.Language.CodeLens;

    public static class Util {
        public static T Get<T>(this CodeLensDescriptorContext ctx, string key) => (T)ctx.Properties[key];

        public static string Labeled(this int n, string singular, string? plural = null)
            => (n, plural) switch {
                (1, _)    => $"{n} {singular}",
                (_, null) => $"{n} {singular}s",
                (_, _)    => $"{n} {plural}"
            };
    }
}
