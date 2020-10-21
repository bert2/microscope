#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public class GeneratedType {
        public string TypeName { get; set; }

        public List<GeneratedMethod> Methods { get; set; }

        public GeneratedType(string typeName, List<GeneratedMethod> methods) {
            TypeName = typeName;
            Methods = methods;
        }

        public static GeneratedType From(TypeDefinition type) => new GeneratedType(
            type.Name,
            type.Methods.Select(GeneratedMethod.From).ToList());

        public override string ToString() => TypeName;
    }
}
