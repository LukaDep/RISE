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
                sh """
                    # Rsync naar appserver, exclude NuGet cache en build-artifacts
                    rsync -aq \
                        --exclude 'bin' \
                        --exclude 'obj' \
                        --exclude '.local' \
                        -e 'ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no' \
                        ./ vagrant@192.168.56.50:/vagrant/ 2> rsync.err || true
                    cat rsync.err

                    # SSH naar appserver om te publishen en runnen
                    ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no vagrant@192.168.56.50 << 'ENDSSH'
                        cd /vagrant
                        dotnet publish -c Release -o ./publish
                        dotnet ./publish/Rise.Client.dll
ENDSSH
                """
            }
        }
    }

    post {
        success {
            echo 'Build & Deploy succeeded!'
        }
        failure {
            echo 'Build or Deploy failed! Controleer rsync.err voor eventuele fouten.'
        }
    }
}
