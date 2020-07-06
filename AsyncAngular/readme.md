# Azure Async Serverless with Angular

## Introduction

Modern business applications use complex communication patterns in the backend. Multiple microservices might be involved. Machine-to-machine communication might be implemented using message bus systems. Certain aspects of transaction processing might be handled by event-driven, serverless functions.

Working with a backend like that is a challenge for UI developers, especially in the browser. An Angular front-end for instance could send a business transaction to a backend-for-frontend (BFF) webserver. Yet, it will not immediately get the result of the transaction. Technically, the BFF server will response to the UI's HTTP request but from a user's perspective, this is not the business transaction's result. It might take a while until the result she is interested in will become available.

This sample demonstrates how UI developers can make use of Azure's serverless capabilities to implement a BFF layer (Node.js) for an Angular app. It uses Azure's *SignalR* service to push messages from the server to the client *without* having to implement its own Websocket or SignalR service.

## Sample Structure

| Folder     | Content                                                                       |
| ---------- | ----------------------------------------------------------------------------- |
| *DevOps*   | ARM-Template and Azure CLI Script for setting up Azure for running the sample |
| *Backend*  | Backend (*Azure Function App*) written in Node.js                             |
| *Frontend* | Angular Frontend                                                              |
| *Slides*   | PPT Slides                                                                    |
