pipeline {
    agent any
    
    environment {
        // Application server configuration
        APP_SERVER_HOST = '192.168.56.50'
        APP_SERVER_USER = 'vagrant'
        APP_NAME = 'Rise.Server'
        APP_PORT = '5000'
        
        // Build configuration
        DOTNET_VERSION = '9.0'
        BUILD_CONFIGURATION = 'Release'
        PUBLISH_DIR = 'publish'
        MAIN_PROJECT = 'src/Rise.Server/Rise.Server.csproj'
        
        // Deployment paths
        DEPLOY_BASE_PATH = '/var/www/dotnetapp'
        CURRENT_PATH = '/var/www/dotnetapp/current'
    }
    
    options {
        // Keep build history
        buildDiscarder(logRotator(numToKeepStr: '10'))
    }
    
    stages {
        stage('Checkout') {
            steps {
                script {
                    echo "Checking out repository"
                    checkout scm
                    
                    // Get commit information
                    sh '''
                    echo "Repository Information:"
                    echo " - Branch: $(git branch --show-current)"
                    echo " - Commit: $(git rev-parse HEAD)"
                    echo " - Author: $(git log -1 --pretty=format:'%an <%ae>')"
                    echo " - Message: $(git log -1 --pretty=format:'%s')"
                    '''
                }
            }
        }
        
        stage('Build') {
            steps {
                script {
                    echo "Building .NET ${DOTNET_VERSION} application..."
                    
                    // Restore dependencies
                    sh "dotnet restore ${MAIN_PROJECT}"
                    
                    // Build the solution
                    sh "dotnet build ${MAIN_PROJECT} --configuration ${BUILD_CONFIGURATION} --no-restore"
                    
                    echo "Build completed successfully!"
                }
            }
        }
        
        stage('Publish Server Only') {
            steps {
                script {
                    echo "Publishing SERVER ONLY for deployment..."
                    
                    // Clean publish directory
                    sh "rm -rf ${PUBLISH_DIR}"
                    
                    // Publish ONLY the server project with workaround for WebAssembly
                    sh """
                    dotnet publish ${MAIN_PROJECT} \
                      --configuration ${BUILD_CONFIGURATION} \
                      --output ${PUBLISH_DIR} \
                      --no-build \
                      --no-restore \
                      /p:WebAssemblyBuild=false
                    """
                    
                    echo "Server application published to ${PUBLISH_DIR}"
                }
            }
        }
        
        stage('Deploy') {
            steps {
                script {
                    echo "Deploying to application server ${APP_SERVER_HOST}..."
                    
                    // Use Jenkins credentials for SSH authentication
                    withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                        
                        // Stop existing processes first
                        sh """
                        ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_SERVER_USER}@${APP_SERVER_HOST} << 'EOF'
                          echo 'Stopping existing processes...'
                          sudo pkill -f 'dotnet' || true
                          sleep 3
                          echo 'Processes stopped'
                        EOF
                        """
                        
                        // Clean and prepare deploy directory
                        sh """
                        ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_SERVER_USER}@${APP_SERVER_HOST} << 'EOF'
                          sudo rm -rf ${DEPLOY_BASE_PATH}/*
                          sudo mkdir -p ${DEPLOY_BASE_PATH}
                          sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${DEPLOY_BASE_PATH}
                        EOF
                        """
                        
                        // Copy application files to server
                        sh """
                        scp -i ${SSH_KEY} -o StrictHostKeyChecking=no -r ${PUBLISH_DIR}/* ${APP_SERVER_USER}@${APP_SERVER_HOST}:${DEPLOY_BASE_PATH}/
                        """
                        
                        // Deploy and start application
                        sh """
                        ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_SERVER_USER}@${APP_SERVER_HOST} << 'EOF'
                          # Fix permissions
                          sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${DEPLOY_BASE_PATH}
                          sudo chmod -R 755 ${DEPLOY_BASE_PATH}
                          
                          # Start the application
                          cd ${DEPLOY_BASE_PATH}
                          echo 'Starting application on 0.0.0.0:${APP_PORT}...'
                          nohup dotnet Rise.Server.dll --urls "http://0.0.0.0:${APP_PORT}" > app.log 2>&1 &
                          
                          echo 'Waiting for startup...'
                          sleep 10
                          
                          # Check if service is running
                          echo '=== Deployment Status ==='
                          if ps aux | grep -q "[d]otnet.*Rise.Server.dll"; then
                            echo '✅ Service is running!'
                            echo '=== Process Info ==='
                            ps aux | grep "[d]otnet.*Rise.Server.dll"
                            echo '=== Port Info ==='
                            ss -tlnp | grep :${APP_PORT} || echo 'Port ${APP_PORT} not listening'
                          else
                            echo '❌ Service failed to start'
                            echo '=== Recent Logs ==='
                            tail -n 20 app.log 2>/dev/null || echo 'No logs found'
                            exit 1
                          fi
                        EOF
                        """
                        
                        echo "Deployment completed successfully!"
                    }
                }
            }
        }
        
        stage('Health Check') {
            steps {
                script {
                    echo "Performing health check..."
                    
                    // Wait for service to start
                    sh "sleep 5"
                    
                    // Test HTTP endpoint
                    sh """
                    echo "Testing connection to ${APP_SERVER_HOST}:${APP_PORT}..."
                    curl -f http://${APP_SERVER_HOST}:${APP_PORT} || {
                      echo "Health check failed - application not responding on port ${APP_PORT}"
                      exit 1
                    }
                    """
                    
                    echo "✅ Health check passed - application is running successfully!"
                }
            }
        }
    }
    
    post {
        always {
            echo "Pipeline execution completed"
        }
        success {
            echo "✅ Pipeline completed successfully!"
        }
        failure {
            echo "❌ Pipeline failed. Check logs for more details."
            script {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh """
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_SERVER_USER}@${APP_SERVER_HOST} << 'EOF' || true
                      echo '=== FAILURE DIAGNOSTICS ==='
                      echo '=== Current Processes ==='
                      ps aux | grep dotnet | grep -v grep || echo 'No dotnet processes found'
                      echo '=== Port Check ==='
                      ss -tlnp | grep :5000 || echo 'Nothing on port 5000'
                      echo '=== App Logs ==='
                      tail -n 30 ${DEPLOY_BASE_PATH}/app.log 2>/dev/null || echo 'No app logs found'
                      echo '=== Directory Contents ==='
                      ls -la ${DEPLOY_BASE_PATH}/ 2>/dev/null || echo 'Deploy directory not found'
                    EOF
                    """
                }
            }
        }
    }
}
