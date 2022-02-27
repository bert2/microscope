#nullable enable

namespace Microscope.CodeAnalysis.Model {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public readonly struct GeneratedMethod {
        public string Name { get; }

        public string FullName { get; }

        public string ReturnTypeName { get; }

        public string ParameterList { get; }

        public IReadOnlyList<InstructionData> Instructions { get; }

        public GeneratedMethod(
            string name,
            string fullName,
            string returnTypeName,
            string parameterList,
            IReadOnlyList<InstructionData> instructions) {
            Name = name;
            FullName = fullName;
            ReturnTypeName = returnTypeName;
            ParameterList = parameterList;
            Instructions = instructions;
        }

        public static GeneratedMethod From(MethodDefinition method) => new GeneratedMethod(
            method.Name,
            method.FullName,
            method.ReturnType.FullName,
            GetParameterList(method),
            instructions: method.HasBody
                ? method.Body.Instructions.Select(InstructionData.From).ToArray()
                : Array.Empty<InstructionData>());

        public override string ToString() => Name;

        private static string GetParameterList(MethodDefinition m) {
            var parameterListStart = m.FullName.IndexOf('(');
            return m.FullName.Substring(parameterListStart);
        }
    }
}
