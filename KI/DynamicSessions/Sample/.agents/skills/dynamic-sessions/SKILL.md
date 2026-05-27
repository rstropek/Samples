---
name: dynamic-sessions
description: Work with Azure Container Apps dynamic sessions through their REST API. Use when agents need to authenticate with Entra ID, create or reuse code interpreter sessions via identifiers, execute Python code inside a session, inspect session metadata, and upload, list, or download files independent of the client programming language.
---

# Dynamic Sessions

Azure Container Apps dynamic sessions provide short-lived, isolated sandboxes behind a session pool. This skill is intentionally language-agnostic on the client side: any language that can make HTTPS requests and attach a bearer token can use it. The only runtime assumption is that the code sent to the built-in code interpreter session is Python.

## When To Use

- Writing client code in any language that calls the dynamic sessions management API directly.
- Building backend services in any language that execute untrusted or AI-generated Python code in a session.
- Managing session identifiers, session metadata, and session lifecycle behavior.
- Uploading input files to a session and retrieving generated outputs.

## Core Concepts

### Session pool

A session pool is the Azure resource that fronts all sessions. Your app talks to the pool management endpoint, not to individual containers.

### Session identifier

The `identifier` query parameter selects the session. If the identifier already exists, the existing session is reused. If it does not exist, the first request that targets the session creates it implicitly.

### Authentication

All pool management API requests require a Microsoft Entra token and the caller must have the `Azure ContainerApps Session Executor` role on the session pool. The token must target `https://dynamicsessions.io`. Client-library details depend on the language, but the HTTP contract is the same.

## Scope

This skill focuses on the built-in **code interpreter** session pool for running Python code. Custom container sessions are out of scope unless a task explicitly asks for them.

## Preferred Workflow

1. Get the pool management endpoint:

```bash
az containerapp sessionpool show \
  --name <SESSION_POOL_NAME> \
  --resource-group <RESOURCE_GROUP> \
  --query "properties.poolManagementEndpoint" \
  --output tsv
```

2. Store it in `POOL_MANAGEMENT_ENDPOINT`.
3. Acquire a Microsoft Entra token for `https://dynamicsessions.io`.
4. Call the REST endpoints documented below.
5. Reuse the same `identifier` to keep working inside the same session.

## Quick Start

Use [code-interpreter-rest-flow.md](sample_codes/getting-started/code-interpreter-rest-flow.md) for a language-agnostic flow that:

- acquires an Entra token
- reuses a session via a caller-supplied identifier
- executes Python code inside the session
- deletes the session explicitly when finished
- uploads a file
- lists files
- downloads a file

## Common Patterns

### Authenticate

Client code varies by language, but the HTTP requirement is always:

```http
Authorization: Bearer <TOKEN_FOR_https://dynamicsessions.io>
```

### Execute Python code

```http
POST <POOL_MANAGEMENT_ENDPOINT>/executions?api-version=2025-10-02-preview&identifier=<SESSION_ID>
Content-Type: application/json
Authorization: Bearer <TOKEN>

{
  "properties": {
    "codeInputType": "inline",
    "executionType": "synchronous",
    "code": "print('Hello from Azure dynamic sessions')"
  }
}
```

### Create or reuse a session

There is no separate documented "create session" call for code interpreter pools. Creation is implicit. Send the first request with a new `identifier`; later requests with the same `identifier` reuse the same session.

### Inspect sessions

```http
POST <POOL_MANAGEMENT_ENDPOINT>/.management/getSession?identifier=<SESSION_ID>&api-version=2024-02-02-preview
Authorization: Bearer <TOKEN>
```

### Stop a session

For built-in code interpreter sessions, the current data-plane REST docs expose an explicit delete endpoint:

```http
DELETE <POOL_MANAGEMENT_ENDPOINT>/session?api-version=2025-10-02-preview&identifier=<SESSION_ID>
Authorization: Bearer <TOKEN>
```

This deletes the code-interpreter session identified by `identifier`. A later execution with the same identifier can recreate a fresh session implicitly.

Do not confuse this with the custom-container management endpoint:

```http
POST <POOL_MANAGEMENT_ENDPOINT>/.management/stopSession?api-version=2025-10-02-preview&identifier=<SESSION_ID>
```

That `stopSession` operation is documented for custom container session pools, not for the built-in code interpreter pool.

### Work with files

```http
POST <POOL_MANAGEMENT_ENDPOINT>/files?api-version=2025-10-02-preview&identifier=<SESSION_ID>
Authorization: Bearer <TOKEN>
Content-Type: multipart/form-data
```

## API Quick Reference

| Purpose | Path | Notes |
|---|---|---|
| Execute Python code | `/executions` | Current code-interpreter docs use `api-version=2025-10-02-preview` |
| Delete code-interpreter session | `/session` | `DELETE`; current docs use `api-version=2025-10-02-preview`; not for custom container pools |
| Upload file | `/files` | `multipart/form-data`; files land under `/mnt/data` |
| List files | `/files` | Returns files in `/mnt/data` |
| File metadata | `/files/{filename}` | For size and modified time |
| Download file | `/files/{filename}/content` | Binary response |
| Get session metadata | `/.management/getSession` | Docs I verified show `api-version=2024-02-02-preview` |
| List sessions | `/.management/listSessions` | Pagination via `skip` and `nextLink` |

## Important Version Note

Microsoft Learn currently shows two API shapes in different articles:

- newer code-interpreter docs: `/executions` and `/files` with `2025-10-02-preview`
- older session-pool docs: `code/execute` and `files/upload` with `2024-02-02-preview`

For new code, prefer the newer code-interpreter REST shape and keep API versions configurable in your client code. Details are in [api-notes.md](references/api-notes.md).

## Best Practices

- Keep Entra tokens server-side only. Never expose them to end users.
- Treat session identifiers as sensitive values. Generate them securely and never let users choose arbitrary identifiers for other tenants.
- Prefer `EgressDisabled` unless the workload genuinely needs outbound network access.
- Assume anything inside a session is visible to code running in that session, including files and environment variables.
- Handle session expiration by recreating state when `SessionWithIdentifierNotFound` or equivalent not-found behavior occurs.
- Generate Python for the code interpreter unless a task explicitly asks for another runtime.
- Keep API versions configurable because the Learn articles and data-plane references are still evolving.

## Learn More

| Topic | How to Find |
|---|---|
| Dynamic sessions overview | `npx @microsoft/learn-cli fetch "https://learn.microsoft.com/azure/container-apps/sessions-usage"` |
| Session pools and management endpoint | `npx @microsoft/learn-cli fetch "https://learn.microsoft.com/azure/container-apps/session-pool"` |
| Code interpreter REST usage | `npx @microsoft/learn-cli fetch "https://learn.microsoft.com/azure/container-apps/sessions-code-interpreter"` |
| Delete code-interpreter session | `npx @microsoft/learn-cli fetch "https://learn.microsoft.com/en-us/rest/api/data-plane/containerapps/session-management/delete?view=rest-data-plane-containerapps-2025-10-02-preview&tabs=HTTP"` |
| Data-plane REST operation groups | `npx @microsoft/learn-cli search "Azure Container Apps data plane code execution session resource files"` |
| Session metadata and lifecycle | `npx @microsoft/learn-cli fetch "https://learn.microsoft.com/azure/container-apps/sessions-usage"` |

## CLI Alternative For Learn MCP Tools

| MCP Tool | CLI Command |
|----------|-------------|
| `microsoft_docs_search(query: "...")` | `mslearn search "..."` |
| `microsoft_code_sample_search(query: "...", language: "...")` | `mslearn code-search "..." --language ...` |
| `microsoft_docs_fetch(url: "...")` | `mslearn fetch "..."` |

Run directly with `npx @microsoft/learn-cli <command>` or install globally with `npm install -g @microsoft/learn-cli`.
