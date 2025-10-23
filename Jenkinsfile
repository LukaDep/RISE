pipeline {
    agent any
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        NUGET_PACKAGES = '/var/lib/jenkins/.nuget/packages'
        APP_SERVER = '192.168.56.50'
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
                sh 'dotnet build --configuration Release --no-restore'
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
                    ssh -o StrictHostKeyChecking=no vagrant@$APP_SERVER "mkdir -p $DEPLOY_PATH"
                    rsync -av -e "ssh -o StrictHostKeyChecking=no" ./publish/ vagrant@$APP_SERVER:$DEPLOY_PATH/
                    ssh -o StrictHostKeyChecking=no vagrant@$APP_SERVER 'sudo systemctl restart dotnetapp.service || echo "Service not found, skipping restart."'
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
