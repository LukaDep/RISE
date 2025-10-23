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
        stage('Deploy') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh '''
                    # Stop service eerst
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "sudo systemctl stop ${APP_NAME} || true"
                    
                    # Kopieer files
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "sudo rm -rf ${DEPLOY_PATH} && sudo mkdir -p ${DEPLOY_PATH}"
                    scp -i ${SSH_KEY} -o StrictHostKeyChecking=no -r ./publish/* vagrant@${APP_SERVER}:${DEPLOY_PATH}/
                    
                    # Fix permissions en start service
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                        sudo chown -R vagrant:vagrant ${DEPLOY_PATH}
                        sudo chmod -R 755 ${DEPLOY_PATH}
                        sudo systemctl start ${APP_NAME}
                    "
                    '''
                }
            }
        }
        stage('Verify') {
            steps {
                sh '''
                sleep 3
                curl -f http://${APP_SERVER}:5000/ || echo "App is not responding yet"
                '''
            }
        }
    }
}
