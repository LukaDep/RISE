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
                echo "=== Dependencies binnenhalen ==="
                sh 'dotnet restore'

                echo "=== Build project ==="
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish') {
            steps {
                echo "=== Self-contained publish ==="
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

        stage('Deploy') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {

                    echo "=== Stop oude app & prepare folder ==="
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            sudo pkill -f 'dotnet Rise.Server.dll' || true;
                            sudo rm -rf ${DEPLOY_PATH};
                            sudo mkdir -p ${DEPLOY_PATH};
                            sudo chown vagrant:vagrant ${DEPLOY_PATH};
                        "
                    '''

                    echo "=== Copy files ==="
                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" ./publish/ \
                        vagrant@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    echo "=== Start new version ==="
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            cd ${DEPLOY_PATH};
                            nohup env ASPNETCORE_URLS='http://0.0.0.0:5000' dotnet Rise.Server.dll > app.log 2>&1 &
                        "
                    '''

                    echo "=== Healthcheck ==="
                    sh '''
                        sleep 5
                        curl -f http://$APP_SERVER:5000/ || exit 1
                    '''
                }
            }
        }
    }

    post {
        success {
            echo "✅ Succesvol gedeployed naar appserver!"
        }
        failure {
            echo "❌ Deploy gefaald — check logs op appserver:"
            sh 'ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no vagrant@$APP_SERVER "tail -n 100 ${DEPLOY_PATH}/app.log || echo NoLog" || true'
        }
    }
}
