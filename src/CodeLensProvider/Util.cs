#nullable enable

namespace Microscope.CodeLensProvider {
    using Microsoft.VisualStudio.Language.CodeLens;

    public static class Util {
        public static T Get<T>(this CodeLensDescriptorContext ctx, string key) => (T)ctx.Properties[key];
    }
}
