pipeline {
    agent any

    environment {
        // === Cloud configuratie ===
        APP_SERVER  = "10.11.2.31"
        DEPLOY_PATH = "/var/www/dotnetapp"
        SSH_KEY     = "/var/lib/jenkins/.ssh/appserver_key"
    }

    stages {

        stage('Restore & Build') {
            steps {
                echo "=== Restore & Build ==="
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish Self-Contained Linux') {
            steps {
                echo "=== Publishing (Linux-x64 Self-contained) ==="
                sh '''
                    dotnet publish src/Rise.Server/Rise.Server.csproj \
                      -c Release \
                      -o publish \
                      --self-contained true \
                      -r linux-x64
                '''
            }
        }

        stage('Deploy & Run on App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {

                    echo "--- Cleanup oude versie ---"
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vicuser@$APP_SERVER "
                            pkill -f 'dotnet Rise.Server.dll' || true;
                            rm -rf ${DEPLOY_PATH}/*;
                            mkdir -p ${DEPLOY_PATH};
                        "
                    '''

                    echo "--- Copy nieuwe bestanden ---"
                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" \
                        ./publish/ vicuser@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    echo "--- Start nieuwe instance ---"
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vicuser@$APP_SERVER "
                            nohup dotnet ${DEPLOY_PATH}/Rise.Server.dll > ${DEPLOY_PATH}/app.log 2>&1 &
                        "
                    '''
                }
            }
        }

        stage('Smoke Test') {
            steps {
                echo "--- Smoke Test: HTTP check ---"
                sh "sleep 5"
                sh "curl -f http://${APP_SERVER}:5000 || exit 1"
            }
        }
    }

    post {
        success {
            echo '✅ Deployment succesvol (zonder systemd)!'
        }
        failure {
            echo '❌ Deployment mislukt — logs ophalen ↓'
            sh '''
                ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no \
                vicuser@${APP_SERVER} "tail -n 100 ${DEPLOY_PATH}/app.log"
            ''' || true
        }
    }
}
