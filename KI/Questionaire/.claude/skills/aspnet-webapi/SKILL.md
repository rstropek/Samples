---
name: aspnet-webapi
description: Guidelines for creating and configuring a new ASP.NET Core Minimal API project. Use when creating a new web API project or when asked to configure an existing one.
metadata:
  author: Rainer Stropek
---

## Open API Specification

If not asked otherwise, add Open API Specification:

* Add NuGet packages:
  * `Microsoft.AspNetCore.OpenApi`
  * `Swashbuckle.AspNetCore.SwaggerUI`
  * `Microsoft.Extensions.ApiDescription.Server`
  Add Open API services to DI (`builder.Services.AddOpenApi();`)
  * Add Swagger UI middleware (`app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));`)
* Add entry to `PropertyGroup` in `.csproj` to generate Open API specification on build (`<OpenApiDocumentsDirectory>.</OpenApiDocumentsDirectory>`)

This will generate an Open API specification file (`<project-name>.json`) in the project directory on build

Always add proper Open API-related metadata (e.g. `.Produces()`, `.WithDescription()`, `.WithTags()`, etc.) to the endpoints to ensure a good API documentation and client generation experience.

## CORS

If not asked otherwise, add CORS with allow any origin, header, and method.

## Aspire Integration

If the web API is part of an Aspire application:

* Ensure that service defaults are added to DI in the web API (`builder.AddServiceDefaults()`)
* Ensure that the web API it is properly integrated with the AppHost

If you are unsure, research in the Aspire documentation at `https://aspire.dev/llms.txt`.
