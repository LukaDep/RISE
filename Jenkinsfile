pipeline {
    agent any

    environment {
        APP_SERVER  = "192.168.56.50"
        DEPLOY_PATH = "/home/vagrant/riseapp"
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
                echo "--- Deploying build to app server ---"

                // 1️⃣ Clean remote deploy folder
                sh """
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} '
                        set -e
                        echo "[1/3] Cleaning target directory"
                        mkdir -p ${DEPLOY_PATH} || true
                        pkill -f Rise.Server || echo "Geen proces gevonden of geen rechten"
                        rm -rf ${DEPLOY_PATH}/* || true
                        echo "Clean complete"
                        exit 0
                    '
                """

                // 2️⃣ Copy new build
                echo "[2/3] Copying published build"
                sh """
                    rsync -avz -e "ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no" \
                        ./publish/ vagrant@${APP_SERVER}:${DEPLOY_PATH}/
                """

                // 3️⃣ Start application
                echo "[3/3] Starting Rise.Server"
                sh """
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} '
                        set -e
                        export ASPNETCORE_URLS=http://0.0.0.0:${APP_PORT}
                        export ASPNETCORE_ENVIRONMENT=Production
                        cd ${DEPLOY_PATH}
                        nohup ./Rise.Server > app.log 2>&1 &
                        sleep 3
                        ps aux | grep Rise.Server | grep -v grep || echo "⚠️ App failed to start"
                        exit 0
                    '
                """
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
            script {
                sh """
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} '
                        echo "--- Processtatus ---"
                        ps aux | grep Rise.Server | grep -v grep || echo "Geen actief proces"
                        echo ""
                        echo "--- Laatste logregels ---"
                        tail -n 50 ${DEPLOY_PATH}/app.log || echo "Geen logbestand gevonden"
                    '
                """
            }
        }
    }
}
