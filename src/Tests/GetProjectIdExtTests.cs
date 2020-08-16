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
        public void VisualStudioWorkspaceImplExists() => typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl")
            .ShouldNotBeNull();

        [TestMethod]
        public void ProjectToGuidMapExists() => typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl",
                throwOnError: true)
            .GetField("_projectToGuidMap", BindingFlags.NonPublic | BindingFlags.Instance)
            .ShouldNotBeNull();

        [TestMethod]
        public void ProjectToGuidMapIsImmutableDictionary() => typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl",
                throwOnError: true)
            .GetField("_projectToGuidMap", BindingFlags.NonPublic | BindingFlags.Instance)
            .FieldType
            .ShouldBe(typeof(ImmutableDictionary<ProjectId, Guid>));
    }
}
