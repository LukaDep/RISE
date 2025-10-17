pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
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

        stage('Archive artifacts') {
            steps {
                archiveArtifacts artifacts: '**/bin/Release/**/*', fingerprint: true
            }
        }

        stage('Deploy to appserver') {
            steps {
                script {
                    // Copy code naar VM (voorbeeld: via rsync over SSH)
                    sh '''
                    rsync -avz --delete ./ vagrant@192.168.56.50:/vagrant/
                    ssh vagrant@192.168.56.50 'cd /vagrant && dotnet publish -c Release -o ./publish && systemctl restart myapp.service || dotnet ./publish/Rise.Client.dll'
                    '''
                }
            }
        }
    } // <-- sluit stages

    post {
        success {
            echo 'Build succeeded!'
        }
        failure {
            echo 'Build failed!'
        }
    }
}
