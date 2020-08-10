#nullable enable

namespace Microscope.CodeLensProvider {
    using System.ComponentModel.Composition;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.Language.CodeLens.Remoting;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(ProviderId)]
    [LocalizedName(typeof(Resources), "Name")]
    [ContentType("code")]
    [Priority(200)]
    public class MSILProvider : IAsyncCodeLensDataPointProvider {
        public const string ProviderId = "MSILInstructions";

        public Task<bool> CanCreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext descriptorContext,
            CancellationToken token)
            => Task.FromResult(true);

        public Task<IAsyncCodeLensDataPoint> CreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext descriptorContext,
            CancellationToken token)
            => Task.FromResult<IAsyncCodeLensDataPoint>(new MSIL());
    }
}
