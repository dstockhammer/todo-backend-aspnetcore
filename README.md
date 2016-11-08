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
* Local (full stack on one VM)
  * API: [http://localhost](http://localhost)
  * Kibana (ELK): [http://localhost:5601/app/kibana#/discover/Default](http://localhost:5601/app/kibana#/discover/Default])
  * Consul UI: [http://localhost:8500/ui](http://localhost:8500/ui)
  
* Azure (full stack on one VM)
  * API: [http://todo-backend-aspnetcore.westeurope.cloudapp.azure.com](http://todo-backend-aspnetcore.westeurope.cloudapp.azure.com/)
  * Kibana (ELK): [http://localhost:5601/app/kibana#/discover/Default](http://localhost:5601/app/kibana#/discover/Default])
  * Consul UI: [http://todo-backend-aspnetcore.westeurope.cloudapp.azure.com:8500/ui](http://todo-backend-aspnetcore.westeurope.cloudapp.azure.com:8500/ui)

* Heroku (only one API instance)
  * API: [https://todo-backend-aspnetcore.herokuapp.com](https://todo-backend-aspnetcore.herokuapp.com)


# Deployment using Docker
(using Terminal in OSX, starting directory is project root)

## Local
* Build and run just the API, printing all stdout from the container:
```
$ cd src/TodoBackend.Api/
$ docker build -t todo-backend-aspnetcore:latest .
$ docker run -p 80:5000 todo-backend-aspnetcore
```

* **OR** run the entire stack in the backgroud and scale the API to 5 containers:
```
$ docker-machine up -d
$ docker-machine scale api=5
```

## Azure
See [https://docs.docker.com/machine/drivers/azure](https://docs.docker.com/machine/drivers/azure)

* Create resource group with virtual machine
```
$ docker-machine create -d azure \
  --azure-ssh-user ops \
  --azure-subscription-id <subscription id> \
  --azure-location westeurope \
  --azure-open-port 80 \
  --azure-open-port 8500 \
  --azure-open-port 5601 \
  machine
```

* Use `docker-machine` to ssh into the VM in Azure, if required
```
$ docker-machine ssh machine
```

* Use `docker-machine` to get the VM's public IP address, if required
```
$ docker-machine ip machine
```

* We're using version 5 of the ELK stack, which requires at least 2GB of memory. So let's ssh into the VM and once connected, increase the limit (see [Elastic Search documentation](https://www.elastic.co/guide/en/elasticsearch/reference/5.0/vm-max-map-count.html) for details)
```
ops@machine:~$ sysctl -w vm.max_map_count=262144
```
 
* Set the newly created machine as default. This makes all `docker` commands go to the machine in Azure
```
$ docker-machine env machine
$ eval $(docker-machine env machine)
```

* Use the regular `docker` or `docker-compose` CLI to start the entire stack or an individual container. Note that this is **NOT** how one would start and scale an infrastructure stack in a real production scenario.
```
$ docker-compose up -d
```

### Optional: Azure CLI
* Install Azure CLI, login, an set mode to Azure Resource Manager (ARM)
```
$ npm install -g azure-cli
$ azure login
$ azure config mode arm
```

* If you have multiple subscriptions, use the CLI to show your subscriptions and select a default
```
$ azure account list
$ azure account set <subscription id>
```

* Display details of the VM
```
$ azure vm show -g docker-machine -n machine
```

* Show public IP and DNS configuration; add a DNS name to the public IP. The VM is now available as http://todo-backend-aspnetcore.westeurope.cloudapp.azure.com
```
$ azure network public-ip list -g docker-machine
$ azure network public-ip show -g docker-machine -n machine-ip
$ azure network public-ip create -g docker-machine -n machine-ip -l westeurope -d "todo-backend-aspnetcore" -a "Dynamic"
```

## Heroku
* Create a Heroku account and create an app (in this example, the app is called `todo-backend-aspnetcore`
* Install the Heroku toolbelt: https://devcenter.heroku.com/articles/heroku-command-line

```
$ cd src/TodoBackend.Api/
$ docker build -t todo-backend-aspnetcore:latest -f Dockerfile.heroku .
$ docker tag todo-backend-aspnetcore registry.heroku.com/todo-backend-aspnetcore/web
$ docker push registry.heroku.com/todo-backend-aspnetcore/web
$ heroku open --app todo-backend-aspnetcore
```

Alternatively, if the directory contains a Heroku-compatible dockerfile (no `EXPOSE` and listening on `$PORT`)
```
$ heroku plugins:install heroku-container-registry
$ heroku heroku container:login
$ heroku container:push web --app todo-backend-aspnetcore
$ heroku open --app todo-backend-aspnetcore
```