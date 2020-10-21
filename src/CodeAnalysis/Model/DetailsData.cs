#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;

    public readonly struct DetailsData {
        public IReadOnlyList<InstructionData> MethodInstructions { get; }

        public IReadOnlyList<GeneratedType> CompilerGeneratedTypes { get; }

        public bool HasCompilerGeneratedTypes => CompilerGeneratedTypes.Count > 0;

        public DetailsData(
            IReadOnlyList<InstructionData> methodInstructions,
            IReadOnlyList<GeneratedType> compilerGeneratedTypes) {
            MethodInstructions = methodInstructions;
            CompilerGeneratedTypes = compilerGeneratedTypes;
        }
    }
}
