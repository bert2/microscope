#nullable enable

namespace Tests {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microscope.VSExtension;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;

    using Shouldly;

    [TestClass]
    public class GetMethodExtTests {
        private static readonly AssemblyDefinition TestAssembly = AssemblyDefinition
            .ReadAssembly(typeof(GetMethodExtTests).Assembly.Location);

        private IEnumerable<char> foo = new[] { "test" }.SelectMany(x => x).ToArray();

        [TestMethod] public void ThrowsForMissingMethod() => new Action(() =>
            TestAssembly
            .GetMethod("Tests.TestData.Class.UnknownMethod", null!))
            .ShouldThrow<InvalidOperationException>();

        [TestMethod] public void FindsInstanceMethod() =>
            TestAssembly
            .GetMethod("Tests.TestData.Class.Method", null!)
            .ShouldNotBeNull();

        [TestMethod] public void FindsStaticMethod() =>
            TestAssembly
            .GetMethod("Tests.TestData.Class.StaticMethod", null!)
            .ShouldNotBeNull();
    }
}
