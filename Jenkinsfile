pipeline {
    agent any

    environment {
        APP_SERVER  = "192.168.56.50"
        DEPLOY_PATH = "/var/www/dotnetapp"
        SSH_KEY     = "/var/lib/jenkins/.ssh/appserver_key"
    }

    stages {

        stage('Checkout') {
            steps {
                echo "=== Source ophalen ==="
                checkout scm
            }
        }

        stage('Deploy Full Repo to AppServer') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {

                    echo "--- Clean & copy repo to AppServer ---"
                    sh """
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER '
                            sudo pkill -f "Rise.Server" || true;
                            sudo rm -rf ${DEPLOY_PATH};
                            sudo mkdir -p ${DEPLOY_PATH};
                            sudo chown -R vagrant:vagrant ${DEPLOY_PATH};
                        '
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" ./ vagrant@$APP_SERVER:${DEPLOY_PATH}/
                    """
                }
            }
        }

        stage('Build & Run on AppServer') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    echo "--- Build & Start app ---"
                    sh """
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER '
                            cd ${DEPLOY_PATH};
                            dotnet restore;
                            dotnet build --configuration Release;
                            ASPNETCORE_URLS=http://0.0.0.0:5000 nohup dotnet src/Rise.Server/bin/Release/net9.0/Rise.Server.dll > app.log 2>&1 &
                        '
                    """
                }
            }
        }

        stage('Smoke Test') {
            steps {
                sh "sleep 5"
                sh "curl -f http://${APP_SERVER}:5000 || exit 1"
            }
        }
    }

    post {
        failure {
            echo "❌ Deploy mislukt — logs ophalen"
            sh "ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} 'tail -n 200 ${DEPLOY_PATH}/app.log' || true"
        }
        success {
            echo "✅ Deployment gelukt!"
        }
    }
}
