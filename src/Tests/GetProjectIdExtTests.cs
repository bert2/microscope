#nullable enable

namespace Tests {
    using System;
    using System.Collections.Immutable;
    using System.Reflection;

    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Shouldly;

    [TestClass]
    public class GetProjectIdExtTests {
        [TestMethod]
        public void VisualStudioWorkspaceImplTypeExists() => typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl")
            .ShouldNotBeNull();

        [TestMethod]
        public void ProjectToGuidMapFieldExists() => typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl",
                throwOnError: true)
            .GetField("_projectToGuidMap", BindingFlags.NonPublic | BindingFlags.Instance)
            .ShouldNotBeNull();

        [TestMethod]
        public void ProjectToGuidMapFieldIsImmutableDictionary() => typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl",
                throwOnError: true)
            .GetField("_projectToGuidMap", BindingFlags.NonPublic | BindingFlags.Instance)
            .FieldType
            .ShouldBe(typeof(ImmutableDictionary<ProjectId, Guid>));
    }
}
