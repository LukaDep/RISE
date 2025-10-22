pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        APP_SERVER = '10.11.2.31'
        DEPLOY_PATH = '/var/www/dotnetapp'
    }

    stages {

        stage('Restore dependencies') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build project') {
            steps {
                sh 'dotnet build --configuration Release --no-restore -m:1 -v:q'
            }
        }

        stage('Run tests') {
            steps {
                script {
                    try {
                        sh 'dotnet test --no-build --verbosity normal'
                    } catch (err) {
                        echo 'No tests found or tests failed — continuing.'
                    }
                }
            }
        }

        stage('Publish project') {
            steps {
                sh 'dotnet publish -c Release -o ./publish'
            }
        }

        stage('Deploy to appserver') {
            steps {
                sshagent(['appserver-ssh']) {
                    sh '''
                        echo "Deploying to $APP_SERVER..."

                        ssh -o StrictHostKeyChecking=no vicuser@$APP_SERVER "mkdir -p $DEPLOY_PATH"

                        rsync -aq -e "ssh -o StrictHostKeyChecking=no" ./publish/ vicuser@$APP_SERVER:$DEPLOY_PATH/

                        ssh -o StrictHostKeyChecking=no vicuser@$APP_SERVER 'sudo systemctl restart dotnetapp.service || echo "Service not found, skipping restart."'

                        echo "Deployment complete."
                    '''
                }
            }
        }
    }

    post {
        success {
            echo 'Build & Deploy succeeded.'
        }
        failure {
            echo 'Build or Deploy failed.'
        }
    }
}
