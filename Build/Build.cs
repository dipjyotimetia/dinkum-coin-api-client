using CrownBet.Build;
using Nuke.Common.Tools.DotNet;
using Nuke.Core;
using System.IO;
using  Nuke.Common.Tools.Nunit;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Core.IO.PathConstruction;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;


namespace Build
{
    public partial class Build : NukeBuild
    {
        [Parameter("The file to write the build version to (" + nameof(Export_Build_Version) + " target only)")] public string BuildVersionFilePath;
        public Target Export_Build_Version => _ => _
            .Description("Outputs the build version to a file")
            .Requires(() => BuildVersionFilePath)
            .Executes(() => File.WriteAllText(BuildVersionFilePath, GetBuildVersion()));



        public Target Compile => _ => _
            .DependsOn(Export_Build_Version)
            .Executes(() =>
                DotNetBuild(
                    RootDirectory,
                    settings => settings
                    .SetConfiguration("Release")
                        .SetFileVersion(GetSemanticBuildVersion())
                        .SetInformationalVersion(GetBuildVersion())
                        .AddProperty("Version", GetBuildVersion())));

        public Target UnitTest => _ => _
            .DependsOn(Compile)
            .Executes(() => 
             DotNetTest(settings => settings
                            .SetProjectFile(RootDirectory / "CrownBet.QA.API.Core.Tests")
							.SetConfiguration("Release")
                            .SetLogger("xunit;LogFilePath=TestResults.xml")
                         .SetNoBuild(true))
            );



        public Target Package => _ => _
            .DependsOn(Compile)
            .Executes(() => {


                DotNetPack(settings => settings.SetVersion(GetBuildVersion())
                                                .SetConfiguration("Release")
                                                .SetNoBuild(true)
                                                .SetOutputDirectory(LibrarySourceDirectory / "bin/Release")
                                                .SetProject(LibrarySourceDirectory)
                                                .SetAuthors("CROWNBET QA")
                                                .SetDescription("Common code for Restful API testing")
                                                .SetPackageTags("QA")
                                                .SetCopyright("Copyright 2017")
                                                .SetPackageId("CrownBet.QA.API.Core")
                );
            });


        public Target Publish => _ => _
            .DependsOn(Package)
            .Executes(() => {

                DotNetNuGetPush(settings => settings
                    .SetTargetPath(LibrarySourceDirectory / "bin/Release" / (LibraryName + $".{GetBuildVersion()}.nupkg"))
                    .SetSource("http://wgtawsnuget01.wgtech.local/nuget/"));
                    });

        private string LibraryName => Path.GetFileNameWithoutExtension(SolutionFile);

        private AbsolutePath LibrarySourceDirectory => RootDirectory / LibraryName;

        private GlobalSettings Settings =>  new GlobalSettings();

        public static int Main() => Execute<Build>(x => x.Compile);

        private string GetBuildVersion()
        {
            string branch = Git.GetBranchName(RootDirectory);

            branch = branch == "master" ? "" : "-" + branch.Replace("/", "-");

            return GetSemanticBuildVersion() + branch;
        }

        private string GetSemanticBuildVersion()
        {
            return $"1.0.{Git.GetCommitCount(RootDirectory)}";
        }
    }
}