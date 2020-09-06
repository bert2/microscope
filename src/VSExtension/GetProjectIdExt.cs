#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;

    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.LanguageServices;

    public static class GetProjectIdExt {
        private static readonly FieldInfo projectToGuidMap = typeof(VisualStudioWorkspace).Assembly
            .GetType(
                "Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.VisualStudioWorkspaceImpl",
                throwOnError: true)
            .GetField("_projectToGuidMap", BindingFlags.NonPublic | BindingFlags.Instance);

        private static T GetValue<T>(this FieldInfo field, object obj) => (T)field.GetValue(obj);

        // TODO: this sometimes fails, because some solutions have multiple `ProjectId`s pointing at the same `Guid`.
        public static ProjectId? GetProjectId(this VisualStudioWorkspace workspace, Guid projectGuid)
            => projectToGuidMap
                .GetValue<ImmutableDictionary<ProjectId, Guid>>(workspace)
                .SingleOrDefault(kvp => kvp.Value == projectGuid)
                .Key;

        public static Guid? GetProjectGuid(this VisualStudioWorkspace workspace, ProjectId projectId)
            => projectToGuidMap
                .GetValue<ImmutableDictionary<ProjectId, Guid>>(workspace)
                .GetValueOrDefault(projectId, Guid.Empty);
    }
}
