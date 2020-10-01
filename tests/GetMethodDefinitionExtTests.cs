#nullable enable

namespace Microscope.Tests {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microscope.CodeAnalysis;
    using Microscope.Shared;
    using Microscope.Tests.TestData;
    using Microscope.Tests.Util;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;

    using Shouldly;

    [TestClass]
    public class GetMethodDefinitionExtTests {
        #region Initialization

        private static readonly AssemblyDefinition testAssembly = AssemblyDefinition
            .ReadAssembly(typeof(GetMethodDefinitionExtTests).Assembly.Location);

        private static SymbolResolver symbols = null!;

        [ClassInitialize] public static async Task ClassInitialize(TestContext tc)
            => symbols = await SymbolResolver.ForMyTests(tc.CancellationTokenSource.Token).Caf();

        [ClassCleanup] public static void ClassCleanup() => symbols?.Dispose();

        #endregion Initialization

        #region Basic

        [TestMethod] public void Basic_InstanceMethod() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class>(c => c.InstanceMethod())));

        [TestMethod] public void Basic_StaticMethod() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get(Class.StaticMethod)));

        [TestMethod] public void Basic_GenericInstanceMethod() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class>(c => c.GenericInstanceMethod<int>())));

        [TestMethod] public void Basic_GenericStaticMethod() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get(Class.GenericStaticMethod<bool>)));

        [TestMethod] public void Basic_InstanceMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class>(c => c.InstanceMethodWithParam(0))));

        [TestMethod] public void Basic_StaticMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get(() => Class.StaticMethodWithParam(true))));

        [TestMethod] public void Basic_GenericInstanceMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class>(c => c.GenericInstanceMethodWithParam(0))));

        [TestMethod] public void Basic_GenericStaticMethodWithParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get(() => Class.GenericStaticMethodWithParam(true))));

        #endregion Basic

        #region GenericClass

        [TestMethod] public void GenericClass_Method() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<GenericClass<int>>(c => c.Method(0))));

        [TestMethod] public void GenericClass_GenericMethod() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<GenericClass<int>>(c => c.GenericMethod(0, true))));

        #endregion GenericClass

        #region NestedClass

        [TestMethod] public void NestedClass_Method() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class.NestedClass>(c => c.Method())));

        [TestMethod] public void NestedNestedClass_Method() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class.NestedClass.NestedNestedClass>(c => c.Method())));

        #endregion NestedClass

        #region AmbiguousClass

        [TestMethod] public void AmbiguousClass_Method() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<AmbiguousClass>(c => c.Method(0))));

        [TestMethod] public void AmbiguousClass_MethodWithGenericParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<AmbiguousClass<int>>(c => c.Method(0))));

        #endregion AmbiguousClass

        #region Overloads

        [TestMethod] public void Overloads_NoParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method())));

        [TestMethod] public void Overloads_IntParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method(0))));

        [TestMethod] public void Overloads_StringParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method(""))));

        [TestMethod] public void Overloads_GenericParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method(true))));

        [TestMethod] public void Overloads_AmbiguousGenericParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method<int>(0))));

        [TestMethod] public void Overloads_ListParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method(new List<int>()))));

        [TestMethod] public void Overloads_GenericListParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method(new List<bool>()))));

        [TestMethod] public void Overloads_AmbiguousGenericListParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method<int>(new List<int>()))));

        #endregion Overloads

        #region MoreOverloads

        [TestMethod] public void MoreOverloads_ListParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<MoreOverloads>(c => c.Method(new List<int>()))));

        [TestMethod] public void MoreOverloads_DictOfListParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<MoreOverloads>(c => c.Method(new Dictionary<string, List<int>>()))));

        [TestMethod] public void MoreOverloads_DuplicateTypeNamesInDifferenNamespaces() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<MoreOverloads>(c => c.Method(new Name1.Space1.Foo()))));

        [TestMethod] public void MoreOverloads_NestedTypeParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<MoreOverloads>(c => c.Method(new MoreOverloads.Foo<bool, char>.Bar<float, int>()))));

        [TestMethod] public void MoreOverloads_GenericMethodWithNestedTypeParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<MoreOverloads>(c => c.Method(new MoreOverloads.Foo<bool, char>.Bar<double, int>()))));

        #endregion MoreOverloads

        #region Arrays

        [TestMethod] public void Arrays_ArrayParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new int[0]))));

        [TestMethod] public void Arrays_JaggedArrayParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new[] { new[] { new int[0] } }))));

        [TestMethod] public void Arrays_MultidimArrayParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new int[0,0,0]))));

        [TestMethod] public void Arrays_ParamsArray() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method('a', 'b', 'c'))));

        [TestMethod] public void Arrays_GenericArrayParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method<int>(new int[0]))));

        [TestMethod] public void Arrays_GenericJaggedArrayParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method<int>(new[] { new[] { new int[0] } }))));

        [TestMethod] public void Arrays_GenericMultidimArrayParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method<int>(new int[0,0,0]))));

        [TestMethod] public void Arrays_GenericArrayElements1() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new Dictionary<char, int>[0]))));

        [TestMethod] public void Arrays_GenericArrayElements2() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new Dictionary<int, char>[0]))));

        [TestMethod] public void Arrays_GenericArrayElements3() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new MoreOverloads.Foo<bool, char>.Bar<float, int>[0]))));

        #endregion Arrays

        #region Refs

        [TestMethod] public void Refs_RefParam() => Should.NotThrow(() => {
            var x = 0;
            _ = testAssembly.GetMethodDefinition(
                symbols.Get<Refs>(c => c.Method(ref x)));
        });

        [TestMethod] public void Refs_InParam() => Should.NotThrow(() => {
            var x = '\0';
            _ = testAssembly.GetMethodDefinition(
                symbols.Get<Refs>(c => c.Method(in x)));
        });

        [TestMethod] public void Refs_OutParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Refs>(c => c.Method(out var x))));

        #endregion Refs

        #region Pointers

        [TestMethod] public unsafe void Pointers_PtrParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Pointers>(c => c.Method((int*)null))));

        [TestMethod] public unsafe void Pointers_PtrToPtrParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Pointers>(c => c.Method((int**)null))));

        [TestMethod] public unsafe void Pointers_PtrArrayParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Pointers>(c => c.Method(new int*[0]))));

        [TestMethod] public unsafe void Pointers_VoidPtrParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Pointers>(c => c.Method((void*)null))));

        #endregion Pointers

        #region Dynamic

        [TestMethod] public void Dynamic_IntParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Dynamic>(c => c.Method(0))));

        [TestMethod] public void Dynamic_DynamicParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Dynamic>(c => c.Method(.0))));

        #endregion Dynamic

        #region Delegate

        [TestMethod] public void Delegate_FuncParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Delegates>(c => c.Method((Func<int, string>)null!))));

        [TestMethod] public void Delegate_DelegateParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Delegates>(c => c.Method((Delegates.Qux)null!))));

        #endregion Delegate

        #region Alias

        [TestMethod] public void Alias_IntParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Alias>(c => c.Method(0))));

        [TestMethod] public void Alias_AliasedParam() => Should.NotThrow(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Alias>(c => c.Method(""))));

        #endregion Alias
    }
}
