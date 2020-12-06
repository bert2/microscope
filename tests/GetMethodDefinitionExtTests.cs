#nullable enable

namespace Microscope.Tests {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Microscope.CodeAnalysis;
    using Microscope.Shared;
    using Microscope.Tests.TestData;
    using Microscope.Tests.Util;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;

    using static FluentAssertions.FluentActions;

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

        [TestMethod] public void Basic_InstanceMethod() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class>(c => c.InstanceMethod())))
            .Should().NotThrow();

        [TestMethod] public void Basic_StaticMethod() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get(Class.StaticMethod)))
            .Should().NotThrow();

        [TestMethod] public void Basic_GenericInstanceMethod() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class>(c => c.GenericInstanceMethod<int>())))
            .Should().NotThrow();

        [TestMethod] public void Basic_GenericStaticMethod() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get(Class.GenericStaticMethod<bool>)))
            .Should().NotThrow();

        [TestMethod] public void Basic_InstanceMethodWithParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class>(c => c.InstanceMethodWithParam(0))))
            .Should().NotThrow();

        [TestMethod] public void Basic_StaticMethodWithParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get(() => Class.StaticMethodWithParam(true))))
            .Should().NotThrow();

        [TestMethod] public void Basic_GenericInstanceMethodWithParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class>(c => c.GenericInstanceMethodWithParam(0))))
            .Should().NotThrow();

        [TestMethod] public void Basic_GenericStaticMethodWithParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get(() => Class.GenericStaticMethodWithParam(true))))
            .Should().NotThrow();

        #endregion Basic

        #region GenericClass

        [TestMethod] public void GenericClass_Method() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<GenericClass<int>>(c => c.Method(0))))
            .Should().NotThrow();

        [TestMethod] public void GenericClass_GenericMethod() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<GenericClass<int>>(c => c.GenericMethod(0, true))))
            .Should().NotThrow();

        #endregion GenericClass

        #region NestedClass

        [TestMethod] public void NestedClass_Method() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class.NestedClass>(c => c.Method())));

        [TestMethod] public void NestedNestedClass_Method() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Class.NestedClass.NestedNestedClass>(c => c.Method())))
            .Should().NotThrow();

        #endregion NestedClass

        #region AmbiguousClass

        [TestMethod] public void AmbiguousClass_Method() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<AmbiguousClass>(c => c.Method(0))))
            .Should().NotThrow();

        [TestMethod] public void AmbiguousClass_MethodWithGenericParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<AmbiguousClass<int>>(c => c.Method(0))))
            .Should().NotThrow();

        #endregion AmbiguousClass

        #region Overloads

        [TestMethod] public void Overloads_NoParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method())))
            .Should().NotThrow();

        [TestMethod] public void Overloads_IntParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method(0))))
            .Should().NotThrow();

        [TestMethod] public void Overloads_StringParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method(""))))
            .Should().NotThrow();

        [TestMethod] public void Overloads_GenericParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method(true))))
            .Should().NotThrow();

        [TestMethod] public void Overloads_AmbiguousGenericParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method<int>(0))))
            .Should().NotThrow();

        [TestMethod] public void Overloads_ListParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method(new List<int>()))))
            .Should().NotThrow();

        [TestMethod] public void Overloads_GenericListParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method(new List<bool>()))))
            .Should().NotThrow();

        [TestMethod] public void Overloads_AmbiguousGenericListParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Overloads>(c => c.Method<int>(new List<int>()))))
            .Should().NotThrow();

        #endregion Overloads

        #region MoreOverloads

        [TestMethod] public void MoreOverloads_ListParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<MoreOverloads>(c => c.Method(new List<int>()))))
            .Should().NotThrow();

        [TestMethod] public void MoreOverloads_DictOfListParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<MoreOverloads>(c => c.Method(new Dictionary<string, List<int>>()))))
            .Should().NotThrow();

        [TestMethod] public void MoreOverloads_DuplicateTypeNamesInDifferentNamespaces() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<MoreOverloads>(c => c.Method(new Name1.Space1.Foo()))))
            .Should().NotThrow();

        [TestMethod] public void MoreOverloads_NestedTypeParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<MoreOverloads>(c => c.Method(new MoreOverloads.Foo<bool, char>.Bar<float, int>()))))
            .Should().NotThrow();

        [TestMethod] public void MoreOverloads_GenericMethodWithNestedTypeParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<MoreOverloads>(c => c.Method(new MoreOverloads.Foo<bool, char>.Bar<double, int>()))))
            .Should().NotThrow();

        #endregion MoreOverloads

        #region Arrays

        [TestMethod] public void Arrays_ArrayParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new int[0]))))
            .Should().NotThrow();

        [TestMethod] public void Arrays_JaggedArrayParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new[] { new[] { new int[0] } }))))
            .Should().NotThrow();

        [TestMethod] public void Arrays_MultidimArrayParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new int[0,0,0]))))
            .Should().NotThrow();

        [TestMethod] public void Arrays_ParamsArray() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method('a', 'b', 'c'))))
            .Should().NotThrow();

        [TestMethod] public void Arrays_GenericArrayParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method<int>(new int[0]))))
            .Should().NotThrow();

        [TestMethod] public void Arrays_GenericJaggedArrayParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method<int>(new[] { new[] { new int[0] } }))))
            .Should().NotThrow();

        [TestMethod] public void Arrays_GenericMultidimArrayParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method<int>(new int[0,0,0]))))
            .Should().NotThrow();

        [TestMethod] public void Arrays_GenericArrayElements1() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new Dictionary<char, int>[0]))))
            .Should().NotThrow();

        [TestMethod] public void Arrays_GenericArrayElements2() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new Dictionary<int, char>[0]))))
            .Should().NotThrow();

        [TestMethod] public void Arrays_GenericArrayElements3() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Arrays>(c => c.Method(new MoreOverloads.Foo<bool, char>.Bar<float, int>[0]))))
            .Should().NotThrow();

        #endregion Arrays

        #region Refs

        [TestMethod] public void Refs_RefParam() => Invoking(() => {
                var x = 0;
                _ = testAssembly.GetMethodDefinition(
                    symbols.Get<Refs>(c => c.Method(ref x)));
            })
            .Should().NotThrow();

        [TestMethod] public void Refs_InParam() => Invoking(() => {
                var x = '\0';
                _ = testAssembly.GetMethodDefinition(
                    symbols.Get<Refs>(c => c.Method(in x)));
            })
            .Should().NotThrow();

        [TestMethod] public void Refs_OutParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Refs>(c => c.Method(out var x))))
            .Should().NotThrow();

        [TestMethod] public void Refs_InParamOverload1() => Invoking(() => {
                var s = "foo";
                _ = testAssembly.GetMethodDefinition(
                        symbols.Get<Refs>(c => c.MethodWithInParamOverload(in s)));
            })
            .Should().NotThrow();

        [TestMethod] public void Refs_InParamOverload2() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Refs>(c => c.MethodWithInParamOverload("foo"))))
            .Should().NotThrow();

        #endregion Refs

        #region Pointers

        [TestMethod] public unsafe void Pointers_PtrParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Pointers>(c => c.Method((int*)null))))
            .Should().NotThrow();

        [TestMethod] public unsafe void Pointers_PtrToPtrParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Pointers>(c => c.Method((int**)null))))
            .Should().NotThrow();

        [TestMethod] public unsafe void Pointers_PtrArrayParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Pointers>(c => c.Method(new int*[0]))))
            .Should().NotThrow();

        [TestMethod] public unsafe void Pointers_VoidPtrParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Pointers>(c => c.Method((void*)null))))
            .Should().NotThrow();

        #endregion Pointers

        #region Dynamic

        [TestMethod] public void Dynamic_IntParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Dynamic>(c => c.Method(0))))
            .Should().NotThrow();

        [TestMethod] public void Dynamic_DynamicParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Dynamic>(c => c.Method(.0))))
            .Should().NotThrow();

        #endregion Dynamic

        #region Delegate

        [TestMethod] public void Delegate_FuncParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Delegates>(c => c.Method((Func<int, string>)null!))))
            .Should().NotThrow();

        [TestMethod] public void Delegate_DelegateParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Delegates>(c => c.Method((Delegates.Qux)null!))))
            .Should().NotThrow();

        #endregion Delegate

        #region Alias

        [TestMethod] public void Alias_IntParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Alias>(c => c.Method(0))))
            .Should().NotThrow();

        [TestMethod] public void Alias_AliasedParam() => Invoking(() =>
            testAssembly.GetMethodDefinition(
                symbols.Get<Alias>(c => c.Method(""))))
            .Should().NotThrow();

        #endregion Alias
    }
}
