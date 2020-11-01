#nullable enable

namespace Microscope.Tests {
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Microscope.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil.Cil;

    [TestClass]
    public class DocumentationTests {
        // `System.Reflection.Emit.OpCodes` doesn't know of `ldelem.any`.
        [TestMethod] public void ReturnsNullForUnknownOpCode() =>
            OpCodes.Ldelem_Any
            .GetDocumentation()
            .Should().BeNull();

        [TestMethod] public void KnowsAllOpCodes() =>
            AllKnownInstructions
            .Where(i => i.GetRawDocumentation() == null)
            .Should().BeEmpty();

        [TestMethod] public void RemovesParamrefTags() =>
            OpCodes.Brtrue
            .GetDocumentation()
            .Should().Be("Transfers control to a target instruction if `value` is true, not null, or non-zero.");

        [TestMethod] public void RemovesSeeCrefTags() =>
            OpCodes.Ckfinite
            .GetDocumentation()
            .Should().Be("Throws `System.ArithmeticException` if value is not a finite number.");

        [TestMethod] public void ReturnsDocumentationForInstructionWithTrailingDot() =>
            OpCodes.Constrained
            .GetDocumentation()
            .Should().Be("Constrains the type on which a virtual method call is made.");

        // The documentation was generated from all opcodes in `System.Reflection.Emit.OpCodes`, but the
        // actual instructions are retrieved as `Mono.Cecil.Cil.OpCodes`.
        private static IEnumerable<string> AllKnownInstructions => typeof(System.Reflection.Emit.OpCodes)
            .GetFields()
            .Select(f => ((System.Reflection.Emit.OpCode)f.GetValue(null)).Name);
    }
}
