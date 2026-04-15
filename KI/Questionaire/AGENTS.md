## Architecture

* Read [docs/project-architecture.md](docs/project-architecture.md) for Aspire wiring, frontend-to-API URL discovery, OpenAPI client generation, non-default config, and test setup.

## Tools

* Use `pnpm` (globally installed), not `npm`

## Skills and CLIs to Use

### Browser Testing

* Playwright CLI (`playwright-cli`)
* Skill to read: `playwright-cli`

### Documentation and Samples for Microsoft Technologies

* E.g. C#, .NET, SQL Server, Microsoft Azure
* Use the **Microsoft Learn MCP plugin** tools directly:
  - `microsoft_docs_search` — find concepts, guides, tutorials
  - `microsoft_code_sample_search` — find code examples
  - `microsoft_docs_fetch` — get full page content from a URL
* Fallback (if MCP unavailable): `mslearn` CLI (globally installed)
* Prefer these over context7 / `find-docs` for Microsoft technologies.

### Documentation and Samples for Other Technologies

* context7 CLI (`ctx7`)
* Skill to read: `find-docs`

### Angular

* New Angular app: `angular-new-app`
* Writing Angular code: `angular-frontend` (prefer over context7 / `find-docs` for Angular topics)
* You can optionally use the Angular MCP server (`angular-cli`) if available

## Quality Assurance

Once you are done with changing/adding code, always:

* Run Code Analysis/Linter
  * For .NET projects, run `dotnet format style --no-restore --verify-no-changes --severity info` to catch IDE-level diagnostics (e.g. collection initializer suggestions, namespace mismatches) that `dotnet build` alone does not report. Fix all findings.
  * For Angular projects, run `pnpm ng lint` in the Frontend folder. Fix all findings.
* Compile
* Run tests (unit and integration)
* Everything must be warning- and error free
