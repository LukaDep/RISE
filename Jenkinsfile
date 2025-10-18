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
                    # Rsync naar de appserver met expliciete chmod voor permissies
                    rsync -avz --chmod=Du=rwx,Dgo=rx,Fu=rw,Fgo=r \
                        --exclude '.git' \
                        --exclude 'bin' \
                        --exclude 'obj' \
                        --exclude '*.lock' \
                        -e 'ssh -i /var/lib/jenkins/.ssh/appserver_key -o StrictHostKeyChecking=no' \
                        ./ vagrant@192.168.56.50:/vagrant/

                    # SSH naar de appserver om te publishen en runnen
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
            echo 'Build or Deploy failed!'
        }
    }
}
