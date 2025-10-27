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
                sh 'dotnet restore -r linux-x64'  // Runtime identifier meegeven bij restore
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
                    --no-restore \
                    -p:UseAppHost=true
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
                        
                        echo "--- Fix symlink structure and update service ---"
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                # Set proper permissions
                                sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${releaseDir};
                                sudo chmod -R 755 ${releaseDir};
                                sudo chmod +x ${releaseDir}/Rise.Server 2>/dev/null || true;
                                
                                # Remove existing current directory/symlink
                                sudo rm -rf ${CURRENT_PATH} || true;
                                
                                # Create proper symlink
                                sudo ln -sf ${releaseDir} ${CURRENT_PATH};
                                sudo chown -h ${APP_SERVER_USER}:${APP_SERVER_USER} ${CURRENT_PATH};
                                
                                echo 'Deployment structure:'
                                ls -la ${DEPLOY_BASE_PATH}/
                                echo 'Current symlink points to:'
                                ls -la ${CURRENT_PATH}/
                                
                                # Update or create service file
                                sudo bash -c 'cat > /etc/systemd/system/rise.service << EOF
[Unit]
Description=Rise .NET Application
After=network.target

[Service]
Type=exec
WorkingDirectory=${CURRENT_PATH}
ExecStart=${CURRENT_PATH}/Rise.Server --urls http://0.0.0.0:5000
Restart=always
RestartSec=10
User=vagrant
Group=vagrant
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_ROOT=${CURRENT_PATH}

[Install]
WantedBy=multi-user.target
EOF'
                                
                                # Fix permissions on service file
                                sudo chmod 644 /etc/systemd/system/rise.service;
                                
                                # Reload and restart service
                                sudo systemctl daemon-reload;
                                sudo systemctl stop rise || true;
                                sleep 3
                                sudo systemctl start rise;
                                sudo systemctl enable rise;
                                
                                echo 'Service status after restart:'
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
                sh "sleep 10"  // Geef de service meer tijd om te starten
                script {
                    withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                        echo "--- Checking service status on server ---"
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                echo '=== Service Status ==='
                                sudo systemctl status rise --no-pager && echo '✅ Service is running' || echo '❌ Service failed';
                                
                                echo '=== Process Check ==='
                                sudo ps aux | grep -E '(Rise.Server|dotnet)' | grep -v grep || echo 'No application process found';
                                
                                echo '=== Port Check ==='
                                sudo ss -tlnp | grep :5000 || echo 'No process listening on port 5000';
                                
                                echo '=== Recent Logs ==='
                                sudo journalctl -u rise --no-pager -n 15 || echo 'No journal logs available';
                                
                                echo '=== Directory Verification ==='
                                echo 'Current symlink:'
                                ls -la ${CURRENT_PATH}
                                echo 'Executable check:'
                                ls -la ${CURRENT_PATH}/Rise.Server || echo 'Executable not found'
                                file ${CURRENT_PATH}/Rise.Server 2>/dev/null || echo 'Cannot check file type'
                                
                                echo '=== Test Direct Execution ==='
                                if [ -f ${CURRENT_PATH}/Rise.Server ] && [ -x ${CURRENT_PATH}/Rise.Server ]; then
                                    echo 'Executable exists and is executable'
                                    cd ${CURRENT_PATH} && ./Rise.Server --urls http://0.0.0.0:5000 --help 2>&1 | head -5 || echo 'Direct execution test failed'
                                else
                                    echo 'Executable missing or not executable'
                                    chmod +x ${CURRENT_PATH}/Rise.Server 2>/dev/null || echo 'Cannot make executable'
                                fi
                            "
                        """
                        
                        // External health check
                        echo "--- Testing external connection ---"
                        sh """
                            for i in {1..5}; do
                                if curl -f --connect-timeout 10 http://${APP_SERVER}:5000; then
                                    echo '✅ Health check passed!'
                                    exit 0
                                fi
                                echo 'Attempt \$i failed, retrying in 3 seconds...'
                                sleep 3
                            done
                            echo '❌ All health check attempts failed'
                            exit 1
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
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                            echo '=== COMPREHENSIVE TROUBLESHOOTING ==='
                            echo '1. Service Status:'
                            sudo systemctl status rise --no-pager || echo 'Service not accessible'
                            
                            echo '2. Full Service Logs:'
                            sudo journalctl -u rise --no-pager -n 50 || echo 'No journal logs'
                            
                            echo '3. Directory Structure:'
                            find ${DEPLOY_BASE_PATH} -type f -name 'Rise.Server' -exec ls -la {} \\; || echo 'No Rise.Server executable found'
                            echo 'Current symlink details:'
                            ls -la ${CURRENT_PATH} 2>/dev/null | head -10 || echo 'Cannot access current'
                            
                            echo '4. File Permissions:'
                            ls -la ${CURRENT_PATH}/ 2>/dev/null | head -5 || echo 'Cannot list current directory'
                            
                            echo '5. Manual Service Start Attempt:'
                            sudo systemctl stop rise 2>/dev/null || true
                            sleep 2
                            if [ -f ${CURRENT_PATH}/Rise.Server ]; then
                                echo 'Trying to start manually...'
                                cd ${CURRENT_PATH} && nohup ./Rise.Server --urls http://0.0.0.0:5000 > /tmp/rise_manual.log 2>&1 &
                                sleep 5
                                echo 'Manual start result:'
                                ps aux | grep Rise.Server | grep -v grep || echo 'Manual start failed'
                                cat /tmp/rise_manual.log 2>/dev/null | tail -10 || echo 'No manual logs'
                            fi
                            
                            echo '6. System Info:'
                            uname -a
                            echo 'Dotnet info (if available):'
                            dotnet --info 2>/dev/null || echo 'Dotnet not in PATH'
                        "
                    """
                }
            }
        }
    }
}
