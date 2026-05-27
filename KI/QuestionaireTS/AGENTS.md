<background>
This is a sample project for learning how to work with the coding agent OpenAI Codex. The scenario is a system for questionnaires. Customers can use the system to put together one or more questionnaires and share them for other people to fill out. The system will then collect the answers and provide some basic analytics.
</background>

<documentation-and-samples>
If you need to consider up-to-date documentation and samples (e.g. because you are performing an important task, you are specifically asked to, etc.), consult the following URLs and MCP servers:

* For next.js:
  * `next-devtools` MCP server
  * Note that you MUST CALL `init` in every chat session before using any other tools from this MCP server.
* For Drizzle ORM:
  * Context7 library ID: llmstxt/orm_drizzle_team_llms_txt
* For Biome.js:
  * Context7 library ID: biomejs/biome
* For Vitest:
  * Context7 library ID: vitest-dev/vitest
* For Fontawesome:
  * Get up-to-date documentation at https://docs.fontawesome.com/llms.txt

Note: Read the `find-docs` skill before using the Context7 CLI.
</documentation-and-samples>

<package-manager>
Use pnpm as package manager.
</package-manager>

<files>
Inspect `package.json` to see the dependencies with versions and scripts.

Ignore all files in the `prompts` folder unless you are explicitly asked to read one of them.

Use the following folder structure:
* `packages/lib` — library with business logic and data access layer
* `apps/web` — Next.js project that uses the library
* `apps/console` — console app that uses the library

Do NOT add any data access code to the web or console apps. This code must be in the library.
</files>

<nextjs-guidelines>
* Use App Router
* Use Server Components for data fetching
* Use Client Components for UI components that need to be interactive
* Use API routes only if needed (prefer Server Components and/or Server Actions)
* When generating UI-related code, only write end-to-end tests if explicitly asked to
* Use regular CSS (modern, with nested selectors), no Tailwind
  * Limit global styles to a minimum, prefer local styles
  * Use HTML tables only if it has a significant benefit, otherwise use CSS Grid or Flexbox
* Use self-hosted Google Fonts as specified in the brand guidelines
</nextjs-guidelines>

<quality-assurance>
After updating or adding any code, make sure that linting, formatting, type checking and unit tests pass without errors/warnings. Always use the scripts in `package.json` to run checks and tests; do NOT run them manually.

Always install dependencies using pnpm. Always install the latest version if not explicitly asked to use a specific version. Do NOT add dependencies manually to `package.json`.

Do not run Playwright tests unless you are explicitly asked to.
</quality-assurance>

<playwright>
Do NOT use the playwright MCP server. Read the `playwright-cli` skill instead and use the Playwright CLI for any Playwright-related tasks.
</playwright>
