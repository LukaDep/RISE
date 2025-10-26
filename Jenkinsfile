pipeline {
    agent any

    environment {
        APP_NAME    = "Rise.Server"
        APP_SERVER  = "192.168.56.50"
        DEPLOY_PATH = "/var/www/dotnetapp"
    }

    stages {
        stage('Restore & Build') {
            steps {
                sh 'dotnet add src/Rise.Server/Rise.Server.csproj package Serilog.Expressions --version 5.0.0 || true'
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
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER bash << 'EOF'
                            echo "=== Stop oude app ==="
                            sudo pkill -f 'dotnet Rise.Server.dll' || true

                            echo "=== Leegmaken en permissies ==="
                            sudo rm -rf ${DEPLOY_PATH}
                            sudo mkdir -p ${DEPLOY_PATH}
                            sudo chown vagrant:vagrant ${DEPLOY_PATH}

                            echo "=== Files kopiëren... ==="
EOF
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" ./publish/ vagrant@$APP_SERVER:${DEPLOY_PATH}/

                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER bash << 'EOF'
                            echo "=== Start nieuwe versie ==="
                            cd ${DEPLOY_PATH}
                            nohup bash -c 'ASPNETCORE_URLS=http://0.0.0.0:5000 dotnet Rise.Server.dll > app.log 2>&1 &' 

                            echo "5 seconden wachten..."
                            sleep 5

                            echo "=== Test applicatie ==="
                            curl -f http://localhost:5000/ || exit 1
EOF
                    '''
                }
            }
        }
    }

    post {
        success { echo "✅ Alles gelukt!" }
        failure {
            echo "❌ Fout — logs op appserver volgen:"
            sh 'ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no vagrant@192.168.56.50 tail -n 200 /var/www/dotnetapp/app.log || true'
        }
    }
}
