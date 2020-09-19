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

        private static Instruction Instruction(string opCode) => new Instruction(label: null, opCode, operand: null);

        private static IEnumerable<Instruction> AllKnownInstructions => typeof(OpCodes)
            .GetFields()
            .Select(f => Instruction(f.Name.ToLower().Replace('_', '.')));
    }
}
