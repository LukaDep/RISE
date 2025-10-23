pipeline {
    agent any
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        APP_SERVER = '192.168.56.50'
        DEPLOY_PATH = '/var/www/dotnetapp'
        APP_NAME = 'dotnetapp.service'
    }
    stages {
        stage('Build') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release --no-restore'
            }
        }
        stage('Publish') {
            steps {
                sh 'dotnet publish src/Rise.Server/Rise.Server.csproj -c Release -o ./publish --no-build'
                
                // Verwijder test assemblies na publish
                sh '''
                cd ./publish
                # Verwijder test-related files
                rm -f *Tests.*
                rm -f Serilog.Sinks.XUnit.*
                rm -f xunit.*
                rm -f coverlet.*
                rm -f NSubstitute.*
                rm -f Shouldly.*
                rm -rf CodeCoverage/
                '''
            }
        }
        stage('Deploy') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh '''
                    # Kopieer files
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "sudo mkdir -p ${DEPLOY_PATH} && sudo chown vagrant:vagrant ${DEPLOY_PATH}"
                    rsync -av -e "ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no" ./publish/ vagrant@${APP_SERVER}:${DEPLOY_PATH}/
                    
                    # Start de app
                    echo "=== Starting application ==="
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                        cd ${DEPLOY_PATH}
                        dotnet Rise.Server.dll
                    " &
                    
                    # Wacht even en test
                    sleep 10
                    curl -f http://${APP_SERVER}:5000/ && echo "Application is running!" || echo "Application failed to start"
                    '''
                }
            }
        }
    }
}
