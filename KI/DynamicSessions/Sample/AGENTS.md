## Tech Stack

* Node.js
* TypeScript
* undici for fetch API

## Folders

* Source in src
* Compiled code in dist

## QA Guidelines

* Whenever code changes are made, run the relevant QA commands before finishing.
* For this project, run `npm run check` and `npm run build` after code changes unless the user explicitly asks not to.
* If a QA step cannot be run, clearly state that in the final response.

## Documentation and Research Guidelines

* If you need information about Azure-related topics, use the `microsoft-docs` and `microsoft-code-reference` skills. Do NOT use the Microsoft Lean MCP server, use the Microsoft Learn CLI instead.
* Before researching on Azure Container Apps Dynamic Sessions, read the `dynamic-sessions` skill as it summarizes key concepts and code samples on that topic.
* Before researching on how to acquire Azure access tokens (during development or in code), read the `azure-identity-defaultazurecredential` skill as it provides detailed guidance and examples.
* If you need information about OpenAI Agent SDK, read the docs at https://openai.github.io/openai-agents-js/llms.txt (contains links to further topics). Prefer this official documentation over Context7.
* For all other topics, use the Context7 CLI based on the `find-docs` skill.
* Only if you cannot find the necessary information using the above skills, use web search.

**Source Priority**:

1. Relevant local skill (`dynamic-sessions`, `azure-identity-defaultazurecredential`)
2. Microsoft docs skills, only as fallback
3. Web search, only as last resort
