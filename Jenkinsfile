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
                // Gebruik framework-dependent in plaats van self-contained voor Blazor
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
                        
                        echo "--- Update deployment and fix service ---"
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                # Set proper permissions
                                sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${releaseDir};
                                sudo chmod -R 755 ${releaseDir};
                                
                                # Remove existing current directory/symlink
                                sudo rm -rf ${CURRENT_PATH} || true;
                                
                                # Create proper symlink
                                sudo ln -sf ${releaseDir} ${CURRENT_PATH};
                                sudo chown -h ${APP_SERVER_USER}:${APP_SERVER_USER} ${CURRENT_PATH};
                                
                                echo 'Deployment structure:'
                                ls -la ${DEPLOY_BASE_PATH}/
                                echo 'Release contents:'
                                ls -la ${releaseDir}/ | head -10
                                
                                # STOP en DISABLE de oude service eerst
                                sudo systemctl stop rise || true
                                sudo systemctl disable rise || true
                                sudo systemctl daemon-reload
                                
                                # Verwijder de oude service file die naar verkeerde directory wijst
                                sudo rm -f /etc/systemd/system/rise.service
                                
                                # Create new service file with CORRECT paths
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
                                
                                # Fix permissions on service file
                                sudo chmod 644 /etc/systemd/system/rise.service;
                                
                                # Reload and restart service
                                sudo systemctl daemon-reload;
                                sudo systemctl enable rise;
                                sudo systemctl start rise;
                                
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
                sh "sleep 8"
                script {
                    withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                        echo "--- Checking service status ---"
                        sh """
                            ssh -i $SSH_KEY -o StrictHostKeyChecking=no ${APP_SERVER_USER}@$APP_SERVER "
                                echo '=== Service Status ==='
                                sudo systemctl status rise --no-pager && echo '✅ Service is running' || echo '❌ Service failed';
                                
                                echo '=== Process Check ==='
                                sudo ps aux | grep -E '(Rise.Server|dotnet)' | grep -v grep || echo 'No application process found';
                                
                                echo '=== Port Check ==='
                                sudo ss -tlnp | grep :5000 || echo 'No process listening on port 5000';
                                
                                echo '=== Recent Logs ==='
                                sudo journalctl -u rise --no-pager -n 10 || echo 'No journal logs available';
                                
                                echo '=== Verify Paths ==='
                                echo 'Current path:'
                                readlink -f ${CURRENT_PATH}
                                echo 'DLL exists:'
                                ls -la ${CURRENT_PATH}/Rise.Server.dll || echo 'DLL not found'
                            "
                        """
                        
                        // External health check
                        echo "--- Testing external connection ---"
                        sh """
                            for i in {1}{1..3}; do
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
                            echo '1. Current Service File:'
                            sudo cat /etc/systemd/system/rise.service || echo 'No service file found'
                            
                            echo '2. Service Status:'
                            sudo systemctl status rise --no-pager || echo 'Service not accessible'
                            
                            echo '3. Full Service Logs:'
                            sudo journalctl -u rise --no-pager -n 30 || echo 'No journal logs'
                            
                            echo '4. Directory Structure:'
                            ls -la ${DEPLOY_BASE_PATH}/
                            echo 'Current symlink target:'
                            ls -la ${CURRENT_PATH}/
                            echo 'DLL check:'
                            ls -la ${CURRENT_PATH}/Rise.Server.dll || echo 'DLL missing'
                            
                            echo '5. Manual Start Test:'
                            sudo systemctl stop rise 2>/dev/null || true
                            sleep 2
                            if [ -f ${CURRENT_PATH}/Rise.Server.dll ]; then
                                echo 'Testing manual start...'
                                cd ${CURRENT_PATH} && /usr/bin/dotnet Rise.Server.dll --urls http://0.0.0.0:5000 &
                                sleep 5
                                echo 'Manual start processes:'
                                ps aux | grep 'dotnet.*Rise.Server' | grep -v grep || echo 'No manual processes'
                                echo 'Port check:'
                                sudo ss -tlnp | grep :5000 || echo 'No port 5000'
                                # Cleanup
                                pkill -f 'dotnet.*Rise.Server' 2>/dev/null || true
                            fi
                        "
                    """
                }
            }
        }
    }
}
