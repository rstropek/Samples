# SignalR "Hello World" Sample

## Introduction

This quite small sample can be used to describe the general idea 
of [SignalR](http://www.asp.net/signalr). It sets up a self-hosted
OWin server (command line exe) with a server-side SignalR hub
and a simple JavaScript client.

With the client, users can start a game or join an existing game.
Once a user starts a game, it waits for another user to join. 
Once a second user joined, both players can start shooting. Each
game has exactly two players.

## NuGet Packages

This samples uses the following NuGet packages:

- `Microsoft.AspNet.SignalR.SelfHost`
- `Microsoft.Owin.Cors`
- `Microsoft.Owin.StaticFiles`

## Components of the Sample

- [Startup.cs](Startup.cs) contains the OWin startup code
- [GameHub.cs](GameHub.cs) contains the server-side SignalR hub
- [index.html](wwwroot/index.html) and [index.js](wwwroot/index.js)
  contain the client-side SignalR code