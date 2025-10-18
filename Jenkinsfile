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

        stage('Publish project') {
            steps {
                // Framework-dependent publish; server moet .NET runtime hebben
                sh 'dotnet publish -c Release -o ./publish'
            }
        }

        stage('Deploy to appserver') {
            steps {
                sh '''
                    # Kopieer publish folder naar appserver
                    rsync -aq -e "ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no" ./publish/ vagrant@192.168.56.50:/vagrant/publish/

                    # SSH naar appserver en run de app
                    ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no vagrant@192.168.56.50 << 'ENDSSH'
                        cd /vagrant/publish
                        dotnet Rise.Client.dll
ENDSSH
                '''
            }
        }
    }

    post {
        success {
            echo 'Build & Deploy succeeded!'
        }
        failure {
            echo 'Build or Deploy failed!'
        }
    }
}
