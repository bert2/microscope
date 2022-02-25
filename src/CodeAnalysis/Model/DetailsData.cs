#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;

    public readonly struct DetailsData {
        public IReadOnlyList<InstructionData>? MethodInstructions { get; }

        public IReadOnlyList<InstructionData>? GetterInstructions { get; }

        public IReadOnlyList<InstructionData>? SetterInstructions { get; }

        public IReadOnlyList<GeneratedType> CompilerGeneratedTypes { get; }

        public bool HasCompilerGeneratedTypes => CompilerGeneratedTypes.Count > 0;

        public bool IsMethod => MethodInstructions != null;

        public bool IsProperty => GetterInstructions != null || SetterInstructions != null;

        public bool HasGetter => GetterInstructions != null;

        public bool HasSetter => SetterInstructions != null;

        public DetailsData(
            IReadOnlyList<InstructionData>? methodInstructions,
            IReadOnlyList<InstructionData>? getterInstructions,
            IReadOnlyList<InstructionData>? setterInstructions,
            IReadOnlyList<GeneratedType> compilerGeneratedTypes) {
            MethodInstructions = methodInstructions;
            GetterInstructions = getterInstructions;
            SetterInstructions = setterInstructions;
            CompilerGeneratedTypes = compilerGeneratedTypes;
        }

        public static DetailsData ForMethod(
            IReadOnlyList<InstructionData> methodInstructions,
            IReadOnlyList<GeneratedType> compilerGeneratedTypes)
            => new DetailsData(
                methodInstructions: methodInstructions,
                getterInstructions: null,
                setterInstructions: null,
                compilerGeneratedTypes);

        public static DetailsData ForProperty(
            IReadOnlyList<InstructionData>? getterInstructions,
            IReadOnlyList<InstructionData>? setterInstructions,
            IReadOnlyList<GeneratedType> compilerGeneratedTypes)
            => new DetailsData(
                methodInstructions: null,
                getterInstructions: getterInstructions,
                setterInstructions: setterInstructions,
                compilerGeneratedTypes);
    }
}
