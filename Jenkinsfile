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
                    -r linux-x64 \
                    --no-restore
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
                                sudo mkdir -p ${releaseDir} ${RELEASES_PATH};
                                sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${DEPLOY_BASE_PATH};
                            "
                        """
                        
                        echo "--- Copy new publish files to release ---"
                        sh """
                            rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" \
                            ./publish/ ${APP_SERVER_USER}@$APP_SERVER:${releaseDir}/
                        """
                        
                        echo "--- Update deployment and restart service ---"
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                # Set proper permissions
                                sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${releaseDir};
                                sudo chmod -R 755 ${releaseDir};
                                
                                # Remove existing current directory if it exists
                                sudo rm -rf ${CURRENT_PATH} || true;
                                
                                # Create symlink from current to new release
                                sudo ln -sf ${releaseDir} ${CURRENT_PATH};
                                sudo chown -h ${APP_SERVER_USER}:${APP_SERVER_USER} ${CURRENT_PATH};
                                
                                # Ensure the service file points to the correct directory
                                if [ -f /etc/systemd/system/rise.service ]; then
                                    sudo sed -i 's|WorkingDirectory=.*|WorkingDirectory=${CURRENT_PATH}|' /etc/systemd/system/rise.service
                                fi
                                
                                # Restart service
                                sudo systemctl daemon-reload;
                                sudo systemctl stop rise || true;
                                sleep 2
                                sudo systemctl start rise;
                                sudo systemctl enable rise;
                                
                                echo 'Service status:'
                                sudo systemctl status rise --no-pager || echo 'Service not running';
                            "
                        """
                    }
                }
            }
        }
        
        stage('Health Check') {
            steps {
                echo "--- Health Check ---"
                sh "sleep 8"  // Geef de service meer tijd om te starten
                script {
                    // Eerst server-side checks
                    withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                        echo "--- Checking service status on server ---"
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                echo '=== Service Status ==='
                                sudo systemctl status rise --no-pager || echo 'Service status check failed';
                                
                                echo '=== Process Check ==='
                                sudo ps aux | grep Rise.Server | grep -v grep || echo 'No Rise.Server process found';
                                
                                echo '=== Port Check ==='
                                sudo netstat -tlnp | grep :5000 || echo 'No process listening on port 5000';
                                sudo ss -tlnp | grep :5000 || echo 'ss: No process on port 5000';
                                
                                echo '=== Application Logs ==='
                                sudo journalctl -u rise --no-pager -n 20 || echo 'No journal logs available';
                                
                                echo '=== Current Directory Contents ==='
                                ls -la ${CURRENT_PATH}/ | head -10 || echo 'Cannot list current directory';
                                
                                echo '=== Try starting service if not running ==='
                                if ! sudo systemctl is-active --quiet rise; then
                                    echo 'Service not active, attempting to start...'
                                    sudo systemctl start rise;
                                    sleep 3
                                    sudo systemctl status rise --no-pager || echo 'Service failed to start';
                                fi
                                
                                echo '=== Final Service Check ==='
                                sudo systemctl is-active --quiet rise && echo '✅ Service is running!' || echo '❌ Service is not running';
                            "
                        """
                        
                        // Probeer nu external health check
                        echo "--- Testing external connection ---"
                        sh """
                            curl -f --connect-timeout 10 --max-time 15 http://${APP_SERVER}:5000 || \
                            (echo 'External connection failed, checking alternative ports...'; \
                             ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER 'sudo netstat -tlnp' || true; \
                             exit 1)
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
            script {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh """
                        echo '=== Detailed Troubleshooting ==='
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                            echo '1. Service Status:'
                            sudo systemctl status rise --no-pager || echo 'Service not found'
                            
                            echo '2. Recent Logs:'
                            sudo journalctl -u rise --no-pager -n 30 || echo 'No journal logs'
                            
                            echo '3. Network Status:'
                            sudo netstat -tlnp | grep -E ':(5000|80|8080)' || echo 'No relevant ports listening'
                            
                            echo '4. Process List:'
                            sudo ps aux | grep -E '(Rise|dotnet)' | grep -v grep || echo 'No relevant processes'
                            
                            echo '5. Directory Structure:'
                            ls -la ${DEPLOY_BASE_PATH}/ || echo 'Base path not accessible'
                            ls -la ${CURRENT_PATH}/ 2>/dev/null | head -5 || echo 'Current path not accessible'
                            
                            echo '6. Check executable:'
                            if [ -f ${CURRENT_PATH}/Rise.Server ]; then
                                echo 'Executable exists, checking permissions:'
                                ls -la ${CURRENT_PATH}/Rise.Server
                                echo 'Trying to run directly:'
                                cd ${CURRENT_PATH} && ./Rise.Server --urls http://0.0.0.0:5000 &
                                sleep 2
                                sudo netstat -tlnp | grep :5000 || echo 'Direct execution failed'
                                kill %1 2>/dev/null || true
                            else
                                echo 'Executable not found in current path'
                                find ${DEPLOY_BASE_PATH} -name 'Rise.Server' -type f || echo 'Executable not found anywhere'
                            fi
                        "
                    """
                }
            }
        }
    }
}
