---
name: aspire
description: Aspire skill covering the Aspire CLI, Aspire SDK, and coding guidelines for how we use Aspire. Use when the user asks to create, run, debug, configure, or troubleshoot an Aspire distributed application.
metadata:
  author: Rainer Stropek
---

## No Deployment

We only use Aspire for local development, testing, and debugging. We do **not** use Aspire for deployment.

## CLI — Running, Inspecting, and Logs

For all CLI commands (start, stop, ps, describe, logs) see [references/aspire-cli.md](references/aspire-cli.md).

## Documentation

Get `https://aspire.dev/llms.txt` for links to Aspire documentation.

## Aspire Templates

You can create Aspire C# projects using dotnet templates. Get a list of installed Aspire templates using `dotnet new --list aspire`. Here are the most important ones:

* `dotnet new aspire-apphost ...`
* `dotnet new aspire-servicedefaults ...`

## New Aspire Project

New Aspire projects in our team always consist of the following components:

* Aspire projects:
  * _AppHost_ project (default name `AppHost`).
  * _Service Defaults_ project (default name `ServiceDefaults`).
* Web API projects:
  * Web API project (aka _backend_) (default name `WebApi`; `dotnet new webapi ...`)
  * xUnit Integration test project for the Web API (default name `WebApiTests`). Add the nuget package `Aspire.Hosting.Testing` for testing.
    * The xUnit test must provide an `IAsyncLifetime` fixture class (`WebApiTestFixture.cs`) that uses Aspire's `DistributedApplicationTestingBuilder` to provide a `HttpClient` for testing the Web API.
  * Project must be added to Aspire AppHost with endpoint URLs
  * Follow the guidelines for creating new ASP.NET Core Minimal API projects in the corresponding skill
* Data access and logic projects:
  * Class library for data access and logic (default name `DataAccess`)
  * xUnit test project for the class library (default name `DataAccessTests`)
* Angular frontend project (default name `Frontend`)
  * Project must be added to Aspire AppHost with endpoint URLs
  * Uses Web API
  * Follow the guidelines for creating new Angular projects in the corresponding skill

Add the necessary .NET project references using `dotnet add reference ...`.

All .NET projects are added to a common solution. We use the new `slnx` format, not `sln` (research if you are unsure how to convert).
