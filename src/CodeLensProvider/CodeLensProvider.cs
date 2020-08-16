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

    using static Shared.Logging;

    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(ProviderId)]
    [LocalizedName(typeof(Resources), "Name")]
    [ContentType("code")]
    [Priority(201)]
    public class CodeLensProvider : IAsyncCodeLensDataPointProvider {
        public const string ProviderId = "ILInstructions";
        private readonly Lazy<ICodeLensCallbackService> callbackService;

        static CodeLensProvider() {
            Log();
        }

        [ImportingConstructor]
        public CodeLensProvider(Lazy<ICodeLensCallbackService> callbackService) {
            try {
                Log();
                this.callbackService = callbackService;
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }

        public Task<bool> CanCreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext descriptorContext,
            CancellationToken token) {
            try {
                Log();
                return Task.FromResult(descriptor.Kind == CodeElementKinds.Method);
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }

        public Task<IAsyncCodeLensDataPoint> CreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext descriptorContext,
            CancellationToken token) {
            try {
                Log();
                return Task.FromResult<IAsyncCodeLensDataPoint>(new CodeLensDataPoint(callbackService.Value, descriptor));
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }
    }
}
