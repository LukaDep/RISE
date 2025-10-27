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
                    
                    echo "--- Set execute permissions and start application ---"
                    sh '''
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                      cd ${DEPLOY_PATH};
                      # Stop any running instance
                      sudo pkill -f 'Rise.Server' || true;
                      sudo pkill -f 'dotnet.*5000' || true;
                      sleep 2;
                      # Make sure the binary is executable
                      chmod +x Rise.Server;
                      # Start the app directly on 0.0.0.0:5000
                      nohup ./Rise.Server --urls \"http://0.0.0.0:5000\" > app.log 2>&1 &
                      echo 'Application started on 0.0.0.0:5000';
                      sleep 3;
                      # Check if process is running
                      ps aux | grep Rise.Server | grep -v grep;
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
            echo '❌ Deployment mislukt — logs ophalen ↓'
            script {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh """
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                      echo '=== Process status ===';
                      ps aux | grep Rise.Server | grep -v grep || echo 'No Rise.Server process found';
                      echo '=== App logs ==='; 
                      cd ${DEPLOY_PATH} && tail -n 50 app.log 2>/dev/null || echo 'No app.log found';
                      echo '=== Check port 5000 ===';
                      sudo netstat -tlnp | grep :5000 || echo 'Nothing listening on port 5000';
                    "
                    """
                }
            }
        }
    }
}
