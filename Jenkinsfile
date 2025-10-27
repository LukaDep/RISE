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
                    
                    echo "--- Start application ---"
                    sh '''
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                      cd ${DEPLOY_PATH};
                      # Stop any running instance
                      sudo pkill -f 'dotnet.*Rise.Server.dll' || true;
                      sleep 2;
                      # Start application with explicit logging
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
                      echo '=== Current directory and files ===';
                      cd ${DEPLOY_PATH} && pwd && ls -la;
                      echo '=== Dotnet info ===';
                      dotnet --info;
                      echo '=== Try to run app directly ===';
                      dotnet Rise.Server.dll --urls 'http://0.0.0.0:5000' || echo 'Direct run failed';
                      echo '=== Check dependencies ===';
                      ldd Rise.Server.dll 2>/dev/null || echo 'ldd not available';
                      echo '=== Full app logs ===';
                      cat app.log 2>/dev/null || echo 'No app.log found';
                    "
                    """
                }
            }
        }
    }
}
