#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    using static CollectGeneratedCodeExt;

    public readonly struct GeneratedType {
        public string Name { get; }

        public string FullName { get; }

        public IReadOnlyList<GeneratedMethod> Methods { get; }

        public GeneratedType(string name, string fullName, IReadOnlyList<GeneratedMethod> methods) {
            Name = name;
            FullName = fullName;
            Methods = methods;
        }

        public static GeneratedType From(TypeDefinition type, MethodDefinition sourceMethod) => new GeneratedType(
            type.Name,
            type.FullName,
            type.Methods
                .Where(IsRelatedTo(sourceMethod))
                .Select(GeneratedMethod.From)
                .ToArray());

        public override string ToString() => Name;
    }
}
