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
                echo "=== Serilog.Expressions toevoegen ==="
                sh 'dotnet add src/Rise.Server/Rise.Server.csproj package Serilog.Expressions --version 5.0.0 || true'

                echo "=== Dependencies binnenhalen ==="
                sh 'dotnet restore'

                echo "=== Project bouwen ==="
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish') {
            steps {
                echo "=== Applicatie publiceren ==="
                sh 'dotnet publish src/Rise.Server/Rise.Server.csproj -c Release -o ./publish --no-build'
            }
        }

        stage('Deploy') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {

                    sh '''
                        echo "=== Oude app stoppen ==="
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER \
                            "sudo pkill -f 'dotnet Rise.Server.dll' || true"

                        echo "=== Deploy map leegmaken ==="
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER \
                            "sudo rm -rf $DEPLOY_PATH/* && sudo mkdir -p $DEPLOY_PATH && sudo chown vagrant:vagrant $DEPLOY_PATH"

                        echo "=== Nieuwe bestanden kopiëren ==="
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" ./publish/ \
                            vagrant@$APP_SERVER:$DEPLOY_PATH/

                        echo "=== Applicatie starten ==="
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER \
                            "cd $DEPLOY_PATH && nohup bash -c 'ASPNETCORE_URLS=http://0.0.0.0:5000 dotnet Rise.Server.dll > app.log 2>&1 &'"

                        echo "=== Even wachten op startup ==="
                        sleep 5

                        echo "=== Controleren of app werkt ==="
                        curl -f http://$APP_SERVER:5000/ || exit 1
                    '''
                }
            }
        }
    }

    post {
        success {
            echo "✅ Deploy gelukt! Applicatie draait! ✅"
        }
        failure {
            echo "❌ Deploy gefaald — logs worden getoond:"
            sh '''
                ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no \
                    vagrant@$APP_SERVER "tail -n 200 $DEPLOY_PATH/app.log" || echo "Geen logs gevonden"
            '''
        }
    }
}
