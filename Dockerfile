FROM microsoft/dotnet:1.1-sdk-msbuild

COPY . /app

WORKDIR /app/src/TodoBackend.Api

RUN dotnet restore
RUN dotnet build

EXPOSE 5000/tcp

CMD dotnet run --server.urls http://*:5000