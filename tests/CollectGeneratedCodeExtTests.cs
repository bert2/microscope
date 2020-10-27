#nullable enable

namespace Microscope.Tests {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Microscope.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;
    using Mono.Cecil.Rocks;

    using Shouldly;

    [TestClass]
    public class CollectGeneratedCodeExtTests {
        private static readonly Dictionary<string, MethodDefinition> testDataMethods = AssemblyDefinition
            .ReadAssembly(typeof(CollectGeneratedCodeExtTestData).Assembly.Location)
            .MainModule
            .GetType(typeof(CollectGeneratedCodeExtTestData).FullName)
            .GetMethods()
            .ToDictionary(m => m.Name);

        [TestMethod] public void FindsClassGeneratedForLambda() =>
            Method(x => x.Lambda())
            .CollectGeneratedCode()
            .ShouldHaveSingleItem();

        [TestMethod] public void FindsClassesGeneratedForNestedLambda() =>
            Method(x => x.NestedLambda())
            .CollectGeneratedCode()
            .Count.ShouldBe(2);

        [TestMethod] public void IgnoresMethodsGeneratedForNeighboringLambdas() =>
            Method(x => x.Lambda())
            .CollectGeneratedCode()
            .ShouldHaveSingleItem()
            .Methods.Count.ShouldBe(3);

        [TestMethod] public void FindsClassGeneratedForIterator() =>
            Method(x => x.Iterator())
            .CollectGeneratedCode()
            .ShouldHaveSingleItem();

        [TestMethod] public void FindsClassGeneratedForAsyncMethod() =>
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Method(x => x.AsyncMethod())
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            .CollectGeneratedCode()
            .ShouldHaveSingleItem();

        [TestMethod] public void FindsClassGeneratedForAnonymousType() =>
            Method(x => x.AnonymousType())
            .CollectGeneratedCode()
            .ShouldHaveSingleItem();

        [TestMethod, Ignore("currently not supported")] public void FindsMethodGeneratedForLocalFunction() =>
            Method(x => x.LocalFunction())
            .CollectGeneratedCode()
            .ShouldHaveSingleItem();

        [TestMethod] public void RemovesDuplicateReferencesToGeneratedClasses() =>
            Method(x => x.Lambda())
            .CollectGeneratedCode()
            .Select(t => t.Name)
            .ShouldBeUnique();

        private static MethodDefinition Method(Expression<Action<CollectGeneratedCodeExtTestData>> selector)
            => testDataMethods[((MethodCallExpression)selector.Body).Method.Name];
    }
}
