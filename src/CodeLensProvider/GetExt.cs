#nullable enable

namespace Microscope.CodeLensProvider {
    using Microsoft.VisualStudio.Language.CodeLens;

    public static class GetExt {
        public static T Get<T>(this CodeLensDescriptorContext ctx, string key) => (T)ctx.Properties[key];

        public static string FullName(this CodeLensDescriptorContext ctx) => ctx.Get<string>("FullyQualifiedName");
    }
}
