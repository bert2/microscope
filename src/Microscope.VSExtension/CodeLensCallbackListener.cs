#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.ComponentModel.Composition;

    using Microscope.Shared;

    using Microsoft.VisualStudio.Language.CodeLens;
    using Microsoft.VisualStudio.Utilities;

    using static Microscope.Shared.Logging;

    [Export(typeof(ICodeLensCallbackListener))]
    [ContentType("CSharp")]
    public class CodeLensCallbackListener : ICodeLensCallbackListener, ICodeLensContext {
        static CodeLensCallbackListener() {
            Log();
        }

        [ImportingConstructor]
        public CodeLensCallbackListener() {
            try {
                Log();
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }

        public int Foo() {
            try {
                Log();
                return 42;
            } catch (Exception ex) {
                Log(ex);
                throw;
            }
        }
    }
}
