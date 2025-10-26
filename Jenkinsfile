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

                    echo "=== Stop old app and clean folder ==="
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            sudo pkill -f 'Rise.Server.dll' || true;
                            sudo rm -rf ${DEPLOY_PATH};
                            sudo mkdir -p ${DEPLOY_PATH};
                            sudo chown -R vagrant:vagrant ${DEPLOY_PATH};
                        "
                    '''

                    echo "=== Copy new publish output ==="
                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" ./publish/ \
                        vagrant@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    echo "=== Start application with correct binding ==="
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
                sleep 5
                sh '''
                    curl -f http://192.168.56.50:5000/ || exit 1
                '''
            }
        }
    }

    post {
        success {
            echo "✅ DEPLOY OK! ✅"
        }
        failure {
            echo "❌ DEPLOY FAIL"
        }
    }
}
