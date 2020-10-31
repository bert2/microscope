#nullable enable

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace Microscope.Tests {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using FluentAssertions;

    using Microscope.CodeAnalysis;
    using Microscope.Tests.Util;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;
    using Mono.Cecil.Rocks;

    [TestClass]
    public class CollectGeneratedCodeExtTests {
        private static readonly Dictionary<string, MethodDefinition> testDataMethods = AssemblyDefinition
            .ReadAssembly(typeof(CollectGeneratedCodeExtTestData).Assembly.Location)
            .MainModule
            .GetType(typeof(CollectGeneratedCodeExtTestData).FullName)
            .GetMethods()
            .ToDictionary(m => m.Name);

        private readonly HashSet<MethodDefinition> empty = new HashSet<MethodDefinition>();

        [TestMethod] public void FindsMethodGeneratedForLambda() =>
            Method(x => x.Lambda())
            .CollectGeneratedMethods(visited: empty)
            .Should().ContainSingle().Which
                .Should().Satisfy(m => m.Name.Should().Be("<Lambda>b__0_0"))
                .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<>c"));

        [TestMethod] public void FindsMethodsGeneratedForClosure() =>
            Method(x => x.Closure(0))
            .CollectGeneratedMethods(visited: empty)
            .Should().SatisfyRespectively(
                ctor => ctor
                    .Should().Satisfy(m => m.Name.Should().Be(".ctor"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<>c__DisplayClass1_0")),
                closure => closure
                    .Should().Satisfy(m => m.Name.Should().Be("<Closure>b__0"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<>c__DisplayClass1_0")));

        [TestMethod] public void FindsMethodsGeneratedForClosureNestedInLambda() =>
            Method(x => x.ClosureNestedInLambda())
            .CollectGeneratedMethods(visited: empty)
            .Should().SatisfyRespectively(
                lambda => lambda
                    .Should().Satisfy(m => m.Name.Should().Be("<ClosureNestedInLambda>b__2_0"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<>c")),
                ctor => ctor
                    .Should().Satisfy(m => m.Name.Should().Be(".ctor"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<>c__DisplayClass2_0")),
                closure => closure
                    .Should().Satisfy(m => m.Name.Should().Be("<ClosureNestedInLambda>b__1"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<>c__DisplayClass2_0")));

        [TestMethod] public void FindsMethodGeneratedForGenericLambda() =>
            Method(x => x.GenericLambda<int>())
            .CollectGeneratedMethods(visited: empty)
            .Should().ContainSingle().Which
                .Should().Satisfy(m => m.Name.Should().Be("<GenericLambda>b__4_0"))
                .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<>c__4`1"));

        [TestMethod] public void FindsMethodGeneratedForLocalFunction() =>
            Method(x => x.LocalFunction())
            .CollectGeneratedMethods(visited: empty)
            .Should().ContainSingle().Which
                .Should().Satisfy(m => m.Name.Should().Be("<LocalFunction>g__Foo|8_0"))
                .And.Satisfy(m => m.DeclaringType.Name.Should().Be("CollectGeneratedCodeExtTestData"));

        [TestMethod] public void FindsMethodsGeneratedForIterator() =>
            Method(x => x.Iterator())
            .CollectGeneratedMethods(visited: empty)
            .Should().SatisfyRespectively(
                ctor => ctor
                    .Should().Satisfy(m => m.Name.Should().Be(".ctor"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<Iterator>d__5")),
                moveNext => moveNext
                    .Should().Satisfy(m => m.Name.Should().Be("MoveNext"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<Iterator>d__5")));

        [TestMethod] public void FindsMethodsGeneratedForAsyncMethod() =>
            Method(x => x.AsyncMethod())
            .CollectGeneratedMethods(visited: empty)
            .Should().SatisfyRespectively(
#if DEBUG // ctor call is optimized away in release builds
                ctor => ctor
                    .Should().Satisfy(m => m.Name.Should().Be(".ctor"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<AsyncMethod>d__6")),
#endif
                moveNext => moveNext
                    .Should().Satisfy(m => m.Name.Should().Be("MoveNext"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<AsyncMethod>d__6")));

        [TestMethod] public void FindsMethodGeneratedForLambdaInAsyncMethod() =>
            Method(x => x.AsyncMethodWithLambda())
            .CollectGeneratedMethods(visited: empty)
            .Should().SatisfyRespectively(
#if DEBUG // ctor call is optimized away in release builds
                ctor => ctor
                    .Should().Satisfy(m => m.Name.Should().Be(".ctor"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<AsyncMethodWithLambda>d__7")),
#endif
                moveNext => moveNext
                    .Should().Satisfy(m => m.Name.Should().Be("MoveNext"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<AsyncMethodWithLambda>d__7")),
                lambda => lambda
                    .Should().Satisfy(m => m.Name.Should().Be("<AsyncMethodWithLambda>b__7_0"))
                    .And.Satisfy(m => m.DeclaringType.Name.Should().Be("<>c")));

        [TestMethod] public void RemovesDuplicates() =>
            Method(x => x.LocalFunctionCalledTwice())
            .CollectGeneratedMethods(visited: empty)
            .Should().OnlyHaveUniqueItems();

        [TestMethod] public void IgnoresAutoProperties() =>
            Method(x => x.AutoProperty())
            .CollectGeneratedMethods(visited: empty)
            .Should().BeEmpty();

        private static MethodDefinition Method(Expression<Action<CollectGeneratedCodeExtTestData>> selector)
            => testDataMethods[((MethodCallExpression)selector.Body).Method.Name];
    }
}
