Build a Web API for questionnaire management. Read the `aspnet-webapi` skill for API guidelines. Read the `aspire` skill for integration details.

## Core Ideas

* Questionnaires have a title and a list of questions. Question types for now: `Text` (free-form string) and `Boolean`.
* Answers for questions can be optional or required.
* Questionnaires are **versioned** — every update creates a new version with the full question list replaced (no partial updates).
* Deletion is **soft delete**.
* Users submit answers for a questionnaire. The full set of answers is posted in one request and validated against the questionnaire definition before storing.
* Repository lives in the `DataAccess` project using `ConcurrentDictionary` (in-memory, no database for now).
* RESTful endpoints as Minimal API, grouped under `/api/questionnaires`. Include sub-resources for versions and answers.
* Separate questionnaire endpoints and answer endpoints into their own files using C# 14 extension blocks on `IEndpointRouteBuilder` (e.g. `QuestionnaireEndpoints.cs` and `AnswerEndpoints.cs`).
* Unit tests in `DataAccessTests`, integration tests in `WebApiTests` (using the existing Aspire test fixture).

## Unit Tests

Write unit tests for the repository covering all operations mentioned above. Add successful and failure cases. Use data-driven tests where appropriate (e.g. for validation scenarios). Structure the tests properly to avoid duplication, too long files, and to ensure readability.

## Integration Tests

After implementation, add web API integration tests covering the following scenario:

1. **Create** a questionnaire "Customer Feedback Q2" with 4 questions (2x Text, 2x Boolean, 1 Text optional, 1 Boolean optional).
2. **Update** it: Add a 5th question (Boolean, optional), tweak existing question text. This becomes version 2.
3. **Submit answers** for version 1 (4 answers).
4. **Submit answers** for version 2 (5 answers).
5. **List answers** and verify both sets are returned.

Add successful and failure cases (e.g. missing required answer, invalid question ID, submitting answers for non-existent questionnaire, etc.).

## After making changes

Follow the Quality Assurance steps in AGENTS.md. Additionally:

* Regenerate the Angular API client (`pnpm run generate-web-api` in the Frontend folder)
* Start the Aspire application (see `aspire` skill) and check the logs for any errors