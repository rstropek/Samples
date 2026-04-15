Analyze the project structure and create a documentation file `docs/project-architecture.md`.

Focus on things that are **non-obvious** and **hard to infer** from just reading the code. Do NOT document things that are self-evident (e.g. "this is an Angular project", "this uses ASP.NET Core"). Instead, document:

* How the Aspire AppHost wires together the projects (resource names, endpoint URLs, environment variable forwarding, references between resources). Include the exact resource names used in `AddProject`/`AddNpmApp` calls, because subsequent agents need these to match Aspire service discovery names.
* How the Angular frontend discovers the Web API URL at runtime (the `set-env.js` mechanism, which environment variables it reads, and how `environment.ts` is structured).
* How the OpenAPI client generation pipeline works end-to-end: which script generates the spec, where the spec file lands, which script generates the Angular client, and where the generated client code lives.
* Any non-default configuration in `angular.json`, `tsconfig.json`, or `.csproj` files that deviates from template defaults (e.g. `OpenApiDocumentsDirectory`, zoneless change detection, custom builder options).
* The xUnit integration test setup: how `DistributedApplicationTestingBuilder` is configured, how `HttpClient` is obtained from the test fixture, and any Aspire-specific test patterns.
* The solution file format (`slnx`) and how projects are organized within it.

Create the `docs` folder if it does not exist. Write clean, well-structured markdown. No fluff — every sentence should save a future agent at least one tool call.

Refer to the documentation file in AGENTS.md.
