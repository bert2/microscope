#nullable enable

namespace Microscope.CodeLensProvider {
    using System;
    using System.ComponentModel.Composition;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.Language.CodeLens.Remoting;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Utilities;

    using static Microscope.Shared.Logging;

    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(ProviderId)]
    [LocalizedName(typeof(Resources), "Name")]
    [ContentType("CSharp")]
    [Priority(210)]
    public class CodeLensProvider : IAsyncCodeLensDataPointProvider {
        public const string ProviderId = "ILInstructions";
        private readonly Lazy<ICodeLensCallbackService> callbackService;

        [ImportingConstructor]
        public CodeLensProvider(Lazy<ICodeLensCallbackService> callbackService) {
            this.callbackService = callbackService;
            Log(); // logs the PID of the out-of-process CodeLens engine
        }

        public Task<bool> CanCreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext context,
            CancellationToken ct)
            => Task.FromResult(descriptor.Kind == CodeElementKinds.Method);

        public async Task<IAsyncCodeLensDataPoint> CreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext context,
            CancellationToken ct) {
            try {
                var dp = new CodeLensDataPoint(callbackService.Value, descriptor);
                await dp.ConnectToVisualStudio().Caf();
                return dp;
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }
    }
}
