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
        
        stage('Restore & Build') {
            steps {
                echo "=== Restore & Build ==="
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release --no-restore'
            }
        }
        
        stage('Publish Framework Dependent') {
            steps {
                echo "=== Publishing (Framework Dependent) ==="
                sh '''
                dotnet publish src/Rise.Server/Rise.Server.csproj \
                  -c Release \
                  -o publish \
                  --self-contained false
                '''
            }
        }
        
        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    echo "--- Clean deploy folder ---"
                    sh '''
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
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
                    
                    echo "--- Stop existing processes and start application ---"
                    sh '''
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                      cd ${DEPLOY_PATH};
                      # Stop any process using port 5000
                      echo 'Stopping processes on port 5000...';
                      sudo fuser -k 5000/tcp || true;
                      # Also stop any dotnet processes
                      sudo pkill -f 'dotnet.*Rise.Server.dll' || true;
                      sleep 3;
                      # Clear port 5000
                      sudo ss -tulpn | grep :5000 && echo 'Port 5000 still in use' || echo 'Port 5000 is free';
                      # Start application
                      echo 'Starting application...';
                      nohup dotnet Rise.Server.dll --urls \"http://0.0.0.0:5000\" > app.log 2>&1 &
                      echo 'Application start command executed';
                      sleep 8;
                      # Check if process is running
                      echo '=== Process check ===';
                      ps aux | grep 'dotnet.*Rise.Server.dll' | grep -v grep;
                      echo '=== Recent logs ===';
                      tail -n 20 app.log 2>/dev/null || echo 'No logs yet';
                      echo '=== Port check ===';
                      ss -tlnp | grep :5000 || echo 'Port 5000 not listening';
                    "
                    '''
                }
            }
        }
        
        stage('Smoke Test') {
            steps {
                echo "--- Smoke Test: HTTP check ---"
                sh "sleep 10"
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
                      echo '=== Check what is using port 5000 ===';
                      sudo ss -tulpn | grep :5000 || echo 'Port 5000 is free';
                      echo '=== Current processes ===';
                      ps aux | grep dotnet | grep -v grep;
                      echo '=== App logs ===';
                      cd ${DEPLOY_PATH} && cat app.log 2>/dev/null || echo 'No app.log found';
                    "
                    """
                }
            }
        }
    }
}
