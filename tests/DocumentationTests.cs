#nullable enable

namespace Microscope.Tests {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Microscope.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Shouldly;

    [TestClass]
    public class DocumentationTests {
        [TestMethod] public void ReturnsNullForUnknownOpCode() =>
            Documentation.For("unknown")
            .ShouldBeNull();

        [TestMethod] public void KnowsAllOpCodes() =>
            AllKnownInstructions
            .Where(i => Documentation.For(i) == null)
            .ShouldBeEmpty();

        [TestMethod] public void RemovesParamrefTags() =>
            Documentation.For("brtrue")
            .ShouldBe("Transfers control to a target instruction if `value` is true, not null, or non-zero.");

        [TestMethod] public void RemovesSeeCrefTags() =>
            Documentation.For("ckfinite")
            .ShouldBe("Throws `System.ArithmeticException` if value is not a finite number.");

        [TestMethod] public void ReturnsDocumentationForInstructionWithTrailingDot() =>
            Documentation.For("constrained.")
            .ShouldBe("Constrains the type on which a virtual method call is made.");

        private static IEnumerable<string> AllKnownInstructions => typeof(OpCodes)
            .GetFields()
            .Select(f => ((OpCode)f.GetValue(null)).Name);
    }
}
