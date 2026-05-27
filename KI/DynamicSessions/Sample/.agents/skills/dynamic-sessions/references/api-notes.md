# API Notes

These notes capture the Microsoft Learn material verified on 2026-04-23 for Azure Container Apps dynamic sessions code interpreter usage.

## Verified Sources

- `https://learn.microsoft.com/azure/container-apps/sessions-usage`
- `https://learn.microsoft.com/azure/container-apps/session-pool`
- `https://learn.microsoft.com/azure/container-apps/sessions-code-interpreter`
- `https://learn.microsoft.com/cli/azure/containerapp/session/code-interpreter?view=azure-cli-latest`
- `https://learn.microsoft.com/rest/api/data-plane/containerapps/operation-groups?view=rest-data-plane-containerapps-2025-10-02-preview`
- `https://learn.microsoft.com/en-us/rest/api/data-plane/containerapps/session-management/delete?view=rest-data-plane-containerapps-2025-10-02-preview&tabs=HTTP`

## Auth Facts

- Pool management API calls require Microsoft Entra authentication.
- The calling principal needs `Azure ContainerApps Session Executor` on the session pool.
- For Python, the documented token scope is `https://dynamicsessions.io/.default`.
- End users should not receive or handle these bearer tokens directly.

## Session Lifecycle Facts

- A new session is created implicitly the first time a request is sent with a new `identifier`.
- Reusing the same `identifier` reuses the same session.
- `DELETE /session?api-version=2025-10-02-preview&identifier=...` is the documented explicit delete operation for code-interpreter sessions.
- Timed lifecycle is the default session termination model.
- Idle requests reset the cooldown timer.
- Code interpreter docs describe automatic cleanup after inactivity.
- Reusing the same identifier after delete can recreate a fresh session implicitly on the next execution request.

## Current API Shape Guidance

There is a doc mismatch across Learn pages:

- Newer code interpreter docs use:
  - `POST /executions`
  - `POST /files`
  - `GET /files`
  - `GET /files/{filename}`
  - `GET /files/{filename}/content`
  - `api-version=2025-10-02-preview`

- Older session pool docs still show:
  - `POST /code/execute`
  - `POST /files/upload`
  - `GET /files/content/{filename}`
  - `GET /files`
  - `api-version=2024-02-02-preview`

For new code, prefer the newer code interpreter article and the current data-plane operation groups, but keep versions configurable because the service is still evolving.

## Session Management Endpoints

The session-usage article documents:

- `POST /.management/getSession?identifier=...&api-version=2024-02-02-preview`
- `POST /.management/listSessions?skip=0&api-version=2024-02-02-preview`

The data-plane session-management delete operation documents:

- `DELETE /session?identifier=...&api-version=2025-10-02-preview`
- This operation is for the built-in code-interpreter/non-custom session pool.
- The REST docs explicitly note that it is not supported for custom container session pools.

Those pages provide the response shape for session metadata and pagination. If you need to update generated code later, re-check the data-plane REST docs for the newest session-management version before changing these calls.

## File Handling Facts

- Code interpreter file data lives under `/mnt/data`.
- Uploads use `multipart/form-data`.
- The code interpreter article documents a 128 MB upload limit.
- Learn CLI docs also expose Azure CLI helpers for file upload, listing, metadata, download, and delete operations under `az containerapp session code-interpreter ...`.

## Guidance For Agents

- Keep the generated client language-agnostic unless the user explicitly requests a language.
- Assume the executed code is Python because this skill targets the built-in Python code interpreter session pool.
- Model session creation as implicit on first request with a new identifier.
- Prefer `DELETE /session` when a task explicitly needs immediate cleanup of a code-interpreter session.
- Do not use `/.management/stopSession` unless the task is about custom container sessions.
