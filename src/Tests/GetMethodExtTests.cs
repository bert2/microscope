#nullable enable

namespace Microscope.Tests {
    using System;

    using Microscope.VSExtension;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;

    using Shouldly;

    [TestClass]
    public class GetMethodExtTests {
        private static readonly AssemblyDefinition TestAssembly = AssemblyDefinition
            .ReadAssembly(typeof(GetMethodExtTests).Assembly.Location);

        //[TestMethod] public void ReturnsNullForMissingMethod() => new Action(() =>
        //    TestAssembly
        //    .GetMethod("Tests.TestData.Class.UnknownMethod"))
        //    .ShouldThrow<InvalidOperationException>();

        //[TestMethod] public void FindsInstanceMethod() =>
        //    TestAssembly
        //    .TryGetMethod("Tests.TestData.Class.Method")
        //    .ShouldNotBeNull();

        //[TestMethod] public void FindsStaticMethod() =>
        //    TestAssembly
        //    .TryGetMethod("Tests.TestData.Class.StaticMethod")
        //    .ShouldNotBeNull();
    }
}
