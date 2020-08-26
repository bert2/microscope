using System;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Logger;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.IO.TextTasks;
using static Nuke.Common.IO.XmlTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.VSTest.VSTestTasks;
using System.Linq;
using Nuke.Common.Utilities;
using System.Text;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild {
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Optional (default: 'Release')")]
    readonly Configuration Configuration = Configuration.Release;

    [Parameter("Version - Optional, set to latest tag if omitted (default: null)")]
    readonly string SemVer;

    [Parameter("Number identifying the build - Optional, set by the CI (default: 0)")]
    readonly int BuildNumber;

    [Parameter("Update assembly properties and VSIX manifest with current version during build - Optional (default: false)")]
    readonly bool UpdateVersion;

    [Parameter("Personal access token for the VS Marketplace - Required for target `Publish` (default: null)")]
    readonly string MarketplaceKey;

    [Solution] readonly Solution Solution;

    [GitRepository] readonly GitRepository GitRepository;

    [PathExecutable] readonly Tool Git;

    [PackageExecutable("Microsoft.VSSDK.BuildTools", "VsixPublisher.exe")]
    readonly Tool VsixPublisher;

    private string version;
    string Version {
        get {
            version ??= $"{SemVer ?? LastGitTag()}.{BuildNumber}";
            return version;
        }
    }

    AbsolutePath SrcDir => RootDirectory / "src";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() => {
            SrcDir.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .Executes(() => {
            MSBuild(s => s
                .SetMSBuildPlatform(MSBuildPlatform.x86)
                .SetTargetPath(Solution)
                .SetTargets("Restore")
                .SetVerbosity(MSBuildVerbosity.Minimal));
        });

    Target SetVersion => _ => _
        .OnlyWhenStatic(() => UpdateVersion)
        .Executes(() => {
            Info($"Current version is {Version}.");

            Info("Updating assembly properties...");

            SrcDir.GlobFiles("**/AssemblyInfo.cs").ForEach(file =>
                ReplaceText(
                    file,
                    @"Version\(""\d+\.\d+\.\d+\.\d+""\)",
                    $@"Version(""{Version}"")"));

            Info("Updating VSIX manifest...");

            ReplaceText(
                SrcDir / "VSExtension" / "source.extension.cs",
                @"Version = ""\d+\.\d+\.\d+\.\d+""",
                $@"Version = ""{Version}""");

            XmlPoke(
                SrcDir / "VSExtension" / "source.extension.vsixmanifest",
                "/ns:PackageManifest/ns:Metadata/ns:Identity/@Version",
                Version,
                ("ns", "http://schemas.microsoft.com/developer/vsx-schema/2011"));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(SetVersion)
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
            var outDir = p.GetMSBuildProject(Configuration).GetPropertyValue("OutputPath");
            VSTest(p.Directory / outDir / "Microscope.Tests.dll");
        });

    Target Publish => _ => _
        .Requires(
            () => MarketplaceKey != null,
            () => GitRepository.Branch == "main")
        .Executes(() => {
            Info($"Publishing {Version} to Visual Studio Marketplace...");
            VsixPublisher("version");
        });

    private string LastGitTag() => Git("describe --tags --abbrev=0").Single().Text;

    private static void ReplaceText(AbsolutePath file, string pattern, string replacement) {
        var text = ReadAllText(file).ReplaceRegex(pattern, _ => replacement);
        WriteAllText(file, text, Encoding.UTF8);
    }
}
