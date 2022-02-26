#nullable enable

namespace Microscope.CodeAnalysis {
    using Microscope.CodeAnalysis.Model;
    using Microscope.Shared;

    using Microsoft.CodeAnalysis;

    using Mono.Cecil;

    using System.Collections.Generic;
    using System.Linq;

    public static class GetCodeLensDataForExt {
        public static (CodeLensData, DetailsData) GetCodeLensDataFor(this AssemblyDefinition assembly, IMethodSymbol symbol) {
            var def = assembly.GetMethodDefinition(symbol);

            var details = DetailsData.ForMethod(
                methodInstructions: def.GetInstructions().ToArray(),
                compilerGeneratedTypes: def.CollectGeneratedCode());

            var data = def.ToCodeLensData();

            return (data, details);
        }

        public static (CodeLensData, DetailsData) GetCodeLensDataFor(this AssemblyDefinition assembly, IPropertySymbol symbol) {
            var getDef = symbol.GetMethod is null ? null : assembly.GetMethodDefinition(symbol.GetMethod);
            var setDef = symbol.SetMethod is null ? null : assembly.GetMethodDefinition(symbol.SetMethod);

            var details = DetailsData.ForProperty(
                propertyAccessors: (getDef, setDef).GetInstructions().ToArray(),
                compilerGeneratedTypes: (getDef, setDef).CollectGeneratedCode());

            var data = (getDef, setDef).ToCodeLensData();

            return (data, details);
        }

        private static IEnumerable<InstructionData> GetInstructions(this MethodDefinition def)
            => def.HasBody ? def.Body.Instructions.Select(InstructionData.From) : Enumerable.Empty<InstructionData>();

        private static IEnumerable<PropertyAccessor> GetInstructions(this (MethodDefinition? get, MethodDefinition? set) prop) {
            if (prop.get != null)
                yield return new PropertyAccessor(prop.get.Name, prop.get.GetInstructions().ToArray());

            if (prop.set != null)
                yield return new PropertyAccessor(prop.set.Name, prop.set.GetInstructions().ToArray());
        }
    }
}
