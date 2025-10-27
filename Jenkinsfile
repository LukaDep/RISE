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
        RELEASES_PATH = '/var/www/dotnetapp/releases'
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
        
        stage('Publish') {
            steps {
                script {
                    echo "Publishing application for deployment..."
                    
                    // Clean publish directory
                    sh "rm -rf ${PUBLISH_DIR}"
                    
                    // Publish the application
                    sh "dotnet publish ${MAIN_PROJECT} --configuration ${BUILD_CONFIGURATION} --output ${PUBLISH_DIR} --no-build --self-contained false"
                    
                    echo "Application published to ${PUBLISH_DIR}"
                }
            }
        }
        
        stage('Deploy') {
            steps {
                script {
                    echo "Deploying to application server ${APP_SERVER_HOST}..."
                    
                    // Use Jenkins credentials for SSH authentication
                    withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                        
                        // Create timestamped release directory
                        def timestamp = sh(script: "date +%Y%m%d%H%M%S", returnStdout: true).trim()
                        def releaseDir = "${DEPLOY_BASE_PATH}/releases/${timestamp}"
                        
                        // Create directory structure on server
                        sh """
                        ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_SERVER_USER}@${APP_SERVER_HOST} << 'EOF'
                          sudo mkdir -p ${releaseDir}
                          sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${DEPLOY_BASE_PATH}
                        EOF
                        """
                        
                        // Copy application files to server using SSH key
                        sh """
                        scp -i ${SSH_KEY} -o StrictHostKeyChecking=no -r ${PUBLISH_DIR}/* ${APP_SERVER_USER}@${APP_SERVER_HOST}:${releaseDir}/
                        """
                        
                        // Deploy application
                        sh """
                        ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_SERVER_USER}@${APP_SERVER_HOST} << 'EOF'
                          # Fix permissions on the release directory
                          sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${releaseDir}
                          sudo chmod -R 755 ${releaseDir}
                          
                          # Remove existing current directory
                          sudo rm -rf ${DEPLOY_BASE_PATH}/current
                          
                          # Create new current directory and copy files
                          sudo mkdir -p ${DEPLOY_BASE_PATH}/current
                          sudo cp -r ${releaseDir}/* ${DEPLOY_BASE_PATH}/current/
                          sudo chown -R ${APP_SERVER_USER}:${APP_SERVER_USER} ${DEPLOY_BASE_PATH}/current
                          sudo chmod -R 755 ${DEPLOY_BASE_PATH}/current
                          
                          # Stop existing service if running
                          sudo pkill -f "dotnet.*Rise.Server.dll" || true
                          sleep 3
                          
                          # Start the application
                          cd ${DEPLOY_BASE_PATH}/current
                          nohup dotnet Rise.Server.dll --urls "http://0.0.0.0:${APP_PORT}" > app.log 2>&1 &
                          
                          echo '=== Deployment completed ==='
                          sleep 5
                          
                          # Check if service is running
                          if ps aux | grep -q "[d]otnet.*Rise.Server.dll"; then
                            echo 'Service is running!'
                          else
                            echo 'Service failed to start'
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
                    
                    // Wait a moment for service to start
                    sh "sleep 10"
                    
                    // Use Jenkins credentials for SSH authentication
                    withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                        
                        // Check if service is running and diagnose
                        sh """
                        ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_SERVER_USER}@${APP_SERVER_HOST} << 'EOF'
                          echo '=== Service Status ==='
                          if ps aux | grep -q "[d]otnet.*Rise.Server.dll"; then
                            echo 'Service ${APP_NAME} is running'
                          else
                            echo 'Service ${APP_NAME} is not running'
                            exit 1
                          fi
                          
                          echo '=== Port Binding Check ==='
                          sudo netstat -tlnp | grep :${APP_PORT} || echo 'No process listening on port ${APP_PORT}'
                          
                          echo '=== Check if binding to 0.0.0.0 ==='
                          sudo netstat -tlnp | grep :${APP_PORT} | grep 0.0.0.0 || echo 'Application not binding to 0.0.0.0'
                          
                          echo '=== Application Logs ==='
                          tail -n 20 ${DEPLOY_BASE_PATH}/current/app.log 2>/dev/null || echo 'No application logs found'
                          
                          echo '=== Local Connection Test ==='
                          curl -f http://localhost:${APP_PORT} || echo 'Local connection failed'
                        EOF
                        """
                        
                        // Test HTTP endpoint from external
                        sh """
                        echo "Testing external connection to ${APP_SERVER_HOST}:${APP_PORT}..."
                        curl -f http://${APP_SERVER_HOST}:${APP_PORT} || {
                          echo "External health check failed - application not responding on port ${APP_PORT}"
                          exit 1
                        }
                        """
                        
                        echo "Health check passed - application is running successfully!"
                    }
                }
            }
        }
    }
    
    post {
        always {
            echo "Pipeline execution completed"
        }
        success {
            echo "Pipeline completed successfully!"
        }
        failure {
            echo "Pipeline failed. Check logs for more details."
            script {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh """
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${APP_SERVER_USER}@${APP_SERVER_HOST} << 'EOF' || true
                      echo '=== FAILURE DIAGNOSTICS ==='
                      echo '=== Current Processes ==='
                      ps aux | grep dotnet | grep -v grep || echo 'No dotnet processes found'
                      echo '=== Port Check ==='
                      netstat -tlnp | grep :5000 || echo 'Nothing on port 5000'
                      echo '=== App Logs ==='
                      tail -n 50 ${DEPLOY_BASE_PATH}/current/app.log 2>/dev/null || echo 'No app logs found'
                      echo '=== Directory Contents ==='
                      ls -la ${DEPLOY_BASE_PATH}/current/ 2>/dev/null || echo 'Deploy directory not found'
                    EOF
                    """
                }
            }
        }
    }
}
