#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;

    public readonly struct DetailsData {
        public IReadOnlyList<InstructionData>? MethodInstructions { get; }

        public IReadOnlyList<PropertyAccessor>? PropertyAccessors { get; }

        public IReadOnlyList<GeneratedType> CompilerGeneratedTypes { get; }

        public bool HasCompilerGeneratedTypes => CompilerGeneratedTypes.Count > 0;

        public bool IsMethod => MethodInstructions != null;

        public bool IsProperty => PropertyAccessors != null;

        public DetailsData(
            IReadOnlyList<InstructionData>? methodInstructions,
            IReadOnlyList<PropertyAccessor>? propertyAccessors,
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
            IReadOnlyList<PropertyAccessor> propertyAccessors,
            IReadOnlyList<GeneratedType> compilerGeneratedTypes)
            => new DetailsData(methodInstructions: null, propertyAccessors, compilerGeneratedTypes);
    }

    public readonly struct PropertyAccessor {
        public string Name { get; }

        public IReadOnlyList<InstructionData> Instructions { get; }

        public PropertyAccessor(string name, IReadOnlyList<InstructionData> instructions) {
            Name = name;
            Instructions = instructions;
        }
    }
}
