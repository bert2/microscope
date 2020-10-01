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

    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Utilities;

    using static Microscope.Shared.Logging;

    [Export(typeof(ICodeLensCallbackListener))]
    [ContentType("CSharp")]
    public class InstructionsProvider : ICodeLensCallbackListener, IInstructionsProvider {
        private readonly VisualStudioWorkspace workspace;

        [ImportingConstructor]
        public InstructionsProvider(VisualStudioWorkspace workspace) => this.workspace = workspace;

        public async Task<CodeLensData> GetInstructions(Guid projGuid, string filePath, int textStart, int textLen, CancellationToken ct) {
            try {
                var doc = workspace.GetDocument(filePath, projGuid);
                var method = await doc.GetMethodSymbolAt(new TextSpan(textStart, textLen), ct).Caf();

                using var peStream = new MemoryStream();
                using var assembly = await doc.Project.Compile(peStream, ct).Caf();

                return assembly is null
                    ? CodeLensData.CompilerError()
                    : assembly
                        .GetMethodDefinition(method)
                        .Body?
                        .Instructions
                        .ToCodeLensData()
                        ?? CodeLensData.Empty();
            } catch (Exception ex) {
                LogVS(ex);
                return CodeLensData.Failure(ex.ToString());
            }
        }

        public int GetVisualStudioPid() => Process.GetCurrentProcess().Id;

        public async Task<bool> IsMicroscopeEnabled() {
            var opts = await GeneralOptions.GetLiveInstanceAsync().Caf();
            return opts.Enabled;
        }
    }
}
