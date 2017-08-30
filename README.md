# GameServersRegistry
Simple demo case of how you can maintain a registry of game servers on ASP.Net Core app

## Motivation

When you run a cluster of game servers, it's important to maintain a registry of servers. It may be needed to provide a list of servers to clients. Or it may be used by other service like matchmaking to decide which server it should assign clients to.

There are various universal open source solutions out there which you can use to solve this problem. But let's see how we can quickly build own game servers registry and host it in the cloud.

Many game developers think that it's hard to build REST API. So this game servers registry is an example of simplest REST API.

This project goes as a second part of main tutorial [here](https://github.com/PoisonousJohn/TanksNetworkingInAzure).

## Project overview

The idea of this service is to share REST endpoint `/api/servers` which accepts GET and POST requests. POST request should be used by game servers to report their ip and port. It's assumed that servers will send POST requests as their heartbeat. This means that request should be sended every N seconds.

From the side of our service registry, once it received POST request, it should add a server in registry and start monitoring it. If server didn't report for M seconds, it's considered dead and will be removed from registry.

GET endpoint should return a list of available servers. Everything is very simple.

## Warning

This service keeps the registry in-memory for simplicity. This may cause problems because there may be multiple instances of the service and a load-balancer in front of them. This means that you may have multiple copies of registries. And client will get result depending on to which instance request was routed by load-balancer.

Ideally it should keep the registry in a distributed cache shared between service instances. For example it may be Redis.

So if you searching for production ready solutions, this project is not a case for sure.

Security is another thing that has been ommitted for simplicity. Right now POST endpoint may be called by anyone. In production it should be secured, so only game servers could call it.

## Deploying to Azure

### Prerequisites

Prior to working with this project, you need **Azure account** ([free trial, requires credit card](https://azure.microsoft.com/en-us/free/)) or account activated via **Azure Pass** (temporary test account provided by Microsoft).

### Creating Web App in Azure

To publish this app you might fork/clone this repository, if you familiar with git. Otherwise you may [download](https://github.com/PoisonousJohn/GameServersRegistry/archive/master.zip) this project as zip archive.

Follow tutorial [here](https://docs.microsoft.com/en-us/aspnet/core/tutorials/publish-to-azure-webapp-using-vs) assuming that you already has .Net Core project you don't need to create it.

## Testing the service

To test this service you may follow tutorial [here](https://github.com/PoisonousJohn/TanksNetworkingInAzure) and launch several instances of game server. You need to replace [this](https://github.com/PoisonousJohn/TanksNetworkingInAzure/blob/master/Dockerfile#L10) line in `Dockerfile` with your service's url in order to make game servers report to your instances of registry instead of mine.