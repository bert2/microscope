#nullable enable

namespace Tests {
    using System.Reflection;

    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Shouldly;

    [TestClass]
    public class GetDocumentIdInCurrentContextExtTests {
        [TestMethod]
        public void GetDocumentIdInCurrentContextExists() => typeof(Workspace)
            .GetMethod(
                "GetDocumentIdInCurrentContext",
                BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                types: new[] { typeof(DocumentId) },
                modifiers: null)
            .ShouldNotBeNull();
    }
}
