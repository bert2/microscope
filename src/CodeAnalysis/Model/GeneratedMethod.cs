#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public class GeneratedMethod {
        public string MethodName { get; set; }

        public List<InstructionData> Instructions { get; set; }

        public GeneratedMethod(string methodName, List<InstructionData> instructions) {
            MethodName = methodName;
            Instructions = instructions;
        }

        public static GeneratedMethod From(MethodDefinition method) => new GeneratedMethod(
            method.Name,
            method.Body.Instructions.Select(InstructionData.From).ToList());

        public override string ToString() => MethodName;
    }
}
