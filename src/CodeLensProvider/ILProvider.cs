#nullable enable

namespace Microscope.CodeLensProvider {
    using System;
    using System.ComponentModel.Composition;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.Language.CodeLens.Remoting;
    using Microsoft.VisualStudio.Utilities;

    using static Shared.Logging;

    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(ProviderId)]
    [LocalizedName(typeof(Resources), "Name")]
    [ContentType("code")]
    [Priority(201)]
    public class ILProvider : IAsyncCodeLensDataPointProvider {
        public const string ProviderId = "ILInstructions";
        private readonly Lazy<ICodeLensCallbackService> callbackService;

        [ImportingConstructor]
        public ILProvider(Lazy<ICodeLensCallbackService> callbackService) {
            try {
                Log();
                this.callbackService = callbackService;
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }

        public async Task<bool> CanCreateDataPointAsync(
            CodeLensDescriptor descriptor,
            CodeLensDescriptorContext descriptorContext,
            CancellationToken token) {
            try {
                //var foo = await callbackService.Value.InvokeAsync<int>(this, nameof(ICodeLensContext.Foo)).ConfigureAwait(false);
                //Log(foo);
                return true;
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
                return Task.FromResult<IAsyncCodeLensDataPoint>(new ILDataPoint());
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }
    }
}
