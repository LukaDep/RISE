pipeline {
    agent any

    environment {
        APP_SERVER   = "192.168.56.50"
        APP_USER     = "vagrant"
        DEPLOY_PATH  = "/opt/Rise.Server"
        SSH_KEY_ID   = "appserver-ssh"
        APP_PORT     = "5000"
    }

    stages {

        stage('Restore & Build') {
            steps {
                echo "=== Restore & Build ==="
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish for Linux') {
            steps {
                echo "=== Publishing (Linux-x64 Self-contained) ==="
                sh '''
                    rm -rf publish
                    dotnet publish src/Rise.Server/Rise.Server.csproj \
                      -c Release \
                      -o publish \
                      --self-contained true \
                      -r linux-x64
                '''
            }
        }

        stage('Deploy & Run App') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: "${SSH_KEY_ID}", keyFileVariable: 'SSH_KEY')]) {
                    script {
                        echo "--- Cleaning old files and copying new build ---"
                        sh """
                            ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_USER}@${APP_SERVER} '
                                sudo mkdir -p ${DEPLOY_PATH}
                                sudo pkill -f "Rise.Server" || true
                                sudo rm -rf ${DEPLOY_PATH}/*
                            '
                        """

                        sh """
                            rsync -avz -e "ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no" \
                                ./publish/ ${APP_USER}@${APP_SERVER}:${DEPLOY_PATH}/
                        """

                        echo "--- Starting application manually ---"
                        sh """
                            ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_USER}@${APP_SERVER} '
                                export ASPNETCORE_URLS="http://0.0.0.0:${APP_PORT}"
                                export ASPNETCORE_ENVIRONMENT=Production
                                cd ${DEPLOY_PATH}
                                sudo nohup ./Rise.Server > app.log 2>&1 &
                                sleep 3
                                sudo ps aux | grep Rise.Server | grep -v grep || echo "App failed to start!"
                            '
                        """
                    }
                }
            }
        }

        stage('Health Check') {
            steps {
                echo "--- Checking if app is reachable ---"
                sh "sleep 5"
                sh "curl -f http://${APP_SERVER}:${APP_PORT} || (echo '❌ App not responding!' && exit 1)"
            }
        }
    }

    post {
        success {
            echo "✅ Deployment succesvol! App draait op http://${APP_SERVER}:${APP_PORT}"
        }

        failure {
            echo "❌ Deployment mislukt. Logs ophalen..."
            script {
                sh """
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_USER}@${APP_SERVER} '
                        echo "--- Laatste regels van app.log ---"
                        sudo tail -n 40 ${DEPLOY_PATH}/app.log || echo "Geen logbestand gevonden"
                        echo "--- Processtatus ---"
                        sudo ps aux | grep Rise.Server | grep -v grep || echo "Geen actief proces"
                    '
                """
            }
        }
    }
}
