#nullable enable

namespace Microscope.Tests {
    using System.Threading.Tasks;

    using Microscope.Tests.TestData;
    using Microscope.Tests.Util;
    using Microscope.VSExtension;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;

    using Shouldly;

    [TestClass]
    public class GetMethodExtTests {
        private static readonly AssemblyDefinition testAssembly = AssemblyDefinition
            .ReadAssembly(typeof(GetMethodExtTests).Assembly.Location);

        private static SymbolResolver symbols = null!;

        [ClassInitialize] public static async Task ClassInitialize(TestContext tc)
            => symbols = await SymbolResolver
                .ForMyTests(tc.CancellationTokenSource.Token)
                .ConfigureAwait(false);

        [ClassCleanup] public static void ClassCleanup() => symbols?.Dispose();

        [TestMethod] public void InstanceMethod() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Class>(c => c.InstanceMethod())));

        [TestMethod] public void StaticMethod() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get(Class.StaticMethod)));

        [TestMethod] public void GenericInstanceMethod() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Class>(c => c.GenericInstanceMethod<int>())));

        [TestMethod] public void GenericStaticMethod() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get(Class.GenericStaticMethod<bool>)));

        [TestMethod] public void InstanceMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Class>(c => c.InstanceMethodWithParam(0))));

        [TestMethod] public void StaticMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get(() => Class.StaticMethodWithParam(true))));

        [TestMethod] public void GenericInstanceMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Class>(c => c.GenericInstanceMethodWithParam(0))));

        [TestMethod] public void GenericStaticMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get(() => Class.GenericStaticMethodWithParam(true))));

        [TestClass]
        public class GenericClass {
            [TestMethod] public void Method() => Should.NotThrow(() =>
                testAssembly.GetMethod(
                    symbols.Get<GenericClass<int>>(c => c.Method(0))));

            [TestMethod] public void GenericMethod() => Should.NotThrow(() =>
                testAssembly.GetMethod(
                    symbols.Get<GenericClass<int>>(c => c.GenericMethod(0, false))));
        }

        [TestClass]
        public class NestedClass {
            [TestMethod] public void Method() => Should.NotThrow(() =>
                testAssembly.GetMethod(
                    symbols.Get<Class.NestedClass>(c => c.Method())));
        }

        [TestClass]
        public class NestedNestedClass {
            [TestMethod] public void Method() => Should.NotThrow(() =>
                testAssembly.GetMethod(
                    symbols.Get<Class.NestedClass.NestedNestedClass>(c => c.Method())));
        }
    }
}
