#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;

    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.LanguageServices;

    public static class ProjectGuidToId {
        private static readonly FieldInfo projectToGuidMapField = typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl",
                throwOnError: true)
            .GetField("_projectToGuidMap", BindingFlags.NonPublic | BindingFlags.Instance);

        public static ProjectId? FindProject(this VisualStudioWorkspace workspace, Guid projectGuid)
            => projectToGuidMapField
                .GetValue<ImmutableDictionary<ProjectId, Guid>>(workspace)
                .SingleOrDefault(kvp => kvp.Value == projectGuid)
                .Key;

        public static T GetValue<T>(this FieldInfo field, object obj) => (T)field.GetValue(obj);
    }
}
