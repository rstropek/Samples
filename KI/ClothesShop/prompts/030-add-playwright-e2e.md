# Add a Playwright end-to-end test

Add Playwright Test with TypeScript to this project. Keep the Playwright setup in
an `e2e` directory and use `npm init playwright@latest e2e` to create it. Do not
use the Java Playwright bindings.

Replace the generated example with an end-to-end test for the existing addition
page. The test should open the application, enter two numbers, submit the form,
and verify the result. Configure Playwright so the test command starts the Spring
Boot application when necessary and targets it on port 8080.

Keep the setup small and run the test to verify it works. Update AGENTS.md
to include the new test command.
