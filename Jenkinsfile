
pipeline {
	agent{ dockerfile true}

    options { 
		//skipDefaultCheckout() 
		buildDiscarder(logRotator(numToKeepStr: '10', artifactNumToKeepStr: '10'))
	}
	environment {	
        NUGET_ACCESS_KEY = credentials('Nuget_Access_Key')
	}
	stages {
		stage("Build") {
			steps {
			//	deleteDir()
                checkout scm

				sh "ls -al"
				sh "echo Build"
				sh "ls -al Build"
				buildTarget "Compile", "-NoDeps"
			//	stash name: "solution", useDefaultExcludes: false
			}
		}
		stage("Unit test")  {
		steps {
			//	deleteDir()
			//	unstash "solution"
				buildTarget "UnitTest", "-NoDeps"
			//	stash name: "solution", useDefaultExcludes: false
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
			//	deleteDir()
			//	unstash "solution"
				buildTarget "Package", "-NoDeps"


             

				buildTarget "Publish", "-NoDeps -NugetKey \"${env.NUGET_ACCESS_KEY}\""
			}
		}
	}
	post {
		always {
		//	deleteDir()
		//	unstash "solution" 
			step([$class: 'XUnitBuilder',
				thresholds: [[$class: 'FailedThreshold', unstableThreshold: '1']],
				tools: [[ $class: 'XUnitDotNetTestType', pattern: '**/TestResults.xml']]]
			)
		}	
	}
}
void buildTarget(String targetName, String parameters = "") {
		sh "dotnet run -p \"Build/Build.csproj\" -Target \"${targetName}\" ${parameters}"
}