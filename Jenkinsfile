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
                echo "=== Ensuring required package is installed ==="
                sh 'dotnet add src/Rise.Server/Rise.Server.csproj package Serilog.Expressions --version 5.0.0 || true'

                echo "=== Restoring dependencies ==="
                sh 'dotnet restore'

                echo "=== Building project ==="
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish') {
            steps {
                echo "=== Publishing ==="
                sh 'dotnet publish src/Rise.Server/Rise.Server.csproj -c Release -o ./publish --no-build'
            }
        }

        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: "appserver-ssh", keyFileVariable: "SSH_KEY")]) {

                    echo "=== Stopping existing app ==="
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "pkill -f 'dotnet Rise.Server.dll' || true"
                    '''

                    echo "=== Syncing new build ==="
                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" \
                            ./publish/ vagrant@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    echo "=== Starting application ==="
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            cd ${DEPLOY_PATH} &&
                            nohup bash -c 'ASPNETCORE_URLS=http://0.0.0.0:5000 dotnet Rise.Server.dll > app.log 2>&1 &' 
                        "
                    '''

                    echo "=== Checking health ==="
                    sh '''
                        sleep 5
                        curl -f http://$APP_SERVER:5000/ || exit 1
                    '''
                }
            }
        }
    }

    post {
        success {
            echo "✅ Success! Deployed & running!"
        }
        failure {
            echo "❌ Build or Deploy failed! Showing logs..."
            sh '''
                ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no vagrant@$APP_SERVER \
                "tail -n 200 ${DEPLOY_PATH}/app.log"
            '''
        }
    }
}
