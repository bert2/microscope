#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;

    public class DetailsData {
        public List<InstructionData> MethodInstructions { get; set; }

        public List<GeneratedType> CompilerGeneratedTypes { get; set; }

        public bool HasCompilerGeneratedTypes => CompilerGeneratedTypes.Count > 0;

        public DetailsData(List<InstructionData> methodInstructions, List<GeneratedType> compilerGeneratedTypes) {
            MethodInstructions = methodInstructions;
            CompilerGeneratedTypes = compilerGeneratedTypes;
        }
    }
}
