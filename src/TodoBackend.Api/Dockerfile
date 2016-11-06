FROM microsoft/dotnet:latest

### install ELK filebeat

ENV FILEBEAT_VERSION 5.0.0

RUN apt-get update -qq \
    && apt-get install -qqy curl \
    && apt-get clean

RUN curl -L -O https://artifacts.elastic.co/downloads/beats/filebeat/filebeat-${FILEBEAT_VERSION}-amd64.deb \
    && dpkg -i filebeat-${FILEBEAT_VERSION}-amd64.deb \
    && rm filebeat-${FILEBEAT_VERSION}-amd64.deb

ADD filebeat.yml /etc/filebeat/filebeat.yml

### build dotnet app

COPY . /app
WORKDIR /app

RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]

EXPOSE 5000/tcp

CMD ["dotnet", "run", "--server.urls", "http://*:5000"]
