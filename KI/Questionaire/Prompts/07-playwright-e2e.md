Playwright is used in the Angular app for end-to-end tests. Read the `angular-frontend` skills for setup and coding guidelines.

Write one Playwright test covering the **questionnaire creation** workflow:

1. Navigate to the questionnaire list page.
2. Click the button/link to create a new questionnaire.
3. Fill in a title.
4. Add at least two questions (one Text, one Boolean), marking one as required and one as optional.
5. Save the questionnaire.
6. Verify that the new questionnaire appears in the list.

## After making changes

Follow the Quality Assurance steps in AGENTS.md. Additionally:

* Start the Aspire application (read the `aspire` skill for CLI details)
* Run the Playwright tests and ensure they pass
