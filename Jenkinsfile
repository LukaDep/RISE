pipeline {
    agent any
    
    environment {
        APP_SERVER = "192.168.56.50"
        DEPLOY_PATH = "/var/www/dotnetapp"
        SSH_KEY = "/var/lib/jenkins/.ssh/appserver_key"
    }
    
    stages {
        stage('Checkout') {
            steps {
                echo "=== Checking out code ==="
                checkout scm
            }
        }
        
        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    echo "--- Stop existing processes ---"
                    sh '''
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                      # Eenvoudige kill die we weten dat werkt
                      echo 'Killing all dotnet processes...';
                      sudo pkill -f 'dotnet' || true;
                      sleep 3;
                      echo 'Processes stopped';
                    "
                    '''
                    
                    echo "--- Clean deploy folder ---"
                    sh '''
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                      sudo rm -rf ${DEPLOY_PATH}/*;
                      sudo mkdir -p ${DEPLOY_PATH};
                      sudo chown -R vagrant:vagrant ${DEPLOY_PATH};
                    "
                    '''
                    
                    echo "--- Copy FULL repository ---"
                    sh '''
                    rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" \
                      ./ vagrant@$APP_SERVER:${DEPLOY_PATH}/ \
                      --exclude='.git/' \
                      --exclude='bin/' \
                      --exclude='obj/' \
                      --exclude='.vs/' \
                      --exclude='TestResults/'
                    '''
                    
                    echo "--- Build and start on server ---"
                    sh '''
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                      cd ${DEPLOY_PATH};
                      echo '=== Building on server ===';
                      dotnet build src/Rise.Server/Rise.Server.csproj -c Release;
                      echo '=== Starting application ===';
                      nohup dotnet run --project src/Rise.Server/Rise.Server.csproj --urls \"http://0.0.0.0:5000\" > app.log 2>&1 &
                      echo 'Application started in background';
                      sleep 15;
                      echo '=== Process check ===';
                      ps aux | grep 'dotnet.*Rise.Server' | grep -v grep;
                      echo '=== Port check ===';
                      ss -tlnp | grep :5000 || echo 'Port 5000 not listening';
                      echo '=== Recent logs ===';
                      tail -n 10 app.log 2>/dev/null || echo 'No logs yet';
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
                echo "✅ Site is accessible!"
            }
        }
    }
    
    post {
        success {
            echo '✅ Deployment succesvol!'
        }
        failure {
            echo '❌ Deployment mislukt — debugging info ↓'
            script {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh """
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                      echo '=== Full diagnostics ===';
                      echo '=== Current processes ===';
                      ps aux | grep dotnet | grep -v grep;
                      echo '=== Port 5000 status ===';
                      ss -tulpn | grep :5000 || echo 'Port 5000 is free';
                      echo '=== App logs ===';
                      cd ${DEPLOY_PATH} && tail -n 30 app.log 2>/dev/null || echo 'No app.log found';
                      echo '=== Directory structure ===';
                      find ${DEPLOY_PATH} -type f -name '*.cs' | head -10;
                    "
                    """
                }
            }
        }
    }
}
