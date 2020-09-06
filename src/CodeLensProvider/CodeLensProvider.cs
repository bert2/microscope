#nullable enable

namespace Microscope.CodeLensProvider {
    using System;
    using System.ComponentModel.Composition;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.Language.CodeLens.Remoting;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(ProviderId)]
    [LocalizedName(typeof(Resources), "Name")]
    [ContentType("code")]
    [Priority(201)]
    public class CodeLensProvider : IAsyncCodeLensDataPointProvider {
        public const string ProviderId = "ILInstructions";
        private readonly Lazy<ICodeLensCallbackService> callbackService;

        [ImportingConstructor]
        public CodeLensProvider(Lazy<ICodeLensCallbackService> callbackService) => this.callbackService = callbackService;

        public Task<bool> CanCreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext context,
            CancellationToken ct)
            => Task.FromResult(descriptor.Kind == CodeElementKinds.Method);

        public Task<IAsyncCodeLensDataPoint> CreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext context,
            CancellationToken ct)
            => Task.FromResult<IAsyncCodeLensDataPoint>(new CodeLensDataPoint(callbackService.Value, descriptor));
    }
}
