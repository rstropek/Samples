# .NET Isolated Functions

## Introduction

This sample demonstrates differences between .NET in-process and isolated in Azure Functions.

## Roadmap

![.NET Functions Roadmap](https://techcommunity.microsoft.com/t5/image/serverpage/image-id/262318i4234B132C742509C/image-size/large)
([Source](https://techcommunity.microsoft.com/t5/apps-on-azure/net-on-azure-functions-roadmap/ba-p/2197916))

## Current State

* .NET 5 is supported in *isolated process* ([read more](https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide))
* *Visual Studio 2019 v16.10* brought full support for creating, local debugging, and deploying .NET 5.0 isolated process Azure Functions apps.
* .NET 6.0 on Azure Functions is currently offered as an early preview.
* Azure Functions V4 is available (since June 9th) as an [early preview](https://github.com/Azure/Azure-Functions/wiki/V4-early-preview).
  * Supports .NET 6 in-process function apps (limited VS support)

## Samples

| Project                     | Scenario                           |
| --------------------------- | ---------------------------------- |
| Functions.DotNetCore3       | .NET Core 3.1 in-process functions |
| Functions.DotNetCore6       | .NET Core 6 isolated functions     |
| Functions.DotNetCore6InProc | .NET Core 6 in-process functions   |
