#tool Microsoft.VSSDK.BuildTools&version=16.7.3069

var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");

Task("Clean")
    .WithCriteria(_ => HasArgument("rebuild"))
    .Does(() =>
{
    Information("Cleaning ...");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    Information("Building...");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("Testing...");
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("Publishing...");
    
    var vsixPublisher = Context.Tools.Resolve("VsixPublisher.exe");
    StartProcess(vsixPublisher, new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("version")
        });
});

RunTarget(target);
