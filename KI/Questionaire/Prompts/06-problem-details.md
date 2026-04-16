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

## Angular Error Handling

* Add an HTTP interceptor that detects `application/problem+json` responses.
* Parse the ProblemDetails body and surface `title`, `detail`, and `errors` (for validation) to the UI via a shared error service or signal.
* Update the questionnaire editor and answer form to display validation errors from ProblemDetails responses.

## Self-Improving Skill

After implementing ProblemDetails, update the `aspnet-webapi` skill (`SKILL.md`) to include the ProblemDetails configuration pattern you just applied, so future projects benefit from it automatically.

## After making changes

Follow the Quality Assurance steps in AGENTS.md. Additionally:

* Regenerate the Angular API client (`pnpm run generate-web-api` in the Frontend folder)
