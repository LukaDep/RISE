pipeline {
    agent any
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        APP_SERVER = '192.168.56.50'
        DEPLOY_PATH = '/var/www/dotnetapp'
        APP_NAME = 'dotnetapp.service'
    }
    stages {
        stage('Build') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release --no-restore'
            }
        }
        stage('Publish') {
            steps {
                sh 'dotnet publish src/Rise.Server/Rise.Server.csproj -c Release -o ./publish --no-build'
            }
        }
        stage('Deploy') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh '''
                    # Kopieer files
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "sudo mkdir -p ${DEPLOY_PATH} && sudo chown vagrant:vagrant ${DEPLOY_PATH}"
                    rsync -av -e "ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no" ./publish/ vagrant@${APP_SERVER}:${DEPLOY_PATH}/
                    
                    # Maak service aan
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                        # Stop service als deze bestaat
                        sudo systemctl stop ${APP_NAME} || true
                        
                        # Create service file
                        sudo tee /etc/systemd/system/${APP_NAME} > /dev/null <<EOF
[Unit]
Description=.NET Rise App
After=network.target

[Service]
Type=notify
WorkingDirectory=${DEPLOY_PATH}
ExecStart=/usr/bin/dotnet ${DEPLOY_PATH}/Rise.Server.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF
                        
                        # Activeer service
                        sudo systemctl daemon-reload
                        sudo systemctl enable ${APP_NAME}
                        sudo systemctl start ${APP_NAME}
                    "
                    '''
                }
            }
        }
        stage('Verify') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {
                    sh '''
                    # Wacht even voor de service om op te starten
                    sleep 5
                    
                    # Check service status
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no vagrant@${APP_SERVER} "
                        echo '=== Service Status ==='
                        sudo systemctl status ${APP_NAME} --no-pager --lines=5
                        
                        echo '=== Application Logs ==='
                        sudo journalctl -u ${APP_NAME} --no-pager -n 10
                    "
                    
                    # Test de applicatie
                    echo '=== Testing Application ==='
                    curl -f http://${APP_SERVER}:5000/ || echo 'Application is starting...'
                    '''
                }
            }
        }
    }
    post {
        success {
            echo 'Build & Deploy succeeded!'
        }
        failure {
            echo 'Build or Deploy failed.'
        }
    }
}
