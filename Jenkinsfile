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
        
        stage('Clean Workspace') {
            steps {
                echo "=== Cleaning workspace ==="
                sh 'rm -rf publish || true'
            }
        }
        
        stage('Restore & Build') {
            steps {
                echo "=== Restore & Build ==="
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release --no-restore'
            }
        }
        
        stage('Publish Framework-Dependent') {
            steps {
                echo "=== Publishing (Framework-dependent) ==="
                sh '''
                dotnet publish src/Rise.Server/Rise.Server.csproj \
                    -c Release \
                    -o publish \
                    --self-contained false \
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
                        
                        echo "--- Update deployment and fix service ---"
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                # Set proper permissions
                                sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${releaseDir};
                                sudo chmod -R 755 ${releaseDir};
                                
                                # Remove existing current directory/symlink
                                sudo rm -rf ${CURRENT_PATH} || true;
                                
                                # Create proper symlink from current to release
                                sudo ln -sf ${releaseDir} ${CURRENT_PATH};
                                sudo chown -h ${APP_SERVER_USER}:${APP_SERVER_USER} ${CURRENT_PATH};
                                
                                echo '=== Deployment Verification ==='
                                echo 'Base structure:'
                                ls -la ${DEPLOY_BASE_PATH}/
                                echo 'Current symlink:'
                                ls -la ${CURRENT_PATH}
                                echo 'Release contents:'
                                ls -la ${releaseDir}/ | head -10
                                echo 'DLL check:'
                                ls -la ${CURRENT_PATH}/Rise.Server.dll || echo 'DLL not found in current'
                                ls -la ${releaseDir}/Rise.Server.dll || echo 'DLL not found in release'
                                
                                # CRITICAL: Remove OLD service file and create NEW one
                                sudo systemctl stop rise || true
                                sudo systemctl disable rise || true
                                sudo rm -f /etc/systemd/system/rise.service
                                
                                # Create NEW service file with CORRECT paths
                                sudo bash -c 'cat > /etc/systemd/system/rise.service << EOF
[Unit]
Description=Rise .NET Application
After=network.target

[Service]
Type=exec
WorkingDirectory=${CURRENT_PATH}
ExecStart=/usr/bin/dotnet ${CURRENT_PATH}/Rise.Server.dll --urls http://0.0.0.0:5000
Restart=always
RestartSec=10
User=vagrant
Group=vagrant
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_ROOT=/usr/share/dotnet

[Install]
WantedBy=multi-user.target
EOF'
                                
                                # Set proper permissions on service file
                                sudo chmod 644 /etc/systemd/system/rise.service;
                                
                                # Reload systemd and start service
                                sudo systemctl daemon-reload;
                                sudo systemctl enable rise;
                                sudo systemctl start rise;
                                
                                echo '=== Service Status ==='
                                sudo systemctl status rise --no-pager || echo 'Service status check failed';
                            "
                        """
                    }
                }
            }
        }
        
        stage('Health Check') {
            steps {
                echo "--- Health Check ---"
                sh "sleep 10"
                script {
                    withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                        echo "--- Checking service status ---"
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                echo '=== Final Verification ==='
                                echo '1. Service Status:'
                                sudo systemctl status rise --no-pager && echo '✅ Service is running' || echo '❌ Service failed';
                                
                                echo '2. Process Check:'
                                sudo ps aux | grep -E 'dotnet.*Rise.Server' | grep -v grep || echo 'No application process found';
                                
                                echo '3. Port Check:'
                                sudo ss -tlnp | grep :5000 || echo 'No process listening on port 5000';
                                
                                echo '4. Recent Logs:'
                                sudo journalctl -u rise --no-pager -n 10 || echo 'No journal logs available';
                                
                                echo '5. Path Verification:'
                                echo 'Current symlink points to:'
                                readlink -f ${CURRENT_PATH}
                                echo 'DLL exists at:'
                                ls -la \\$(readlink -f ${CURRENT_PATH})/Rise.Server.dll || echo 'DLL not found at resolved path'
                                
                                echo '6. Manual Connection Test:'
                                curl -f http://localhost:5000 && echo '✅ Local connection successful' || echo '❌ Local connection failed';
                            "
                        """
                        
                        // External health check
                        echo "--- Testing external connection ---"
                        sh """
                            for i in 1 2 3; do
                                echo "Health check attempt \$i..."
                                if curl -f --connect-timeout 10 http://${APP_SERVER}:5000; then
                                    echo '✅ Health check passed!'
                                    exit 0
                                fi
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
                            echo '1. Current Service File Content:'
                            sudo cat /etc/systemd/system/rise.service 2>/dev/null || echo 'No service file found'
                            
                            echo '2. Service Status:'
                            sudo systemctl status rise --no-pager 2>/dev/null || echo 'Service not accessible'
                            
                            echo '3. Service File Exists:'
                            sudo ls -la /etc/systemd/system/rise.service 2>/dev/null || echo 'Service file does not exist'
                            
                            echo '4. Directory Structure:'
                            ls -la ${DEPLOY_BASE_PATH}/
                            echo 'Current symlink:'
                            ls -la ${CURRENT_PATH} 2>/dev/null || echo 'Current not accessible'
                            echo 'Actual release directory:'
                            ls -la ${RELEASES_PATH}/*/ 2>/dev/null | head -5 || echo 'No releases found'
                            
                            echo '5. File Check in Current:'
                            find ${CURRENT_PATH} -name '*.dll' -type f 2>/dev/null | head -5 || echo 'No DLLs found in current'
                            
                            echo '6. Manual Start Test:'
                            sudo systemctl stop rise 2>/dev/null || true
                            sleep 2
                            RELEASE_DIR=\\$(readlink -f ${CURRENT_PATH})
                            if [ -f \\\\$RELEASE_DIR/Rise.Server.dll ]; then
                                echo 'Testing manual start from: \\\\$RELEASE_DIR'
                                cd \\\\$RELEASE_DIR && /usr/bin/dotnet Rise.Server.dll --urls http://0.0.0.0:5000 &
                                sleep 5
                                echo 'Manual start result:'
                                ps aux | grep 'dotnet.*Rise.Server' | grep -v grep || echo 'No manual processes'
                                sudo ss -tlnp | grep :5000 || echo 'No port 5000'
                                # Cleanup
                                pkill -f 'dotnet.*Rise.Server' 2>/dev/null || true
                            else
                                echo 'DLL not found at: \\\\$RELEASE_DIR/Rise.Server.dll'
                                find ${DEPLOY_BASE_PATH} -name 'Rise.Server.dll' -type f || echo 'DLL not found anywhere'
                            fi
                        "
                    """
                }
            }
        }
    }
}
