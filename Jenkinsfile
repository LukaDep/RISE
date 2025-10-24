pipeline {
    agent any

    environment {
        APP_NAME    = "Rise.Server"
        APP_SERVER  = "192.168.56.50"
        DEPLOY_PATH = "/var/www/dotnetapp"
        SSH_KEY     = "/var/lib/jenkins/.ssh/appserver_key"
    }

    stages {

        stage('Restore & Build') {
            steps {
                echo "=== Restoring dependencies ==="
                sh 'dotnet restore'

                echo "=== Building project ==="
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish') {
            steps {
                echo "=== Publishing application ==="
                sh 'dotnet publish src/Rise.Server/Rise.Server.csproj -c Release -o ./publish --no-build'
            }
        }

        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver_ssh', keyFileVariable: 'SSH_KEY')]) {

                    echo "=== Testing SSH connection (debug mode) ==="
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "echo SSH OK"
                    '''

                    echo "=== Cleaning old deployment on appserver ==="
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            (sudo pkill -f 'dotnet Rise.Server.dll' || true) && \
                            sudo rm -rf ${DEPLOY_PATH}/* || true && \
                            sudo mkdir -p ${DEPLOY_PATH} && \
                            sudo chown vagrant:vagrant ${DEPLOY_PATH} || true
                        " || true
                    '''

                    echo "=== Copying new build to appserver ==="
                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" ./publish/ vagrant@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    echo "=== Starting application on appserver ==="
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            cd ${DEPLOY_PATH};
                            nohup dotnet Rise.Server.dll > app.log 2>&1 &
                        "
                    '''
                }
            }
        }
    }

    post {
        success {
            echo "✅ Build and deployment successful!"
        }
        failure {
            echo "❌ Build or Deployment failed!"
        }
    }
}
