#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;

    public readonly struct DetailsData {
        public IReadOnlyList<InstructionData>? MethodInstructions { get; }

        public IReadOnlyList<GeneratedMethod>? PropertyAccessors { get; }

        public IReadOnlyList<GeneratedType> CompilerGeneratedTypes { get; }

        public bool HasCompilerGeneratedTypes => CompilerGeneratedTypes.Count > 0;

        public bool IsMethod => MethodInstructions != null;

        public bool IsProperty => PropertyAccessors != null;

        public DetailsData(
            IReadOnlyList<InstructionData>? methodInstructions,
            IReadOnlyList<GeneratedMethod>? propertyAccessors,
            IReadOnlyList<GeneratedType> compilerGeneratedTypes) {
            MethodInstructions = methodInstructions;
            PropertyAccessors = propertyAccessors;
            CompilerGeneratedTypes = compilerGeneratedTypes;
        }

        public static DetailsData ForMethod(
            IReadOnlyList<InstructionData> methodInstructions,
            IReadOnlyList<GeneratedType> compilerGeneratedTypes)
            => new DetailsData(methodInstructions, propertyAccessors: null, compilerGeneratedTypes);

        public static DetailsData ForProperty(
            IReadOnlyList<GeneratedMethod> propertyAccessors,
            IReadOnlyList<GeneratedType> compilerGeneratedTypes)
            => new DetailsData(methodInstructions: null, propertyAccessors, compilerGeneratedTypes);
    }
}
