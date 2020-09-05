#nullable enable

namespace Microscope.Tests {
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microscope.Tests.TestData;
    using Microscope.Tests.Util;
    using Microscope.VSExtension;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;

    using Shouldly;

    [TestClass]
    public class GetMethodExtTests {
        #region Initialization

        private static readonly AssemblyDefinition testAssembly = AssemblyDefinition
            .ReadAssembly(typeof(GetMethodExtTests).Assembly.Location);

        private static SymbolResolver symbols = null!;

        [ClassInitialize] public static async Task ClassInitialize(TestContext tc)
            => symbols = await SymbolResolver
                .ForMyTests(tc.CancellationTokenSource.Token)
                .ConfigureAwait(false);

        [ClassCleanup] public static void ClassCleanup() => symbols?.Dispose();

        #endregion Initialization

        #region Basic

        [TestMethod] public void Basic_InstanceMethod() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Class>(c => c.InstanceMethod())));

        [TestMethod] public void Basic_StaticMethod() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get(Class.StaticMethod)));

        [TestMethod] public void Basic_GenericInstanceMethod() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Class>(c => c.GenericInstanceMethod<int>())));

        [TestMethod] public void Basic_GenericStaticMethod() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get(Class.GenericStaticMethod<bool>)));

        [TestMethod] public void Basic_InstanceMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Class>(c => c.InstanceMethodWithParam(0))));

        [TestMethod] public void Basic_StaticMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get(() => Class.StaticMethodWithParam(true))));

        [TestMethod] public void Basic_GenericInstanceMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Class>(c => c.GenericInstanceMethodWithParam(0))));

        [TestMethod] public void Basic_GenericStaticMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get(() => Class.GenericStaticMethodWithParam(true))));

        #endregion Basic

        #region GenericClass

        [TestMethod] public void GenericClass_Method() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<GenericClass<int>>(c => c.Method(0))));

        [TestMethod] public void GenericClass_GenericMethod() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<GenericClass<int>>(c => c.GenericMethod(0, true))));

        #endregion GenericClass

        #region NestedClass

        [TestMethod] public void NestedClass_Method() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Class.NestedClass>(c => c.Method())));

        [TestMethod] public void NestedNestedClass_Method() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Class.NestedClass.NestedNestedClass>(c => c.Method())));

        #endregion NestedClass

        #region AmbiguousClass

        [TestMethod] public void AmbiguousClass_Method() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<AmbiguousClass>(c => c.Method(0))));

        [TestMethod] public void AmbiguousClass_MethodWithGenericParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<AmbiguousClass<int>>(c => c.Method(0))));

        #endregion AmbiguousClass

        #region Overloads

        [TestMethod] public void Overloads_MethodWithoutParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Overloads>(c => c.Method())));

        [TestMethod] public void Overloads_MethodWithIntParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Overloads>(c => c.Method(0))));

        [TestMethod] public void Overloads_MethodWithStringParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Overloads>(c => c.Method(""))));

        [TestMethod] public void Overloads_MethodWithGenericParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Overloads>(c => c.Method(true))));

        [TestMethod] public void Overloads_MethodWithAmbiguousGenericParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Overloads>(c => c.Method<int>(0))));

        [TestMethod] public void Overloads_MethodWithListParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Overloads>(c => c.Method(new List<int>()))));

        [TestMethod] public void Overloads_MethodWithGenericListParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Overloads>(c => c.Method(new List<bool>()))));

        [TestMethod] public void Overloads_MethodWithAmbiguousGenericListParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<Overloads>(c => c.Method<int>(new List<int>()))));

        #endregion Overloads

        #region MoreOverloads

        [TestMethod] public void MoreOverloads_MethodWithListParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<MoreOverloads>(c => c.Method(new List<int>()))));

        [TestMethod] public void MoreOverloads_MethodWithDictOfListParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<MoreOverloads>(c => c.Method(new Dictionary<string, List<int>>()))));

        [TestMethod] public void MoreOverloads_DuplicateTypeNamesInDifferenNamespaces() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<MoreOverloads>(c => c.Method(new Name1.Space1.Foo()))));

        [TestMethod] public void MoreOverloads_MethodWithNestedTypeParam() => Should.NotThrow(() =>
            testAssembly.GetMethod(
                symbols.Get<MoreOverloads>(c => c.Method(new MoreOverloads.Foo<bool, char>.Bar<float, int>()))));

        #endregion MoreOverloads
    }
}
