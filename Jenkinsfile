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
                echo "=== Restoring dependencies ==="
                sh 'dotnet restore'

                echo "=== Building project ==="
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish') {
            steps {
                echo "=== Publishing application ==="
                sh 'dotnet publish src/Rise.Server/Rise.Server.csproj -c Release -o ./publish --no-build'
            }
        }

        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {

                    echo "=== Stopping old app ==="
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            pkill -f 'dotnet Rise.Server.dll' || true
                        "
                    '''

                    echo "=== Copying published files ==="
                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" \
                          ./publish/ vagrant@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    echo "=== Starting new version ==="
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vagrant@$APP_SERVER "
                            nohup bash -c 'ASPNETCORE_URLS=http://0.0.0.0:5000 dotnet ${DEPLOY_PATH}/Rise.Server.dll > ${DEPLOY_PATH}/app.log 2>&1 &' 
                        "
                    '''

                    echo "=== Checking service health ==="
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
            echo "✅ Build & Deployment success!"
        }
        failure {
            echo "❌ Build or Deployment failed! Showing logs:"
            sh """
                ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no vagrant@$APP_SERVER \
                'tail -n 200 ${DEPLOY_PATH}/app.log || echo \"No logs available\"'
            """
        }
    }
}
