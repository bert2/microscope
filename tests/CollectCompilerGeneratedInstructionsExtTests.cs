#nullable enable

namespace Microscope.Tests {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Microscope.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using Mono.Collections.Generic;

    using Shouldly;

    [TestClass]
    public class CollectCompilerGeneratedInstructionsExtTests {
        private static readonly Dictionary<string, Collection<Instruction>> methodInstructions = AssemblyDefinition
            .ReadAssembly(typeof(CollectCompilerGeneratedInstructionsExtTestData).Assembly.Location)
            .MainModule
            .GetType(typeof(CollectCompilerGeneratedInstructionsExtTestData).FullName)
            .GetMethods()
            .ToDictionary(m => m.Name, m => m.Body.Instructions);

        [TestMethod] public void FindsGeneratedLambdaClass() =>
            MethodInstructions(x => x.Lambda())
            .CollectCompilerGeneratedInstructions()
            .ShouldHaveSingleItem();

        [TestMethod] public void RemovesDuplicateReferencesToGeneratedClasses() =>
            MethodInstructions(x => x.Lambda())
            .CollectCompilerGeneratedInstructions()
            .Select(t => t.TypeName)
            .ShouldBeUnique();

        private static Collection<Instruction> MethodInstructions(
            Expression<Action<CollectCompilerGeneratedInstructionsExtTestData>> selector)
            => methodInstructions[((MethodCallExpression)selector.Body).Method.Name];
    }
}
