#tool Microsoft.VSSDK.BuildTools&version=16.7.3069

var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");
var publishVersion = Argument<string>("publishVersion", null);

Task("Clean")
    .WithCriteria(_ => HasArgument("rebuild"))
    .Does(() => {
    Information("Cleaning ...");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() => {
    Information("Building...");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
    Information("Testing...");
});

Task("PublishToOpenGallery")
    .IsDependentOn("Build")
    .Does(() => {
    if (publishVersion is null)
        throw new InvalidOperationException("Target `Publish` requires `publishVersion` argument: `PS> .\\build.ps1 -Target Publish -ScriptArgs \"--publishVersion=0.0.0\"`");
    Information($"Publishing {publishVersion} to Open VSIX Gallery...");
});

Task("PublishToMarketplace")
    .Does(() => {
    if (publishVersion is null)
        throw new InvalidOperationException("Target `Publish` requires `publishVersion` argument: `PS> .\\build.ps1 -Target Publish -ScriptArgs \"--publishVersion=0.0.0\"`");
    Information($"Publishing {publishVersion} to Visual Studio Marketplace...");

    var vsixPublisher = Context.Tools.Resolve("VsixPublisher.exe");
    StartProcess(vsixPublisher, new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("version")
        });
});

Task("Publish")
    .IsDependentOn("PublishToOpenGallery")
    .IsDependentOn("PublishToMarketplace");

RunTarget(target);
