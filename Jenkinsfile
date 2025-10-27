pipeline {
    agent any

    environment {
        APP_SERVER  = "192.168.56.50"
        DEPLOY_PATH = "/opt/Rise.Server"
        SSH_KEY     = "/var/lib/jenkins/.ssh/appserver_key"
        APP_PORT    = "5000"
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
                    rm -rf publish
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
                echo "--- Cleaning and Deploying ---"
                sh '''
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                        sudo mkdir -p ${DEPLOY_PATH};
                        sudo pkill -f Rise.Server || true;
                        sudo rm -rf ${DEPLOY_PATH}/*;
                        sudo chown -R vagrant:vagrant ${DEPLOY_PATH};
                    "
                '''

                sh '''
                    rsync -avz -e "ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no" \
                        ./publish/ vagrant@${APP_SERVER}:${DEPLOY_PATH}/
                '''

                echo "--- Starting app manually on 0.0.0.0:${APP_PORT} ---"
                sh '''
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                        export ASPNETCORE_URLS=http://0.0.0.0:${APP_PORT};
                        export ASPNETCORE_ENVIRONMENT=Production;
                        cd ${DEPLOY_PATH};
                        nohup ./Rise.Server > app.log 2>&1 &
                        sleep 3;
                        sudo ps aux | grep Rise.Server | grep -v grep || echo 'App failed to start';
                    "
                '''
            }
        }

        stage('Smoke Test') {
            steps {
                echo "--- Smoke Test ---"
                sh "sleep 5"
                sh "curl -f http://${APP_SERVER}:${APP_PORT} || (echo '❌ App not reachable!' && exit 1)"
            }
        }
    }

    post {
        success {
            echo "✅ Deployment succesvol! App draait op http://${APP_SERVER}:${APP_PORT}"
        }

        failure {
            echo "❌ Deployment mislukt — logs ophalen ↓"
            sh '''
                ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                    echo '--- Processtatus ---';
                    sudo ps aux | grep Rise.Server | grep -v grep || echo 'Geen actief proces';
                    echo '';
                    echo '--- Laatste logregels ---';
                    sudo tail -n 50 ${DEPLOY_PATH}/app.log || echo 'Geen logbestand gevonden';
                "
            ''' || true
        }
    }
}
