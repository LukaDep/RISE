pipeline {
    agent any

    environment {
        APP_NAME    = "Rise.Server"
        APP_SERVER  = "192.168.56.50"
        DEPLOY_PATH = "/var/www/dotnetapp"
    }

    stages {

        stage('Restore & Build') {
            steps {
                echo "=== Restore & Build ==="
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish Self-Contained (no trimming)') {
            steps {
                echo "=== Publishing (Linux-x64 Self-contained) ==="
                sh '''
                    dotnet publish src/Rise.Server/Rise.Server.csproj \
                        -c Release \
                        -o ./publish \
                        --self-contained true \
                        -r linux-x64 \
                        /p:PublishTrimmed=false \
                        /p:TrimUnusedDependencies=false \
                        /p:CopyLocalLockFileAssemblies=true
                '''
            }
        }

        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(
                    credentialsId: 'appserver-ssh',
                    keyFileVariable: 'SSH_KEY'
                )]) {

                    echo "--- Killing old app and cleaning folder ---"
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            sudo pkill -f 'Rise.Server.dll' || true;
                            sudo rm -rf ${DEPLOY_PATH};
                            sudo mkdir -p ${DEPLOY_PATH};
                            sudo chown -R vagrant:vagrant ${DEPLOY_PATH};
                        "
                    '''

                    echo "--- Deploy files via rsync ---"
                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" \
                        ./publish/ vagrant@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    echo "--- Start application ---"
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            cd ${DEPLOY_PATH};
                            nohup dotnet Rise.Server.dll > app.log 2>&1 &
                        "
                    '''
                }
            }
        }

        stage('Smoke Test') {
            steps {
                echo "--- Checking if API is reachable ---"
                sh '''
                    sleep 3
                    curl -f http://$APP_SERVER:5000/ || exit 1
                '''
                echo "✅ Smoke Test passed!"
            }
        }
    }

    post {
        success {
            echo "✅ Deployment success!"
        }
        failure {
            echo "❌ Deployment failed — logs incoming:"
            sh '''
                ssh -i /var/lib/jenkins/.ssh/appserver_key \
                -o StrictHostKeyChecking=no \
                vagrant@$APP_SERVER "tail -n 200 ${DEPLOY_PATH}/app.log || echo 'Geen logs'"
            '''
        }
    }
}
