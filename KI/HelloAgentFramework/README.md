# Hello Agent Framework — AG-UI + Aspire + local Ollama

A small, end-to-end sample that shows several features of the **Microsoft Agent Framework**:

- Hosting an `AIAgent` as an **AG-UI**-compatible HTTP/SSE endpoint (ASP.NET Core Minimal API, no auth).
- Driving the agent with a **local Ollama** model (`gpt-oss:20b`) through `OllamaSharp`.
- A **function tool** (`AddNumbers`) that the model calls on its own.
- **.NET Aspire** orchestration (AppHost + ServiceDefaults) for one-command local startup and telemetry.
- A **hand-written TypeScript + React (Vite) client** that speaks the AG-UI protocol directly — *no
  prebuilt AG-UI / CopilotKit SDK* — so you can see exactly what the protocol is.

## What is AG-UI?

[AG-UI](https://docs.ag-ui.com/) is a tiny protocol between a UI and an agent backend:

1. The client sends an HTTP **POST** with a JSON `RunAgentInput` body:
   `{ threadId, runId, messages: [{ id, role, content }], tools, state }`.
2. The server answers with a **Server-Sent Events** stream (`text/event-stream`). Each `data:` line is a
   JSON event with a `type`:

   | Event | Meaning |
   | --- | --- |
   | `RUN_STARTED` / `RUN_FINISHED` | start / end of a turn |
   | `TEXT_MESSAGE_START` / `_CONTENT` / `_END` | assistant text, streamed as `delta`s |
   | `TOOL_CALL_START` / `_ARGS` / `_END` / `_RESULT` | the agent calling a backend tool |
   | `REASONING_MESSAGE_CONTENT` | model "thinking" (emitted by reasoning models like gpt-oss) |
   | `RUN_ERROR` | something went wrong |

The whole client side of this lives in [`Frontend/src/aguiClient.ts`](Frontend/src/aguiClient.ts) — it's
just `fetch` + a `ReadableStream` reader that splits the stream on blank lines and `JSON.parse`s each event.

## Architecture

```
┌──────────────────────────┐      POST /ag-ui  (RunAgentInput)
│  Frontend (Vite + React) │  ───────────────────────────────►  ┌──────────────────────────┐
│  hand-written AG-UI client│  ◄───────────────────────────────  │  AgentApi (Minimal API)  │
└──────────────────────────┘      SSE event stream               │  MapAGUI("/ag-ui", agent)│
        ▲ VITE_AGENT_URL                                         │  + AddNumbers tool       │
        │ (injected by Aspire)                                   └────────────┬─────────────┘
        │                                                                     │ IChatClient
┌───────┴──────────────────┐                                     ┌────────────▼─────────────┐
│  Aspire AppHost          │  orchestrates both resources        │  Local Ollama            │
│  (dashboard, wiring)     │                                     │  gpt-oss:20b @ :11434    │
└──────────────────────────┘                                     └──────────────────────────┘
```

The browser calls the agent **cross-origin**, so the agent enables a permissive **CORS** policy (dev only).
Aspire injects the agent's URL into the Vite app as `VITE_AGENT_URL`, exposed to the browser via
`import.meta.env.VITE_AGENT_URL`.

## Prerequisites

- **.NET 10 SDK** (pinned via [`global.json`](global.json)).
- **Node.js** (for the Vite client; Aspire runs `npm install` automatically).
- **[Ollama](https://ollama.com/)** running locally with the model pulled:
  ```bash
  ollama pull gpt-oss:20b
  ```

## Run it

```bash
aspire run            # or: dotnet run --project AppHost
```

Then:

1. Open the **Aspire dashboard** (URL printed on startup, e.g. `https://localhost:17042`).
2. Open the **`frontend`** resource URL.
3. Try a normal question, and try **`What is 21 + 21?`** to watch the `AddNumbers` tool call appear in the
   chat. Assistant replies are rendered from Markdown (via [`marked`](https://marked.js.org/)).

> The first response is slow while Ollama loads `gpt-oss:20b` into memory.

## Project layout

| Project | Purpose |
| --- | --- |
| `AppHost/` | Aspire app host — wires up `agentapi` + the Vite `frontend`. |
| `ServiceDefaults/` | Shared Aspire defaults (OpenTelemetry, health checks, discovery, resilience). |
| `AgentApi/` | The agent: Ollama → `AsAIAgent` (+ `AddNumbers` tool) → `MapAGUI`. See `Program.cs`. |
| `Frontend/` | Vite + React client with a hand-written AG-UI client (`src/aguiClient.ts`, `src/App.tsx`). |

## Notes

- `Ollama:Endpoint` / `Ollama:Model` are read from `AgentApi/appsettings.json` and overridden by the
  AppHost via environment variables (`Ollama__Endpoint`, `Ollama__Model`).
- `Microsoft.Agents.AI.Hosting.AGUI.AspNetCore` is currently a **preview** package.
- The client tolerates events that omit `messageId` / `toolCallId` (some Ollama responses do).
- Markdown is rendered as raw HTML for simplicity — fine for this **local, trusted** demo, but add a
  sanitizer (e.g. DOMPurify) before pointing it at untrusted output.
