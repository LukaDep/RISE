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
                    echo "--- Full deployment in one command ---"
                    sh '''
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                      set -e
                      echo '=== Starting full deployment ==='
                      
                      # Stop existing processes
                      echo '1. Killing all dotnet processes...'
                      sudo pkill -f 'dotnet' || true
                      sleep 3
                      
                      # Clean deploy folder
                      echo '2. Cleaning deploy folder...'
                      sudo rm -rf ${DEPLOY_PATH}/*
                      sudo mkdir -p ${DEPLOY_PATH}
                      sudo chown -R vagrant:vagrant ${DEPLOY_PATH}
                      
                      echo '3. Processes stopped and folder cleaned'
                    "
                    
                    # Copy repository (separate command for better progress)
                    echo '4. Copying repository...'
                    rsync -avz -e \"ssh -i $SSH_KEY -o StrictHostKeyChecking=no\" \
                      ./ vagrant@$APP_SERVER:${DEPLOY_PATH}/ \
                      --exclude='.git/' \
                      --exclude='bin/' \
                      --exclude='obj/' \
                      --exclude='.vs/' \
                      --exclude='TestResults/'
                    
                    # Build and start application
                    echo '5. Building and starting application...'
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                      set -e
                      cd ${DEPLOY_PATH}
                      
                      echo '6. Building on server...'
                      dotnet build src/Rise.Server/Rise.Server.csproj -c Release
                      
                      echo '7. Starting application...'
                      nohup dotnet run --project src/Rise.Server/Rise.Server.csproj --urls \\\"http://0.0.0.0:5000\\\" > app.log 2>&1 &
                      
                      echo '8. Waiting for startup...'
                      sleep 15
                      
                      echo '9. Checking deployment...'
                      echo '=== Process check ==='
                      ps aux | grep 'dotnet.*Rise.Server' | grep -v grep
                      echo '=== Port check ==='
                      ss -tlnp | grep :5000 || echo 'Port 5000 not listening'
                      echo '=== Recent logs ==='
                      tail -n 10 app.log 2>/dev/null || echo 'No logs yet'
                      echo '=== Deployment completed ==='
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
                      ls -la ${DEPLOY_PATH}/ | head -10;
                    "
                    """
                }
            }
        }
    }
}
