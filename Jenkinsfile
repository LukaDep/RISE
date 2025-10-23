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
            }
        }
        stage('Deploy and Debug') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh '''
                    # Kopieer files
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "sudo mkdir -p ${DEPLOY_PATH} && sudo chown vagrant:vagrant ${DEPLOY_PATH}"
                    rsync -av -e "ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no" ./publish/ vagrant@${APP_SERVER}:${DEPLOY_PATH}/
                    
                    # Start de app handmatig om de echte error te zien
                    echo "=== Starting app manually to see real error ==="
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                        cd ${DEPLOY_PATH}
                        echo 'Current directory:'
                        pwd
                        echo 'Files in directory:'
                        ls -la
                        echo 'Starting application...'
                        dotnet Rise.Server.dll
                    " || true
                    '''
                }
            }
        }
    }
    post {
        always {
            echo 'Pipeline completed - check console output for the actual error'
        }
    }
}
