pipeline {
    agent any
    
    tools {
        jdk 'jdk11'
        maven 'maven3'
    }

    stages {
        stage('Git Checkout') {
            steps {
                git branch: 'main', poll: false, url: 'https://github.com/madhavi2809/Reaction-Timer-Game.git'
            }
        }
        
        stage('Compile') {
            steps {
                bat "mvn clean compile"
            }
        }
        
        stage('OWASP SCAN') {
            steps {
                dependencyCheck additionalArguments: '--format HTML', odcInstallation: 'DP'
            }
        }
        
        stage('Test Application') {
            steps {
                bat "mvn clean install -DskipTests=true"
            }
        }
        
        stage('Build Maven') {
            steps {
                checkout scmGit(branches: [[name: '*/main']], extensions: [], userRemoteConfigs: [[credentialsId: '0b9dfdb5-290c-48b2-910b-0d3ebd7f853a', url: 'https://github.com/madhavi2809/jenkins-docker2.git']])
                bat "mvn -Dmaven.test.failure.ignore=true clean package"
            }
        }
        
        stage('Build Docker Image') {
            steps {
                script {
                    bat 'docker build -t madhavi2804/reaction_timer_game1 .'
                    echo 'Build Docker Image'
                }
            }
        }
        
        stage('Push Docker Image') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'docker_hub_credentials', usernameVariable: 'DOCKER_HUB_USERNAME', passwordVariable: 'DOCKER_HUB_PASSWORD')]) {
                    bat "docker login -u ${DOCKER_HUB_USERNAME} -p ${DOCKER_HUB_PASSWORD}"
                    bat "docker push madhavi2804/reaction_timer_game1"
                }
            }
        }
        
        stage('Email Trigger') {
        steps {
                echo "The Build is successful"
            }
            post {
                success {
                    emailext subject: "Pipeline '${currentBuild.fullDisplayName}' Successful",
                            body: 'Project Name: Reaction Timer Game | Build Status: SUCCESS | All the stages were executed successfully. ',
                            from: 'madhavitest18@gmail.com',
                            mimeType: 'text/html', 
                            replyTo: 'jenkins@example.com',
                            to: 'madhavi4871.be22@chitkara.edu.in',
                            attachLog: true
                }
              //  failure {
                //    emailext subject: "Pipeline '${currentBuild.fullDisplayName}' Falied",
                //            body: 'Project Name: Reaction Timer Game | Build Status: FAILURE | Check the attached logs for error. ',
                //            from: 'madhavitest18@gmail.com',
                //            mimeType: 'text/html', 
                //            replyTo: 'jenkins@example.com',
               //             to: 'madhavi4871.be22@chitkara.edu.in',
                //            attachLog: true
              //  }
            }
        }
    }
}
