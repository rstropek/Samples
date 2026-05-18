Add a command `mcp` to the CLI that runs a STDIO MCP server. It must offer the following tools with the same functionality as the existing CLI:
- `questionnaire_list`
- `questionnaire_get`
- `questionnaire_result`
- `submission_submit`

Describe that before `submission_submit`, an AI client must call `questionnaire_get` to get the questionnaire and its questions. The submission must fit to the questionnaire's structure, otherwise the server will reject it. The JSON schema of the MCP tool must use any-of to reflect the question types.

Add a single integration test with the MCP client (in-memory transport):
- Verify that all four tools are available
- Try to call `questionnaire_list` and verify that it does not return an error

At the end, don't forget to update docs in `./docs` and the `questionnaire-cli` skill.
