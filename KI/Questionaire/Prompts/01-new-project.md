Setup a new project for Questionaire management according to the `aspire` skill. Follow the guidelines in `angular-frontend` and `aspnet-webapi` skills for creating the projects.

Add demo unit tests (`true == true`) to the xUnit projects to ensure that tests work.

Add a single `ping` endpoint to the web API that returns `pong`. Remove all other auto-generated demo code from the web api. Add an integration test to the web api tests project that uses Aspire testing to call the `ping` endpoint and verify that it returns `pong`.

Remove the default home page from the Angular frontend. Replace it with code that uses the generated API client to call the `ping` endpoint and display the result.

After generating the new project or after making significant configuration changes, always:

* Compile the solution (`dotnet build` in the root)
* Run C# Code Analysis and ensure no warnings
* Run .NET unit tests
* Generate the backend API in the Angular project from Swagger
* Build the Angular project (`npm run build` in the Angular project)
* Start the Aspire application (`dotnet run --proect AppHost` in the root)
* Use Aspire CLI to verify that everything is running:
  * Read the corresponding skill to learn about the Aspire CLI
  * Use `aspire describe` to get the URLs of the running services
  * Check logs
  * Do a smoke test of the API and the Angular frontend with `curl`
