pipeline {
    agent any

    environment {
        APP_SERVER  = "192.168.56.50"
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

        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {

                    echo "--- Stop service & Clean deploy folder ---"
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            sudo systemctl stop rise || true;
                            sudo rm -rf ${DEPLOY_PATH}/*;
                            sudo mkdir -p ${DEPLOY_PATH};
                            sudo chown -R vagrant:vagrant ${DEPLOY_PATH};
                        "
                    '''

                    echo "--- Copy new publish files ---"
                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" \
                        ./publish/ vagrant@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    echo "--- Restart Rise service ---"
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            sudo systemctl daemon-reload;
                            sudo systemctl restart rise;
                        "
                    '''
                }
            }
        }

        stage('Smoke Test') {
            steps {
                echo "--- Smoke Test: HTTP check ---"
                sh "sleep 4"
                sh "curl -f http://${APP_SERVER}:5000 || exit 1"
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
                ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no \
                vagrant@${APP_SERVER} "sudo systemctl status rise --no-pager; tail -n 200 ${DEPLOY_PATH}/app.log"
            ''' || true
        }
    }
}
