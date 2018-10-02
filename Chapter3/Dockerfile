FROM ubuntu:16.04
MAINTAINER Amit Malik(contact2amitmalik@gmail.com)
#Define Work directory
WORKDIR /app 
#Preare Image for NodeJS and Install NodeJS with build tools including NPM
RUN apt-get update
RUN apt-get install -y sudo wget curl git openssl gnupg &&\
    curl -sL https://deb.nodesource.com/setup_8.x | sudo -E bash - &&\
    sudo apt-get install -y nodejs &&\
    sudo apt-get install -y build-essential
#Copying only Package file to the image
COPY package*.json /app/
#Install all dependencies etc
RUN npm install
#Copying everything from local app folder inside app folder
COPY . /app/
#Define what will happen when the container will start
CMD npm start
#Expose the port where your application is running
EXPOSE 3000
