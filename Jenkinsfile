def nugetAccessKey = ''

pipeline {
	agent {label 'dotnetcore'}

    options { 
		skipDefaultCheckout() 
		buildDiscarder(logRotator(numToKeepStr: '10', artifactNumToKeepStr: '10'))
	}
	environment {
		DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
		DOTNET_CLI_TELEMETRY_OPTOUT = "1"	
	}
	stages {
		stage("Build") {
			steps {
				deleteDir()
                checkout scm

				buildTarget "Compile", "-NoDeps"
				stash name: "solution", useDefaultExcludes: false
			}
		}
		stage("Unit test")  {
		steps {
				deleteDir()
				unstash "solution"
				buildTarget "UnitTest", "-NoDeps"
				stash name: "solution", useDefaultExcludes: false
 			}
		}
		// stage("Code Coverage")  {
		// steps {
		// 		deleteDir()
		// 		unstash "solution"

		// 		buildTarget "UnitTest", "-NoDeps"
				
		// 		stash name: "solution", useDefaultExcludes: false
 		// 	}
		// }		
		stage("Publish NuGet package") {
			when { branch "master" }
			steps {
				deleteDir()
				unstash "solution"
				buildTarget "Package", "-NoDeps"

                script{
                    nugetAccessKey = credentials('Nuget_Access_Key')
                    sh "echo ${nugetAccessKey}"
                }

				buildTarget "Publish", "-NoDeps -NugetKey \"${nugetAccessKey}\""
			}
		}
	}
	post {
		always {
			deleteDir()
			unstash "solution" 
			step([$class: 'XUnitBuilder',
				thresholds: [[$class: 'FailedThreshold', unstableThreshold: '1']],
				tools: [[ $class: 'XUnitDotNetTestType', pattern: '**/TestResults.xml']]]
			)
		}	
	}
}
void buildTarget(String targetName, String parameters = "") {
		sh "dotnet run -p Build -Target ${targetName} ${parameters}"
}