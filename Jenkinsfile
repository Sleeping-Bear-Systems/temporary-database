pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:9.0'
        }
    }
    environment {
        HOME = '/tmp'
        BASE_VERSION = '1.0.1'
        VERSION_SUFFIX = ''
        PREVIEW_SUFFIX = "${env.BRANCH_NAME == 'main' ? '' : '-preview.'}"
        BUILD_SUFFIX = "${env.BRANCH_NAME == 'main' ? '' : env.BUILD_NUMBER}"
        VERSION = "${BASE_VERSION}${PREVIEW_SUFFIX}${BUILD_SUFFIX}"
        NEXUS = credentials('nexus')
        NUGET_API_KEY = credentials('nuget-api-key')
        NEXUS_API_KEY = credentials('nexus-api-key')
        SBS_TEST_SERVER_POSTGRES = credentials('sbs-test-server-postgres')
        SBS_TEST_SERVER_MSSQL = credentials('sbs-test-server-mssql')
        SBS_TEST_SERVER_MYSQL = credentials('sbs-test-server-mysql')
    }
    stages {
        stage('Build & Test') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet build --no-restore /p:ContinuousIntegrationBuild=true -c Release /p:Version=$VERSION'
                sh 'dotnet test --no-build --no-restore -c Release --verbosity normal'
            }
        }
        stage('Publish Nexus') {
            when {
                not {
                    branch 'main'
                }
            }
            steps {
                sh 'dotnet nuget push **/*.nupkg --source NexusHosted --api-key $NEXUS_API_KEY --skip-duplicate'
            }
        }
        stage('Publish nuget.org') {
            when {
                branch 'main'
            }
            steps {
                sh 'dotnet nuget push **/*.nupkg --source https://api.nuget.org/v3/index.json --api-key $NUGET_API_KEY --skip-duplicate'
            }
        }
    }
    post {
        always {
            cleanWs()
        }
    }
}
