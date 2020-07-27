# Azure Private Endpoints

## Introduction

This sample demonstrates Azure Private Endpoints for App Service and SQL Database (PaaS flavor). The SQL DB and the private web API are NOT accessible over the public internet. Only the public web API can be reached from outside.

For demo purposes, the private web API (aka backend API) returns the IP address of the SQL DB (will be a local IP address from our Azure VNet because we will prevent access from the public internet) and the result of a dummy query (to demonstrate the DB access really works). The public web API (aka frontend API) returns the IP address of the backend API (again a local IP address from our VNet) and the result from calling the backend API.

```txt
                                       +-------------------------------------------------------------+
                                       | A Z U R E                                                   |
           .-~~~-.                     |                                                             |
   .- ~ ~-(       )_ _                 |     +-----------------+           +-----------------+       |
  /                     ~ -.           |     |                 |           |                 |       |
 |      I N T E R N E T      \  ===========> | Public Web API  | ========> | Private Web API |       |
  \                         .'         |     |                 |           |                 |       |
    ~- . _____________ . -~            |     +-----------------+           +-----------------+       |
                                       |                                             |               |
                                       |                                             |               |
                                       |                                             V               |
                                       |                                          _______            |
                                       |                                         /       \           |
                                       |                                        |\_______/|          |
                                       |                                        |         |          |
                                       |                                        |  SQL DB |          |
                                       |                                        |         |          |
                                       |                                         \_______/           |
                                       |                                                             |
                                       +-------------------------------------------------------------+
```

## Code for APIs

You can find the code for the backend API [on GitHub](https://github.com/rstropek/Samples/blob/master/AzurePrivateEndpoints/BffWithBackendAndSQL/BackendService/Controllers/DatabaseController.cs). The code for the frontend API is [also there](https://github.com/rstropek/Samples/blob/master/AzurePrivateEndpoints/BffWithBackendAndSQL/FrontendService/Controllers/FrontendController.cs).

Both APIs are available on the Docker hub, too:

* https://hub.docker.com/repository/docker/rstropek/pep-demo-backend
* https://hub.docker.com/repository/docker/rstropek/pep-demo-frontend
  
This sample uses these Docker images in conjunction with Azure App Service for Linux.
