Build the Angular UI for questionnaire management. Read the `angular-frontend` and `frontend-design` skills for coding guidelines.

## Architecture

Separate non-UI logic from components using Angular services. Components should only handle rendering and user interaction — all API calls, data transformation, and business logic (e.g. reordering, validation, filtering deleted questionnaires) belong in services. Use signals in services to expose state that components can bind to.

## Pages

### Questionnaire List

* Show all questionnaires (latest version of each) in a table or card list.
* Toggle to show/hide soft-deleted questionnaires.
* Actions per questionnaire: edit, soft delete, undelete (for deleted ones).
* Link/button to create a new questionnaire.
* Link/button to answer a questionnaire (only for non-deleted ones).

### Questionnaire Editor (Create & Edit)

* Form for title and a list of questions.
* Each question: text, type (Text/Boolean), required/optional toggle.
* Reorder questions with arrow up/down buttons (no drag & drop).
* Add/remove questions.
* On save: creates a new questionnaire or a new version of an existing one.

### Answer Questionnaire

* Load the latest version of a questionnaire and display a form for answering.
* Render appropriate input controls based on question type (text input for Text, checkbox for Boolean).
* Mark required vs optional questions visually.
* Client-side validation before submission.

## After making changes

* Build the Angular project (`pnpm run build` in the Frontend folder)
* Start the Aspire application (see `aspire` skill) and verify with Playwright CLI that the UI renders and basic interactions work
