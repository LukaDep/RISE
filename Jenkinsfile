pipeline {
    agent any

    environment {
        APP_SERVER  = "10.11.2.31"
        DEPLOY_PATH = "/var/www/dotnetapp"
        SSH_KEY     = "/var/lib/jenkins/.ssh/appserver_key"
    }

    stages {

        stage('Cleanup Workspace') {
            steps {
                echo "🧹 Cleaning workspace..."
                cleanWs(deleteDirs: true, disableDeferredWipeout: true)
            }
        }

        stage('Restore & Build') {
            steps {
                echo "=== Restore & Build ==="
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish Linux') {
            steps {
                echo "=== Publishing (Linux-x64, non-self-contained) ==="
                sh '''
                    dotnet publish src/Rise.Server/Rise.Server.csproj \
                      -c Release \
                      -o publish \
                      --no-self-contained \
                      -r linux-x64
                '''
            }
        }

        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    echo "--- Clean deploy folder ---"
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vicuser@$APP_SERVER "
                            pkill -f 'dotnet Rise.Server.dll' || true;
                            rm -rf ${DEPLOY_PATH}/*;
                            mkdir -p ${DEPLOY_PATH};
                        "
                    '''

                    echo "--- Copy published files ---"
                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" \
                        ./publish/ vicuser@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    echo "--- Start app manually ---"
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vicuser@$APP_SERVER "
                            cd ${DEPLOY_PATH} && \
                            nohup dotnet Rise.Server.dll > app.log 2>&1 &
                        "
                    '''
                }
            }
        }

        stage('Smoke Test') {
            steps {
                echo "--- Smoke Test: HTTP check ---"
                sh "sleep 5"
                sh "curl -f http://${APP_SERVER}:80 || exit 1"
            }
        }
    }

    post {
        success {
            echo '✅ Deployment succesvol!'
        }
        failure {
            echo '❌ Deployment mislukt — logs ophalen ↓'
            sh '''
                ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vicuser@${APP_SERVER} "
                    tail -n 100 ${DEPLOY_PATH}/app.log || true
                "
            '''
        }
    }
}
