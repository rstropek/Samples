Add Drizzle ORM with SQLite (via `better-sqlite3`) to the existing Next.js project. Research the latest Drizzle ORM documentation to ensure you follow current best practices.

## Requirements

### Database Setup

* Use SQLite as the database engine with `better-sqlite3` as the driver.
* The SQLite database file location must be configurable via `.env.local` (during development) and environment variables (in production):
  * `DB_FOLDER` — absolute path to where the database file is stored (default: `data` folder relative to the project root folder)
  * `DB_NAME` — name of the database file (default: `questionaire.db`)
* Add the database folder and `*.db` files to `.gitignore`.
* Make sure to add scripts for generating and applying migrations to the `package.json` scripts.

### Schema

* Create a demo table `questionaire` with the following columns:
  * `id` — PK (surrogate key)
  * `name` — text, not null (e.g. "Satisfaction Survey 2026")
  * `description` — longer text, not null
  * `tags` - comma-separated list of tags, text, not null (e.g. "satisfaction,2026,customer-feedback")

### Data Access Layer

* Create a data access layer that is cleanly separated from any UI code.
* Don't forget: Data access code must be in the library, not in the web or console apps.
* The data access layer must export:
  * The Drizzle database client instance (exported function `getDb()`).
  * A function (e.g. `getDbStatus`) that executes something like `SELECT 42 AS result` and returns the result. This serves as a simple health-check query.
  * A function `listQuestionaires` that returns all questionaires from the database
* Add integration tests that test the functions mentioned above **with an SQLite in-memory database**.

### UI Integration

* Update the existing home page (`app/page.tsx`) so that it calls the `getDbStatus` function from the data access layer and displays the query result (e.g. "DB status: 42") alongside the existing "Hello world!" text.
* The page must be a **Server Component**, no client-side data fetching.
* Add a call to `getDbStatus` to the console app, too, to make sure DB access works.

## Console App — Commander.js

* Remove the existing demo logic from the console app.
* Add `commander` to the console app for implementing a CLI.
* Add a `ping` command that calls `getDbStatus` and prints the result to stdout. This serves as a simple health check.
* Add a `list` command that calls `listQuestionaires` and prints the result to stdout.

Add a skill to `./.agents/skills` describing how to use the CLI.

* Name of the skill: `questionaire-cli`
* Description: "Use this skill to interact with the Questionaire CLI."
* Describe how to run the CLI using `pnpm`. Mention all available commands with example command lines.

## Acceptance Criteria

* Migrations have been generated and applied
* SQLite database file has been created
* Home page displays the DB health-check result
* There is an end-to-end test verifying the DB result on the home page
* Console app displays the DB health-check result
* Console app filled the demo data into the database
* Everything compiles, lints, formats, and type checks without errors/warnings
