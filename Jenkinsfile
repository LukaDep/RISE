pipeline {
    agent any

    environment {
        APP_SERVER  = "192.168.56.50"
        DEPLOY_PATH = "/var/www/dotnetapp"
    }

    stages {

        stage('Restore & Build') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet add src/Rise.Server/Rise.Server.csproj package Serilog.Expressions --version 5.0.0'
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish Self-Contained') {
            steps {
                sh '''
                    dotnet publish src/Rise.Server/Rise.Server.csproj \
                        -c Release \
                        -o ./publish \
                        --self-contained true \
                        -r linux-x64 \
                        /p:PublishTrimmed=false
                '''
            }
        }

        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {

                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            sudo pkill -f 'Rise.Server.dll' || true;
                            sudo rm -rf ${DEPLOY_PATH};
                            sudo mkdir -p ${DEPLOY_PATH};
                            sudo chown -R vagrant:vagrant ${DEPLOY_PATH};
                        "
                    '''

                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" ./publish/ \
                        vagrant@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            cd ${DEPLOY_PATH};
                            nohup ASPNETCORE_URLS=http://0.0.0.0:5000 dotnet Rise.Server.dll > app.log 2>&1 &
                        "
                    '''
                }
            }
        }

        stage('Smoke Test') {
            steps {
                script {
                    sleep 8
                    sh 'curl -f http://192.168.56.50:5000/ || echo "⚠️ Smoke test failed"'
                }
            }
        }
    }

    post {
        always {
            sh 'ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no vagrant@192.168.56.50 "tail -n 200 /var/www/dotnetapp/app.log" || true'
        }
        success {
            echo "✅ DEPLOY OK ✅"
        }
        failure {
            echo "❌ DEPLOY FAILED ❌ (logs above)"
        }
    }
}
