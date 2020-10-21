#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public readonly struct GeneratedType {
        public string TypeName { get; }

        public IReadOnlyList<GeneratedMethod> Methods { get; }

        public GeneratedType(string typeName, IReadOnlyList<GeneratedMethod> methods) {
            TypeName = typeName;
            Methods = methods;
        }

        public static GeneratedType From(TypeDefinition type) => new GeneratedType(
            type.Name,
            type.Methods.Select(GeneratedMethod.From).ToArray());

        public override string ToString() => TypeName;
    }
}
