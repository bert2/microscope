#nullable enable

namespace Microscope.VSExtension.Options {
    using System.ComponentModel;

    public enum BuildConfig { Auto, Debug, Release }

    public class GeneralOptions : BaseOptionModel<GeneralOptions> {
        [Category("General")]
        [DisplayName("Enabled")]
        [Description("Specifies whether to activate the CodeLens for IL instructions or not.")]
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        [Category("General")]
        [DisplayName("Refresh on save")]
        [Description("Specifies whether the instructions should be refreshed everytime the current document is saved.")]
        [DefaultValue(true)]
        public bool RefreshOnSave { get; set; } = true;

        [Category("General")]
        [DisplayName("Build configuration")]
        [Description("Specifies whether to show optimized (Release) or unoptimized (Debug) instructions. Auto will use the build configuration set via Visual Studio.")]
        [DefaultValue(BuildConfig.Auto)]
        [TypeConverter(typeof(EnumConverter))]
        public BuildConfig BuildConfig { get; set; } = BuildConfig.Auto;
    }
}
