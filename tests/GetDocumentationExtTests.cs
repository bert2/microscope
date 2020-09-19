namespace Microscope.Tests {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Microscope.CodeLensProvider;
    using Microscope.Shared;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Shouldly;

    [TestClass]
    public class GetDocumentationExtTests {
        [TestMethod] public void ReturnsNullForUnknownOpCode() =>
            Instruction("unknown")
            .GetDocumentation()
            .ShouldBeNull();

        [TestMethod] public void KnowsAllOpCodes() =>
            AllKnownInstructions
            .Where(i => i.GetDocumentation() == null)
            .ShouldBeEmpty();

        [TestMethod] public void RemovesParamrefTags() =>
            Instruction("brtrue")
            .GetDocumentation()
            .ShouldBe("Transfers control to a target instruction if `value` is true, not null, or non-zero.");

        [TestMethod] public void RemovesSeeCrefTags() =>
            Instruction("ckfinite")
            .GetDocumentation()
            .ShouldBe("Throws `System.ArithmeticException` if value is not a finite number.");

        private static Instruction Instruction(string opCode) => new Instruction(label: null, opCode, operand: null);

        private static IEnumerable<Instruction> AllKnownInstructions => typeof(OpCodes)
            .GetFields()
            .Select(f => Instruction(f.Name.ToLower().Replace('_', '.')));
    }
}
