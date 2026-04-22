Create a workspace for our project. It must contain three projects:

* A library with business logic and data access layer (no UI)
* A Next.js 16 project that uses the library
* A console app that uses the library

## General Rules

* Use TypeScript with strict type checking
* Use Biome.js for linting and formatting
* Use tsc for type checking
* Use Vitest for unit testing
* Use Playwright for end-to-end testing
* Add support for Font Awesome icons (free icons are sufficient)
* Use the latest version of all dependencies. Research the documentation to ensure you are following the latest good practices.

## Library

For now, the library should just export a `math` module with an `add` function. This is just an example that we will replace later.

The library must also contain unit tests with Vitest. Add a demo test for the `add` function.

## Next.js project

Consult the `next-devtools` MCP server for the latest documentation and best practices for creating a new Next.js project.

Create a new Next.js 16 project with App Router in the current folder. Use Biome.js, not ESLint.

The application should consist of a single page that displays _Hello World_ and the result of calling the `add` function from the library. Add a `thumbs-up` icon to verify that Font Awesome is working. Write an end-to-end Playwright test verifying the "Hello World" page.

Disable auto CSS injection for Font Awesome and import styles in the root layout to prevent the icon flash on load.

Add a single demo unit test (1 + 2 = 3) to make sure Vitest is working.

## Console app

The console app must display the result of calling the `add` function from the library with 22 and 20 as arguments.

We do not need any automated tests for the console app for now.

## Acceptance criteria

* Workspace has been created
* All projects have been created
* Root `package.json` contains scripts for linting, formatting, type checking, testing, starting the web app, and running the console app
* `npm outdated` shows no outdated dependencies
* Quality guidelines from AGENTS.md have been followed
* Playwright tests are working
* You ran the console app and verified its output
