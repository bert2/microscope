#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.CodeAnalysis;
    using Microscope.Shared;
    using Microscope.VSExtension.Options;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Utilities;

    using static Microscope.Shared.Logging;

    [Export(typeof(ICodeLensCallbackListener))]
    [ContentType("CSharp")]
    [ContentType("Basic")]
    public class InstructionsProvider : ICodeLensCallbackListener, IInstructionsProvider {
        private readonly VisualStudioWorkspace workspace;

        [ImportingConstructor]
        public InstructionsProvider(VisualStudioWorkspace workspace) => this.workspace = workspace;

        public async Task<bool> IsMicroscopeEnabled() {
            var opts = await GeneralOptions.GetLiveInstanceAsync().Caf();
            return opts.Enabled;
        }

        public int GetVisualStudioPid() => Process.GetCurrentProcess().Id;

        public async Task<CodeLensData> LoadInstructions(
            Guid dataPointId,
            Guid projGuid,
            string filePath,
            int textStart,
            int textLen,
            CancellationToken ct) {
            try {
                var document = workspace.GetDocument(filePath, projGuid);
                var symbol = await document.GetSymbolAt(new TextSpan(textStart, textLen), ct).Caf();

                using var peStream = new MemoryStream();
                using var assembly = await document.Project.Compile(peStream, await GetOptimizationLvl().Caf(), ct).Caf();
                if (assembly is null) return CodeLensData.BuildError();

                var (data, details) = symbol switch {
                    IMethodSymbol m => assembly.GetCodeLensDataFor(m),
                    IPropertySymbol p => assembly.GetCodeLensDataFor(p),
                    _ => throw new InvalidOperationException($"Symbol {symbol} is not a method or property.")
                };

                CodeLensConnectionHandler.StoreDetailsData(dataPointId, details);

                return data;
            } catch (Exception ex) {
                LogVS(ex);
                return CodeLensData.Failure(ex.ToString());
            }
        }

        private async Task<OptimizationLevel?> GetOptimizationLvl() {
            var opts = await GeneralOptions.GetLiveInstanceAsync().Caf();
            return opts.BuildConfig switch {
                BuildConfig.Debug => OptimizationLevel.Debug,
                BuildConfig.Release => OptimizationLevel.Release,
                _ => null
            };
        }
    }
}
