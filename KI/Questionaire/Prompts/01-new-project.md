Setup a new project for Questionaire management according to the `aspire` skill. Follow the guidelines in `angular-frontend` and `aspnet-webapi` skills for creating the projects.

Add demo unit tests (`true == true`) to the xUnit projects to ensure that tests work.

Add a single `ping` endpoint to the web API that returns `pong`. Remove all other auto-generated demo code from the web api. Add an integration test to the web api tests project that uses Aspire testing to call the `ping` endpoint and verify that it returns `pong`.

Remove the default home page from the Angular frontend. Replace it with code that uses the generated API client to call the `ping` endpoint and display the result.

## After making changes

Follow the Quality Assurance steps in AGENTS.md. Additionally:

* Generate the Angular API client (`pnpm run generate-web-api` in the Frontend folder)
* Start the Aspire application (`dotnet run --project AppHost` in the root)
* Use Aspire CLI to verify that everything is running:
  * Read the corresponding skill to learn about the Aspire CLI
  * Use `aspire describe` to get the URLs of the running services
  * Check logs
  * Do a smoke test of the API and the Angular frontend with `curl`
