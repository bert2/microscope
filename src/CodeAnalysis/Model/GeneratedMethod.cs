#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public readonly struct GeneratedMethod {
        public string Name { get; }

        public IReadOnlyList<InstructionData> Instructions { get; }

        public GeneratedMethod(string name, IReadOnlyList<InstructionData> instructions) {
            Name = name;
            Instructions = instructions;
        }

        public static GeneratedMethod From(MethodDefinition method) => new GeneratedMethod(
            method.Name,
            method.Body.Instructions.Select(InstructionData.From).ToArray());

        public override string ToString() => Name;
    }
}
