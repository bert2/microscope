#nullable enable

namespace Microscope.Tests {
    using System.Collections.Generic;
    using System.Linq;

    using Microscope.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil.Cil;

    using Shouldly;

    [TestClass]
    public class DocumentationTests {
        // `System.Reflection.Emit.OpCodes` doesn't know of `ldelem.any`.
        [TestMethod] public void ReturnsNullForUnknownOpCode() =>
            OpCodes.Ldelem_Any
            .GetDocumentation()
            .ShouldBeNull();

        [TestMethod] public void KnowsAllOpCodes() =>
            AllKnownInstructions
            .Where(i => i.GetRawDocumentation() == null)
            .ShouldBeEmpty();

        [TestMethod] public void RemovesParamrefTags() =>
            OpCodes.Brtrue
            .GetDocumentation()
            .ShouldBe("Transfers control to a target instruction if `value` is true, not null, or non-zero.");

        [TestMethod] public void RemovesSeeCrefTags() =>
            OpCodes.Ckfinite
            .GetDocumentation()
            .ShouldBe("Throws `System.ArithmeticException` if value is not a finite number.");

        [TestMethod] public void ReturnsDocumentationForInstructionWithTrailingDot() =>
            OpCodes.Constrained
            .GetDocumentation()
            .ShouldBe("Constrains the type on which a virtual method call is made.");

        // The documentation was generated from all opcodes in `System.Reflection.Emit.OpCodes`, but the
        // actual instructions are retrieved as `Mono.Cecil.Cil.OpCodes`.
        private static IEnumerable<string> AllKnownInstructions => typeof(System.Reflection.Emit.OpCodes)
            .GetFields()
            .Select(f => ((System.Reflection.Emit.OpCode)f.GetValue(null)).Name);
    }
}
