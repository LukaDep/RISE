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
        stage('Restore & Build') {
            steps {
                sh '''
                    echo "=== Restoring dependencies ==="
                    dotnet restore

                    echo "=== Building project ==="
                    dotnet build --configuration Release --no-restore
                '''
            }
        }

        stage('Publish') {
            steps {
                sh '''
                    echo "=== Publishing application ==="
                    dotnet publish src/Rise.Server/Rise.Server.csproj -c Release -o ./publish --no-build
                '''
            }
        }

        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh '''
                        echo "=== Cleaning old deployment on appserver ==="
                        ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                            sudo pkill -f 'dotnet Rise.Server.dll' || true;
                            sudo rm -rf ${DEPLOY_PATH}/*;
                            sudo mkdir -p ${DEPLOY_PATH};
                            sudo chown vagrant:vagrant ${DEPLOY_PATH};
                        "

                        echo "=== Copying new build files ==="
                        rsync -av --exclude '*Tests.*' --exclude '*.pdb' \
                          -e "ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no" \
                          ./publish/ vagrant@${APP_SERVER}:${DEPLOY_PATH}/

                        echo "=== Starting application on appserver ==="
                        ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                            nohup bash -c 'ASPNETCORE_URLS=http://0.0.0.0:5000 dotnet ${DEPLOY_PATH}/Rise.Server.dll > ${DEPLOY_PATH}/app.log 2>&1 &' && sleep 5
                        "

                        echo "=== Checking if application is reachable ==="
                        curl -f http://${APP_SERVER}:5000/ && echo 'Application is running!' || echo 'Application failed to start!'
                    '''
                }
            }
        }
    }

    post {
        success {
            echo 'Build & Deployment succeeded!'
        }
        failure {
            echo 'Build or Deployment failed!'
        }
    }
}
