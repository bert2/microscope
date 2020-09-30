#nullable enable

namespace Microscope.VSExtension.Options {
    using System.ComponentModel;

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
    }
}
