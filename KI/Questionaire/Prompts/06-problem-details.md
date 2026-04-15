Add proper error handling to the Web API using RFC 9457 ProblemDetails. Research ProblemDetails in ASP.NET Core Minimal API in Microsoft Learn (e.g. article "Handle errors in ASP.NET Core APIs"). Also read the `aspnet-webapi` skill for API guidelines.

## Requirements

* Configure the application to return ProblemDetails for all error responses, including validation errors and not-found cases.
* Add a global exception handler so that unhandled exceptions also produce a ProblemDetails response (with appropriate status code and without leaking implementation details in production).
* Review all existing endpoints and ensure they return ProblemDetails-compliant error responses instead of plain status codes.

## Integration Tests

* Add a dummy endpoint (e.g. `GET /api/debug/throw`) that deliberately throws an unhandled exception. This endpoint is only for testing purposes.
* Add integration tests in `WebApiTests` that verify:
  * Known error cases (e.g. not-found, validation failure) return a ProblemDetails JSON response with the correct `application/problem+json` content type.
  * The dummy throw endpoint returns a ProblemDetails response (not a raw 500 or stack trace).

## After making changes

* Compile the solution (`dotnet build` in the root)
* Run C# code analysis, fix any warnings
* Run all .NET tests (`dotnet test` in the root)
* Regenerate the Angular API client (`pnpm run generate-web-api` in the Frontend folder)
* Build the Angular project (`pnpm run build` in the Frontend folder)
