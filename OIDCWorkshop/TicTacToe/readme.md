# TicTacToe Sample

## Introduction

I use this sample to demonstrate basics of [OpenID Connect](openid.net/) with [Identity Server](https://identityserver.io).

## Sample Workflow

### Create *Identity Server*

Create an *Identity Server* with `dotnet new is4inmem` (make sure you install [Identity Server's templates](https://github.com/IdentityServer/IdentityServer4.Templates))

* Update all NuGet references (if you want to be safe, don't include pre-releases)
* In my demos, I like to add *Application Insights*
* Disable starting of a browser when debugging (its annoying)
* Run it (listens on port 5000)

### Run *UI*

Go into `ui` folder. Run `npm install` and `npm rebuild` to build it (result will be in `dist` folder). Next, you can run the web server for the UI with `npm start`. It listens on port 5002. Finally, open the UI with a browser.

Here are some ideas what you could do for demos:

* Speak about client-side Application Insights
* Speak about how [OIDC Client Library](https://github.com/IdentityModel/oidc-client-js) is used
* Analyze authorize link

### Run *Web API*

Go into `save-api` folder. Run `npm install` and `node server.js` to start it. It listens on port 5001.

Here are some ideas what you could do for demos:

* Speak about server-side Application Insights
* Speak about server-side JWT check

 
