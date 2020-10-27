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
    public class CollectCompilerGeneratedInstructionsExtTests {
        private static readonly Dictionary<string, MethodDefinition> methods = AssemblyDefinition
            .ReadAssembly(typeof(CollectCompilerGeneratedInstructionsExtTestData).Assembly.Location)
            .MainModule
            .GetType(typeof(CollectCompilerGeneratedInstructionsExtTestData).FullName)
            .GetMethods()
            .ToDictionary(m => m.Name);

        [TestMethod] public void FindsClassGeneratedForLambda() =>
            Method(x => x.Lambda())
            .CollectCompilerGeneratedInstructions()
            .ShouldHaveSingleItem();

        [TestMethod] public void FindsClassGeneratedForNestedLambda() =>
            Method(x => x.NestedLambda())
            .CollectCompilerGeneratedInstructions()
            .Count.ShouldBe(2);

        [TestMethod] public void FindsClassGeneratedForIterator() =>
            Method(x => x.Iterator())
            .CollectCompilerGeneratedInstructions()
            .ShouldHaveSingleItem();

        [TestMethod] public void FindsClassGeneratedForAsyncMethod() =>
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Method(x => x.AsyncMethod())
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            .CollectCompilerGeneratedInstructions()
            .ShouldHaveSingleItem();

        [TestMethod] public void FindsClassGeneratedForAnonymousType() =>
            Method(x => x.AnonymousType())
            .CollectCompilerGeneratedInstructions()
            .ShouldHaveSingleItem();

        [TestMethod, Ignore("currently not supported")] public void FindsMethodGeneratedForLocalFunction() =>
            Method(x => x.LocalFunction())
            .CollectCompilerGeneratedInstructions()
            .ShouldHaveSingleItem();

        [TestMethod] public void RemovesDuplicateReferencesToGeneratedClasses() =>
            Method(x => x.Lambda())
            .CollectCompilerGeneratedInstructions()
            .Select(t => t.Name)
            .ShouldBeUnique();

        private static MethodDefinition Method(
            Expression<Action<CollectCompilerGeneratedInstructionsExtTestData>> selector)
            => methods[((MethodCallExpression)selector.Body).Method.Name];
    }
}
