pipeline {
    agent any
    
    environment {
        APP_SERVER = "192.168.56.50"
        APP_SERVER_USER = "vagrant"
        DEPLOY_BASE_PATH = "/opt/Rise.Server"
        CURRENT_PATH = "/opt/Rise.Server/current"
        RELEASES_PATH = "/opt/Rise.Server/releases"
        SSH_KEY = "/var/lib/jenkins/.ssh/appserver_key"
    }
    
    stages {
        stage('Checkout') {
            steps {
                echo "=== Repository checkout ==="
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
                    script {
                        // Create timestamped release
                        def timestamp = sh(script: "date +%Y%m%d%H%M%S", returnStdout: true).trim()
                        def releaseDir = "${RELEASES_PATH}/${timestamp}"
                        
                        echo "--- Creating release directory: ${releaseDir} ---"
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                sudo mkdir -p ${releaseDir} ${CURRENT_PATH} ${RELEASES_PATH};
                                sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${DEPLOY_BASE_PATH};
                            "
                        """
                        
                        echo "--- Copy new publish files to release ---"
                        sh """
                            rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" \
                            ./publish/ ${APP_SERVER_USER}@$APP_SERVER:${releaseDir}/
                        """
                        
                        echo "--- Update current symlink and restart service ---"
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                # Set proper permissions
                                sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${releaseDir};
                                sudo chmod -R 755 ${releaseDir};
                                
                                # Remove existing current symlink
                                sudo rm -f ${CURRENT_PATH} || true;
                                
                                # Create new symlink
                                sudo ln -sf ${releaseDir} ${CURRENT_PATH};
                                
                                # Ensure proper ownership
                                sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${CURRENT_PATH};
                                
                                # Restart service
                                sudo systemctl daemon-reload;
                                sudo systemctl stop rise || true;
                                sudo systemctl start rise;
                                sudo systemctl enable rise;
                            "
                        """
                    }
                }
            }
        }
        
        stage('Health Check') {
            steps {
                echo "--- Health Check ---"
                sh "sleep 5"
                script {
                    // Check from Jenkins
                    sh "curl -f http://${APP_SERVER}:5000 || exit 1"
                    
                    // Additional server-side checks
                    withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                echo '=== Service Status ==='
                                sudo systemctl status rise --no-pager || true;
                                echo '=== Port Check ==='
                                sudo netstat -tlnp | grep :5000 || echo 'Checking alternative port...';
                                sudo ss -tlnp | grep :5000 || echo 'Port not found';
                                echo '=== Local Connection Test ==='
                                curl -f http://localhost:5000 && echo 'Local connection successful' || echo 'Local connection failed';
                            "
                        """
                    }
                }
            }
        }
    }
    
    post {
        success {
            echo '✅ Deployment succesvol!'
        }
        failure {
            echo '❌ Deployment mislukt — troubleshooting info ↓'
            withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                sh """
                    ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                        echo '=== Service Status ===';
                        sudo systemctl status rise --no-pager;
                        echo '=== Recent Logs ===';
                        sudo journalctl -u rise --no-pager -n 50 || echo 'Journal not available';
                        echo '=== Application Logs ===';
                        if [ -f ${CURRENT_PATH}/app.log ]; then
                            tail -n 100 ${CURRENT_PATH}/app.log;
                        else
                            echo 'No app.log found in current deployment';
                            find ${DEPLOY_BASE_PATH} -name '*.log' -exec tail -n 50 {} \\; 2>/dev/null || echo 'No log files found';
                        fi;
                        echo '=== Current Deployment Contents ===';
                        ls -la ${CURRENT_PATH}/ || echo 'Current path not accessible';
                    "
                """ || true
            }
        }
    }
}
