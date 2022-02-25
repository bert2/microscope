#nullable enable

namespace Microscope.CodeAnalysis {
    using Microscope.CodeAnalysis.Model;
    using Microscope.Shared;

    using Microsoft.CodeAnalysis;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    using System.Linq;

    public static class GetCodeLensDataForExt {
        public static (CodeLensData, DetailsData) GetCodeLensDataFor(this AssemblyDefinition assembly, IMethodSymbol symbol) {
            var def = assembly.GetMethodDefinition(symbol);

            var details = DetailsData.ForMethod(
                methodInstructions: def.GetInstructions(),
                compilerGeneratedTypes: def.CollectGeneratedCode());

            var data = def.ToCodeLensData();

            return (data, details);
        }

        public static (CodeLensData, DetailsData) GetCodeLensDataFor(this AssemblyDefinition assembly, IPropertySymbol symbol) {
            var getDef = symbol.GetMethod is null ? null : assembly.GetMethodDefinition(symbol.GetMethod);
            var setDef = symbol.SetMethod is null ? null : assembly.GetMethodDefinition(symbol.SetMethod);

            var details = DetailsData.ForProperty(
                getterInstructions: getDef?.GetInstructions(),
                setterInstructions: setDef?.GetInstructions(),
                compilerGeneratedTypes: (getDef, setDef).CollectGeneratedCode());

            var data = (getDef, setDef).ToCodeLensData();

            return (data, details);
        }

        private static InstructionData[] GetInstructions(this MethodDefinition def) {
            var instrs = def.HasBody ? def.Body.Instructions : new Collection<Instruction>(capacity: 0);
            return instrs.Select(InstructionData.From).ToArray();
        }
    }
}
