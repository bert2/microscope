#nullable enable

namespace Tests {
    using Microscope.VSExtension;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;

    using Shouldly;

    [TestClass]
    public class GetMethodExtTests {
        private static readonly AssemblyDefinition TestAssembly = AssemblyDefinition
            .ReadAssembly(typeof(GetMethodExtTests).Assembly.Location);

        [TestMethod] public void ReturnsNullForMissingMethod() =>
            TestAssembly
            .TryGetMethod("Tests.TestData.Class.UnknownMethod")
            .ShouldBeNull();

        [TestMethod] public void FindsInstanceMethod() =>
            TestAssembly
            .TryGetMethod("Tests.TestData.Class.Method")
            .ShouldNotBeNull();

        [TestMethod] public void FindsStaticMethod() =>
            TestAssembly
            .TryGetMethod("Tests.TestData.Class.StaticMethod")
            .ShouldNotBeNull();
    }
}
