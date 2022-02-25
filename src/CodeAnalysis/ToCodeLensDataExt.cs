#nullable enable

namespace Microscope.CodeAnalysis {
    using Microscope.Shared;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    using System;

    public static class ToCodeLensDataExt {
        public static CodeLensData ToCodeLensData(this MethodDefinition method) {
            var instrs = method.Body?.Instructions ?? new Collection<Instruction>(capacity: 0);
            var boxOpsCount = 0;
            var callvirtOpsCount = 0;

            foreach (var instr in instrs) {
                if (instr.OpCode.Code == Code.Box)
                    boxOpsCount++;
                else if (instr.OpCode.Code == Code.Callvirt && instr.Previous.OpCode.Code != Code.Constrained)
                    callvirtOpsCount++;
            }

            return CodeLensData.Success(instrs.Count, boxOpsCount, callvirtOpsCount, method.Body?.CodeSize ?? 0);
        }

        public static CodeLensData ToCodeLensData(this (MethodDefinition? get, MethodDefinition? set) property)
            => (property.get, property.set) switch {
                (null   , null   ) => throw new InvalidOperationException("Property must have a set or a get method."),
                (var get, null   ) => get.ToCodeLensData(),
                (null   , var set) => set.ToCodeLensData(),
                (var get, var set) => get.ToCodeLensData().Merge(set.ToCodeLensData())
            };
    }
}
