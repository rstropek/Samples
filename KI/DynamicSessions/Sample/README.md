# Azure Dynamic Sessions Sample

This sample shows two ways to use Azure Container Apps Dynamic Sessions from a Node.js/TypeScript application:

- `simple` mode: a direct, imperative workflow
- `agent` mode: an OpenAI Agents SDK workflow with a custom tool that executes Python inside the dynamic session

The sample uses a code-interpreter session pool, uploads a CSV file into the session, runs Python against that file, and downloads generated outputs back to the local machine.

## What The Sample Demonstrates

- Acquiring a Microsoft Entra access token with `DefaultAzureCredential`
- Starting and reusing an Azure Container Apps Dynamic Session via a session identifier
- Uploading files into a code-interpreter session
- Executing Python code inside the session
- Downloading generated files from the session
- Explicitly deleting a code-interpreter session with `DELETE /session`
- Building a local custom tool for the OpenAI Agents SDK that delegates code execution to Azure Dynamic Sessions
- Keeping an interactive agent conversation in a local REPL while persisting chat memory in-process
- Synchronizing files produced by the agent back into the local `data` folder

## Project Structure

- [src/app.ts](./src/app.ts): CLI entry point and mode switch
- [src/simpleMode.ts](./src/simpleMode.ts): direct workflow without an agent
- [src/agentMode.ts](./src/agentMode.ts): OpenAI Agents SDK workflow with REPL and custom tool
- [src/azureDynamicSessions.ts](./src/azureDynamicSessions.ts): Azure authentication and Dynamic Sessions REST helpers
- [src/csvGenerator.ts](./src/csvGenerator.ts): local CSV generation/reuse logic
- [src/runtimeConfig.ts](./src/runtimeConfig.ts): environment loading and session-id resolution
- [system-prompt.md](./system-prompt.md): system prompt template for `agent` mode (supports `{{VAR}}` placeholders)
- [data](./data): generated input/output files

## Data Flow

### `simple` mode

1. Ensure `data/customer-revenue.csv` exists locally.
2. Acquire an Entra token for `https://dynamicsessions.io/.default`.
3. Start a dynamic session implicitly by issuing the first execution request.
4. Upload the CSV to `/mnt/data/customer-revenue.csv` inside the session.
5. Run Python with `pandas` to compute the average revenue per customer grouped by year.
6. Download `customer-revenue-summary.csv` back into `./data`.
7. Delete the session.

### `agent` mode

1. Ensure the same input CSV exists locally.
2. Start a dynamic session and upload the CSV.
3. Create an OpenAI Agent using `gpt-5.4`.
4. Expose a custom tool that accepts Python code from the agent and executes it inside the Azure session.
5. Start a terminal REPL.
6. For each user turn:
   - the agent decides whether to call the Python tool
   - the tool executes the generated script in the dynamic session
   - the app prints the agent answer
   - the app lists files inside the session
   - any newly generated files are downloaded into `./data`
7. When the user exits the REPL, delete the session.

## Input Data

The sample works with `data/customer-revenue.csv`.

If the file does not exist, the app generates it with:

- 10,000 rows
- customer IDs `C00001` through `C10000`
- 5,000 rows for year `2025`
- 5,000 rows for year `2026`
- random revenue values between `10000` and `12000`

If the file already exists, it is reused.

## Environment

The sample expects these values (see [.env.example](./.env.example)):

- `POOL_MANAGEMENT_ENDPOINT`: Azure Dynamic Sessions pool management endpoint
- `OPENAI_API_KEY`: required for `agent` mode
- `AZURE_TENANT_ID`: optional Entra tenant ID for `DefaultAzureCredential`
- `DYNAMIC_SESSION_ID`: optional fixed session identifier
- `EXECUTIONS_API_VERSION`: optional override for the `/executions` API version (default `2025-10-02-preview`)
- `FILES_API_VERSION`: optional override for the files API version (default `2025-10-02-preview`)
- `SESSION_DELETE_API_VERSION`: optional override for the `DELETE /session` API version (default `2025-10-02-preview`)

The project uses Node's built-in `.env` loading through:

```bash
node --env-file=.env dist/app.js
```

## Running The Sample

Install dependencies:

```bash
npm install
```

Build:

```bash
npm run build
```

Run the direct workflow:

```bash
npm start -- simple
```

Run the agent workflow:

```bash
npm start -- agent
```

Example question in `agent` mode:

```text
What is the average revenue?
```

Example task that creates a file:

```text
Create a summary CSV by year in /mnt/data named agent-summary.csv and tell me where you wrote it.
```

## Azure Dynamic Sessions Notes

- The sample targets the built-in code-interpreter session pool.
- Session creation is implicit on first use of a new `identifier`.
- Files are uploaded into `/mnt/data`.
- Python execution uses the `/executions` endpoint.
- The session is explicitly deleted with:

```http
DELETE /session?api-version=2025-10-02-preview&identifier=<SESSION_ID>
```

## OpenAI Agents SDK Notes

`agent` mode uses:

- `@openai/agents`
- model `gpt-5.4`
- a local `MemorySession` for in-process conversational state
- a custom function tool that runs Python inside Azure Dynamic Sessions instead of OpenAI-hosted Code Interpreter

The important architectural point is that the model does not execute code locally and does not execute code on OpenAI infrastructure for this sample. The model only decides what Python to run. The actual execution happens in Azure Dynamic Sessions through the custom tool.

## System Prompt

The agent's system prompt lives in [system-prompt.md](./system-prompt.md) and is loaded at startup. The file supports `{{VAR}}` placeholders which are substituted from values supplied in [src/agentMode.ts](./src/agentMode.ts). The current template uses `{{INPUT_FILE_NAME}}` so the prompt stays in sync with the filename defined by [src/csvGenerator.ts](./src/csvGenerator.ts).

The prompt tells the agent that:

- the code-interpreter kernel is persistent across tool calls within the conversation, so state (imports, DataFrames) survives between executions
- the tool only returns `stdout` and `stderr`, so values must be `print`ed
- any file written under `/mnt/data` is automatically synced to the local `./data` folder after each turn
- `pandas` is the preferred tool for tabular work

## Presentation-Friendly Logging

`agent` mode contains additional console logging intended for demos:

- section headers for lifecycle stages
- labeled user and agent output
- debug output for generated Python scripts
- debug output for Azure execution results (`stdout`/`stderr`)
- file synchronization messages when the agent creates new outputs

In an interactive terminal, ANSI colors are used to visually separate normal output from debug output.

## Useful Commands

- `npm run check`: run Biome checks
- `npm run build`: compile TypeScript
- `npm start -- simple`: run direct mode
- `npm start -- agent`: run agent mode

## Why This Sample Is Interesting

This sample is not just a CSV demo. It shows a pattern that is useful for real-world AI systems:

- use an LLM to decide what code should run
- execute that code in an isolated sandbox outside the app process
- keep file-based artifacts in the sandbox
- pull generated outputs back into the app when needed

That separation is the core concept behind the sample.
