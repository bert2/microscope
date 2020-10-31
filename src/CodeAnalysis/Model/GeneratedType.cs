#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public readonly struct GeneratedType {
        public string Name { get; }

        public string FullName { get; }

        public IReadOnlyList<GeneratedMethod> Methods { get; }

        public GeneratedType(string name, string fullName, IReadOnlyList<GeneratedMethod> methods) {
            Name = name;
            FullName = fullName;
            Methods = methods;
        }

        public static GeneratedType From(IGrouping<TypeDefinition, MethodDefinition> g) => new GeneratedType(
            g.Key.Name,
            g.Key.FullName,
            g.Select(GeneratedMethod.From).ToArray());

        public override string ToString() => Name;
    }
}
