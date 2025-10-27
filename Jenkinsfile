pipeline {
    agent any
    
    environment {
        // Application server configuration
        APP_SERVER = '192.168.56.50'
        APP_SERVER_USER = 'vagrant'
        DEPLOY_PATH = '/var/www/dotnetapp'
        SSH_KEY_ID = 'appserver-ssh'  // Jenkins credentials ID voor SSH key
        
        // Build configuration
        DOTNET_VERSION = '9.0'
        BUILD_CONFIGURATION = 'Release'
        PUBLISH_DIR = 'publish'
        MAIN_PROJECT = 'src/Rise.Server/Rise.Server.csproj'
    }
    
    stages {
        stage('Restore & Build') {
            steps {
                echo "=== Restore & Build ==="
                sh 'dotnet restore ${MAIN_PROJECT}'
                sh 'dotnet build ${MAIN_PROJECT} --configuration ${BUILD_CONFIGURATION} --no-restore'
            }
        }
        
        stage('Publish Self-Contained Linux') {
            steps {
                echo "=== Publishing (Linux-x64 Self-contained) ==="
                sh """
                dotnet publish ${MAIN_PROJECT} \
                    -c ${BUILD_CONFIGURATION} \
                    -o ${PUBLISH_DIR} \
                    --self-contained true \
                    -r linux-x64
                """
            }
        }
        
        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: SSH_KEY_ID, keyFileVariable: 'SSH_KEY')]) {
                    echo "--- Clean deploy folder ---"
                    sh """
                    ssh -i \${SSH_KEY} -o StrictHostKeyChecking=no ${APP_SERVER_USER}@${APP_SERVER} \
                        "sudo rm -rf ${DEPLOY_PATH}/*; \
                         sudo mkdir -p ${DEPLOY_PATH}; \
                         sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${DEPLOY_PATH}"
                    """
                    
                    echo "--- Copy publish files ---"
                    sh """
                    rsync -avz -e "ssh -i \${SSH_KEY} -o StrictHostKeyChecking=no" \
                        ./${PUBLISH_DIR}/ ${APP_SERVER_USER}@${APP_SERVER}:${DEPLOY_PATH}/
                    """
                    
                    echo "--- Copy mockdata ---"
                    sh """
                    rsync -avz -e "ssh -i \${SSH_KEY} -o StrictHostKeyChecking=no" \
                        ./src/Rise.Services/Schedule/MockData/ ${APP_SERVER_USER}@${APP_SERVER}:${DEPLOY_PATH}/Rise.Services/Schedule/MockData/
                    """
                    
                    echo "--- Run the application in background on 0.0.0.0:5000 ---"
                    sh """
                    ssh -i \${SSH_KEY} -o StrictHostKeyChecking=no ${APP_SERVER_USER}@${APP_SERVER} << 'EOF'
                        # Stop any existing process on port 5000
                        sudo fuser -k 5000/tcp || true
                        
                        # Run the app in background with nohup
                        cd ${DEPLOY_PATH}
                        nohup dotnet Rise.Server.dll --urls http://0.0.0.0:5000 > app.log 2>&1 &
                        
                        # Wait for startup
                        sleep 5
                        
                        # Check if it's running
                        if curl -f http://0.0.0.0:5000; then
                            echo "Application started successfully!"
                        else
                            echo "Failed to start application - check logs in ${DEPLOY_PATH}/app.log"
                            exit 1
                        fi
EOF
                    """
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
            withCredentials([sshUserPrivateKey(credentialsId: SSH_KEY_ID, keyFileVariable: 'SSH_KEY')]) {
                sh """
                ssh -i \${SSH_KEY} -o StrictHostKeyChecking=no \
                    ${APP_SERVER_USER}@${APP_SERVER} "tail -n 200 ${DEPLOY_PATH}/app.log" || true
                """
            }
        }
    }
}
