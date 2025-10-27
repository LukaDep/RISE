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
                    
                    echo "--- Start application with dotnet command ---"
                    sh '''
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                      cd ${DEPLOY_PATH};
                      # Stop any running instance
                      sudo pkill -f 'dotnet.*Rise.Server.dll' || true;
                      sleep 2;
                      # Start using dotnet command (more reliable)
                      nohup dotnet Rise.Server.dll --urls \"http://0.0.0.0:5000\" > app.log 2>&1 &
                      echo 'Application started with dotnet on 0.0.0.0:5000';
                      sleep 5;
                      # Check if process is running
                      echo '=== Checking process ===';
                      ps aux | grep 'dotnet.*Rise.Server.dll' | grep -v grep;
                      echo '=== Checking app log ===';
                      tail -n 10 app.log 2>/dev/null || echo 'No app.log yet';
                      echo '=== Checking port ===';
                      ss -tlnp | grep :5000 || echo 'Port 5000 not listening yet';
                    "
                    '''
                }
            }
        }
        
        stage('Smoke Test') {
            steps {
                echo "--- Smoke Test: HTTP check ---"
                sh "sleep 8"
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
            echo '❌ Deployment mislukt — logs ophalen ↓'
            script {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh """
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                      echo '=== Process status ===';
                      ps aux | grep 'dotnet.*Rise.Server.dll' | grep -v grep || echo 'No dotnet process found';
                      echo '=== App logs ==='; 
                      cd ${DEPLOY_PATH} && tail -n 30 app.log 2>/dev/null || echo 'No app.log found';
                      echo '=== Check port 5000 ===';
                      ss -tlnp | grep :5000 || echo 'Nothing listening on port 5000';
                      echo '=== Check dotnet version ===';
                      dotnet --version || echo 'Dotnet not installed';
                      echo '=== Check files ===';
                      ls -la ${DEPLOY_PATH}/ | head -10;
                    "
                    """
                }
            }
        }
    }
}
