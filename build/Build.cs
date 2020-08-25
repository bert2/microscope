using System;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.Logger;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.VSTest.VSTestTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild {
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default: 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Version to publish - Required for target `Publish`, format: \"{major}.{minor}.{patch}\" (semver), default: `null`")]
    readonly string PublishVersion;

    [Parameter("Personal access token to the VS Marketplace - Required for target `Publish`, default: `null`")]
    readonly string MarketplaceKey;

    [Solution] readonly Solution Solution;

    [GitRepository] readonly GitRepository GitRepository;

    [PackageExecutable("Microsoft.VSSDK.BuildTools", "VsixPublisher.exe")]
    readonly Tool VsixPublisher;

    AbsolutePath SourceDirectory => RootDirectory / "src";

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() => {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() => {
            MSBuild(s => s
                .SetMSBuildPlatform(MSBuildPlatform.x86)
                .SetTargetPath(Solution)
                .SetTargets("Restore")
                .SetVerbosity(MSBuildVerbosity.Minimal));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Produces(ArtifactsDirectory / "*.vsix")
        .Executes(() => {
            MSBuild(s => s
                .SetMSBuildPlatform(MSBuildPlatform.x86)
                .SetTargetPath(Solution)
                .SetTargets("Rebuild")
                .SetVerbosity(MSBuildVerbosity.Minimal)
                .SetConfiguration(Configuration)
                .SetMaxCpuCount(Environment.ProcessorCount)
                .SetNodeReuse(IsLocalBuild)
                .SetProperty("DeployExtension", false)
                .SetProperty("ZipPackageCompressionLevel", "normal"));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() => {
            var p = Solution.GetProject("Tests");
            VSTest(p.Directory / p.GetProperty("OutputPath") / "Microscope.Tests.dll");
        });

    Target Publish => _ => _
        .Requires(
            () => PublishVersion != null,
            () => MarketplaceKey != null,
            () => GitRepository.Branch == "main")
        .Executes(() => {
            Info($"Publishing {PublishVersion} to Visual Studio Marketplace...");
            VsixPublisher("version");
        });
}
