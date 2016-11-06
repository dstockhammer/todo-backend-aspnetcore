# todo-backend-aspnetcore
This is an implementation of the [Todo-Backend](http://todobackend.com) API in C# on [ASP.NET Core](https://www.asp.net/core).

## Infrastructure
* [Docker](https://www.docker.com)
* [Docker Compose](https://docs.docker.com/compose) for local deployment
* [NGINX](https://nginx.org) for load balancing the api
* [Consul](https://www.consul.io) for service discovery and configuration management
* [ELK Stack](https://www.elastic.co) for log visualisation

## Getting started
* Install the latest version of [.NET Core](https://www.microsoft.com/net/core) and [Docker](https://www.docker.com/products/docker) for your platform
* From the project root dir, use [compose](https://docs.docker.com/compose) to build the app, download all infrastructure containers and run the entire stack:
```
> docker-compose up -d
```
* If everything works, start scaling the API:
```
> docker-compose scale api=5
```

## URIs
* API (this app): [http://localhost](http://localhost)
* Kibana (ELK): [http://localhost:5601/app/kibana](http://localhost:5601/app/kibana)
* Consul UI: [http://localhost:8500/ui](http://localhost:8500/ui)
