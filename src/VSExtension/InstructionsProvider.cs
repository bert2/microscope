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

    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    using static Microscope.Shared.Logging;

    [Export(typeof(ICodeLensCallbackListener))]
    [ContentType("CSharp")]
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
                var method = await document.GetMethodSymbolAt(new TextSpan(textStart, textLen), ct).Caf();

                using var peStream = new MemoryStream();
                using var assembly = await document.Project.Compile(peStream, ct).Caf();
                if (assembly is null) return CodeLensData.CompilerError();

                var instructions = assembly
                    .GetMethodDefinition(method)
                    .Body?
                    .Instructions
                    ?? new Collection<Instruction>(capacity: 0);

                CodeLensConnectionHandler.StoreInstructions(dataPointId, instructions);

                return instructions.ToCodeLensData();
            } catch (Exception ex) {
                LogVS(ex);
                return CodeLensData.Failure(ex.ToString());
            }
        }
    }
}
