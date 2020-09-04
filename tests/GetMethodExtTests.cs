#nullable enable

namespace Microscope.Tests {
    using System.Threading.Tasks;

    using Microscope.Tests.Util;
    using Microscope.VSExtension;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;

    using Shouldly;

    [TestClass]
    public class GetMethodExtTests {
        private static readonly AssemblyDefinition TestAssembly = AssemblyDefinition
            .ReadAssembly(typeof(GetMethodExtTests).Assembly.Location);

        private static SymbolResolver symbols = null!;

        [ClassInitialize] public static async Task ClassInitialize(TestContext tc)
            => symbols = await SymbolResolver
                .ForMyTests(tc.CancellationTokenSource.Token)
                .ConfigureAwait(false);

        [ClassCleanup] public static void ClassCleanup() => symbols?.Dispose();

        [TestMethod] public void FindsInstanceMethod() {
            var symbol = symbols.Get<TestData.Class>(c => c.Method());
            var method = TestAssembly.GetMethod(symbol);
            method.ShouldNotBeNull();
        }
    }
}
