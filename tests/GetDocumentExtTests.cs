#nullable enable

namespace Microscope.Tests {
    using System;
    using System.Collections.Immutable;
    using System.Reflection;

    using FluentAssertions;

    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GetDocumentExtTests {
        [TestMethod]
        public void VisualStudioWorkspaceImplTypeExists() => typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl")
            .Should().NotBeNull();

        [TestMethod]
        public void ProjectToGuidMapFieldExists() => typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl",
                throwOnError: true)
            .GetField("_projectToGuidMap", BindingFlags.NonPublic | BindingFlags.Instance)
            .Should().NotBeNull();

        [TestMethod]
        public void ProjectToGuidMapFieldIsImmutableDictionary() => typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl",
                throwOnError: true)
            .GetField("_projectToGuidMap", BindingFlags.NonPublic | BindingFlags.Instance)
            .FieldType
            .Should().Be(typeof(ImmutableDictionary<ProjectId, Guid>));

        [TestMethod]
        public void GetDocumentIdInCurrentContextMethodExists() => typeof(Workspace)
            .GetMethod(
                "GetDocumentIdInCurrentContext",
                BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                types: new[] { typeof(DocumentId) },
                modifiers: null)
            .Should().NotBeNull();
    }
}
