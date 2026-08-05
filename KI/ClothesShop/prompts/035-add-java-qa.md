# Add Java quality checks

Set up a small quality baseline for the Java code:

- Spotless for formatting, with a formatter that supports Java 25
- Checkstyle for linting, using a sensible standard ruleset
- Maven compilation and tests as the remaining checks

Bind the non-mutating lint and format checks to Maven's `verify` phase so
`./mvnw verify` is the single Java QA command. Also provide
`./mvnw spotless:apply` for fixing formatting. Format the existing code and make
the full check pass.

Update AGENTS.md with a short QA checklist. After code changes, agents should run
`./mvnw verify`; after user-facing changes, they should also run the existing
Playwright end-to-end tests. Keep the checklist concise and include the exact
commands.
