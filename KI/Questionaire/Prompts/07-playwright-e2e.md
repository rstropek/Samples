Add Playwright end-to-end tests to the Angular frontend project. Read the `angular-frontend` skills for setup and coding guidelines.

## Setup

* Install Playwright in the `Frontend` folder using `pnpm`.
* Configure Playwright to run against the local dev server URL provided by Aspire (read the `aspire` skill to understand how to start the application and discover service URLs).
* Store Playwright config and tests under `Frontend/e2e/`.

## Tests

Write one Playwright test covering the **questionnaire creation** workflow:

1. Navigate to the questionnaire list page.
2. Click the button/link to create a new questionnaire.
3. Fill in a title.
4. Add at least two questions (one Text, one Boolean), marking one as required and one as optional.
5. Save the questionnaire.
6. Verify that the new questionnaire appears in the list.

## After making changes

* Build the Angular project (`pnpm run build` in the Frontend folder)
* Start the Aspire application (read the `aspire` skill for CLI details)
* Run the Playwright tests and ensure they pass
