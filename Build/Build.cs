using Nuke.Core;
using System.IO;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Core.IO.PathConstruction;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Tools.DotNet;
using Nuke.Core.Tooling;

namespace Build
{
    public partial class Build : NukeBuild
    {
        [Parameter("Nuget access key", Name = "NugetKey")] public string NugetKey;


       private string OpenCoverPath = "C:/Tools/OpenCover/OpenCover.Console.exe";

        public Target Compile => _ => _
            .Executes(() =>
                DotNetBuild(
                    RootDirectory,
                    settings => settings
                    .SetConfiguration("Release")
                         ));
        

        public Target UnitTest => _ => _
            .DependsOn(Compile)
            .Executes(() => 
             DotNetTest(settings => settings
                        .SetProjectFile(RootDirectory / "DinkumCoin.Api.Client.Tests")
							.SetConfiguration("Release")
                            .SetLogger("xunit;LogFilePath=TestResults.xml")
                         .SetNoBuild(true))
            );


        public Target CodeCoverage => _ => _
            .Description("Perform all unit tests")
            .DependsOn(Compile)
            .Executes(() =>
            {
            string[] testProjects = { "DinkumCoin.Api.Client" };
                foreach (var testProject in testProjects)
                {
                    string opencoverParams = $"-register:user " +
                    $"-target:dotnet.exe " +
                    $"\"-targetdir:{ testProject}\" " +
                    $"\"-targetargs:test\" " +
                    $"\"-output:{RootDirectory}/OpenCover.coverageresults\" " +
                    $"-mergeoutput " +
                    $"-oldStyle " +
                    $"-excludebyattribute:System.CodeDom.Compiler.GeneratedCodeAttribute " +
                    $"\"-filter:+[DinkumCoin*]* -[*Tests]*\" ";

                    ProcessTasks.StartProcess(
                                 OpenCoverPath, opencoverParams, RootDirectory).AssertZeroExitCode();
                }

            });


        public Target Package => _ => _
            .DependsOn(Compile)
            .Executes(() => {


                DotNetPack(settings => settings.SetConfiguration("Release")
                                                .SetNoBuild(true)
                                                .SetOutputDirectory(LibrarySourceDirectory / "bin/Release")
                                                .SetProject(LibrarySourceDirectory)
                                                .SetAuthors("DinkumCoin QA")
                                                .SetDescription("DinkumCoin QA")
                                                .SetPackageTags("QA")
                                                .SetCopyright("Copyright 2017")
                                                .SetPackageId("DinkumCoin.Api.Client")
                );
            });


        public Target Publish => _ => _
            .DependsOn(Package)
            .Requires(() => NugetKey)
            .Executes(() => {

                DotNetNuGetPush(settings => settings
                                .SetTargetPath(LibrarySourceDirectory / "bin/Release" / (LibraryName + $".1.0.0.nupkg"))
                                    .SetSource("https://api.nuget.org/v3/index.json")
                                    .SetApiKey(NugetKey)
                               );
                    });

        private string LibraryName => Path.GetFileNameWithoutExtension(SolutionFile);

        private AbsolutePath LibrarySourceDirectory => RootDirectory / LibraryName;

        private GlobalSettings Settings =>  new GlobalSettings();

        public static int Main() => Execute<Build>(x => x.Compile);

    }
}