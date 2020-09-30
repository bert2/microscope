#nullable enable

namespace Microscope.Tests {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Microscope.Shared;
    using Microscope.VSExtension;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Shouldly;

    [TestClass]
    public class DocumentationTests {
        [TestMethod] public void ReturnsNullForUnknownOpCode() =>
            Documentation.For(Instruction("unknown"))
            .ShouldBeNull();

        [TestMethod] public void KnowsAllOpCodes() =>
            AllKnownInstructions
            .Where(i => Documentation.For(i) == null)
            .ShouldBeEmpty();

        [TestMethod] public void RemovesParamrefTags() =>
            Documentation.For(Instruction("brtrue"))
            .ShouldBe("Transfers control to a target instruction if `value` is true, not null, or non-zero.");

        [TestMethod] public void RemovesSeeCrefTags() =>
            Documentation.For(Instruction("ckfinite"))
            .ShouldBe("Throws `System.ArithmeticException` if value is not a finite number.");

        private static Instruction Instruction(string opCode) => new Instruction(label: null!, opCode, operand: null!);

        private static IEnumerable<Instruction> AllKnownInstructions => typeof(OpCodes)
            .GetFields()
            .Select(f => Instruction(f.Name.ToLower().Replace('_', '.')));
    }
}
