pipeline {
    agent any

    environment {
        // === Cloud configuratie ===
        APP_SERVER  = "10.11.2.31"   
        DEPLOY_PATH = "/var/www/dotnetapp"
        SSH_KEY     = "/var/lib/jenkins/.ssh/appserver_key"
    }

    stages {
         stage('Install Node & Tailwind Dependencies') {
            steps {
                echo "=== Install Node Dependencies for Tailwind ==="
                // Als package.json aanwezig is, installeer Node modules
                sh '''
                    if [ -f package.json ]; then
                        npm ci || npm install
                    fi
                '''
            }
        }

        stage('Build TailwindCSS') {
            steps {
                echo "=== Building Tailwind CSS ==="
                sh '''
                    npx tailwindcss -i ./src/Rise.Client/wwwroot/css/tailwind.css \
                                    -o ./src/Rise.Client/wwwroot/css/output.css \
                                    --minify
                '''
            }
        }
        stage('Restore & Build') {
            steps {
                echo "=== Restore & Build ==="
                sh 'dotnet restore'
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
                      -r linux-x64
                '''
            }
        }

        stage('Deploy to App Server') {
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'appserver-ssh', keyFileVariable: 'SSH_KEY')]) {

                    echo "--- Stop service & Clean deploy folder ---"
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vicuser@$APP_SERVER "
                            sudo systemctl stop rise || true;
                            sudo rm -rf ${DEPLOY_PATH}/*;
                            sudo mkdir -p ${DEPLOY_PATH};
                            sudo chown -R vicuser:vicuser ${DEPLOY_PATH};
                        "
                    '''

                    echo "--- Copy new publish files ---"
                    sh '''
                        rsync -avz -e "ssh -i $SSH_KEY -o StrictHostKeyChecking=no" \
                        ./publish/ vicuser@$APP_SERVER:${DEPLOY_PATH}/
                    '''

                    echo "--- Restart Rise service ---"
                    sh '''
                        ssh -i $SSH_KEY -o StrictHostKeyChecking=no vicuser@$APP_SERVER "
                            sudo systemctl daemon-reload;
                            sudo systemctl restart rise;
                        "
                    '''
                }
            }
        }

        stage('Smoke Test') {
            steps {
                echo "--- Smoke Test: HTTP check ---"
                sh "sleep 5"
                sh "curl -f https://campusg6.vichogent.be || exit 1"
            }
        }
    }

    post {
        success {
            echo '✅ Deployment succesvol (cloud)!'
        }
        failure {
            echo '❌ Deployment mislukt — logs ophalen ↓'
            sh '''
                ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no \
                vicuser@${APP_SERVER} "sudo systemctl status rise --no-pager; tail -n 200 ${DEPLOY_PATH}/app.log"
            ''' || true
        }
    }
}
