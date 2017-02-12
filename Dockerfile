FROM microsoft/dotnet:1.1.0-sdk-msbuild-rc4

COPY . /app
WORKDIR /app

RUN dotnet restore
RUN dotnet publish src/TodoBackend.Api/TodoBackend.Api.csproj -c Release -o ../../bin

EXPOSE 5000

ENTRYPOINT [ "dotnet", "bin/TodoBackend.Api.dll" ]