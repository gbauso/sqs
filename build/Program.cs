using System;
using System.Diagnostics.CodeAnalysis;
using Cake.Frosting;
using dotenv.net;
using Overleaf.Cake.Frosting;
using Overleaf.Cake.Frosting.Tasks;
using Overleaf.Cake.Frosting.Tasks.Lambda;

namespace Build;

public static class Program
{
    public static int Main(string[] args)
    {
        DotEnv.Fluent()
            .WithEnvFiles("../.env")
            .Load();

        return new CakeHost()
            .AddAssembly(typeof(CleanTask).Assembly)
            .UseWorkingDirectory("..")
            .InstallTools()
            .UseContext<BuildContext>()
            .Run(args);
    }

    public static CakeHost InstallTools(this CakeHost host)
        {
            host.SetToolPath($"./caketools");  
            host.InstallTool(new Uri("nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.8.0"));
            host.InstallTool(new Uri("nuget:?package=NuGet.CommandLine&version=5.8.1")); 
            host.InstallTool(new Uri("nuget:?package=JetBrains.dotCover.GlobalTool&version=2021.3.3"));
            host.InstallTool(new Uri("dotnet:?package=GitVersion.Tool&version=5.10.1"));
            host.InstallTool(new Uri($"nuget:https://www.myget.org/F/parknow-dotnet/auth/{System.Environment.GetEnvironmentVariable("MyGetApiKey")}/api/v2?package=ParameterStoreUploader&version=3.0.1"));
            return host;
        }


    [ExcludeFromCodeCoverage]
    [IsDependentOn(typeof(CleanTask))]
    [IsDependentOn(typeof(GitVersionTask))]
    [IsDependentOn(typeof(SonarInitTask))]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(TestAndCoverTask))]
    [IsDependentOn(typeof(SonarEndTask))]
    [IsDependentOn(typeof(PublishLambdaTask))]
    [IsDependentOn(typeof(PackageTask))]
    [IsDependentOn(typeof(AwsCodeArtifactsPusherTask))]
    [IsDependentOn(typeof(PackagePushTask))]
    [IsDependentOn(typeof(VersionFinalizeTask))]
    public sealed class Default : FrostingTask
    {
    }

    [ExcludeFromCodeCoverage]
    [IsDependentOn(typeof(EnvironmentVariablesTask))]
    [IsDependentOn(typeof(ParameterStoreUploadLambdaTask))]
    [IsDependentOn(typeof(ServerlessDeployLambdaTask))]
    public sealed class OctopusDeploy : FrostingTask
    {
    }

    [ExcludeFromCodeCoverage]
    [IsDependentOn(typeof(CleanTask))]
    [IsDependentOn(typeof(GitVersionTask))]
    [IsDependentOn(typeof(EnvironmentVariablesTask))]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(TestAndCoverTask))]
    [IsDependentOn(typeof(PublishLambdaTask))]
    [IsDependentOn(typeof(PackageTask))]
    [IsDependentOn(typeof(ParameterStoreUploadLambdaTask))]
    [IsDependentOn(typeof(ServerlessDeployLambdaTask))]
    [IsDependentOn(typeof(VersionFinalizeTask))]
    public sealed class LocalDeploy : FrostingTask
    {
    }
}


