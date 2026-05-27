# Code Interpreter REST Flow

This sample is language-agnostic on the client side. It shows the HTTP contract that any client can implement. The code sent to the session is Python.

## 1. Get the pool management endpoint

```bash
POOL_MANAGEMENT_ENDPOINT=$(az containerapp sessionpool show \
  --name <SESSION_POOL_NAME> \
  --resource-group <RESOURCE_GROUP> \
  --query "properties.poolManagementEndpoint" \
  --output tsv)
```

## 2. Acquire a Microsoft Entra token

```bash
TOKEN=$(az account get-access-token \
  --resource https://dynamicsessions.io \
  --query accessToken \
  --output tsv)
```

## 3. Execute Python in a session

The first request with a new `identifier` implicitly creates the session.

```http
POST <POOL_MANAGEMENT_ENDPOINT>/executions?api-version=2025-10-02-preview&identifier=<SESSION_ID>
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "properties": {
    "codeInputType": "inline",
    "executionType": "synchronous",
    "code": "print('hello from python')"
  }
}
```

`curl` form:

```bash
curl -X POST \
  "$POOL_MANAGEMENT_ENDPOINT/executions?api-version=2025-10-02-preview&identifier=<SESSION_ID>" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "properties": {
      "codeInputType": "inline",
      "executionType": "synchronous",
      "code": "print(\"hello from python\")"
    }
  }'
```

## 4. Upload an input file

Uploaded files land in `/mnt/data`.

```bash
curl -X POST \
  "$POOL_MANAGEMENT_ENDPOINT/files?api-version=2025-10-02-preview&identifier=<SESSION_ID>" \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@input.csv"
```

## 5. Execute Python that reads and writes files

```http
POST <POOL_MANAGEMENT_ENDPOINT>/executions?api-version=2025-10-02-preview&identifier=<SESSION_ID>
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "properties": {
    "codeInputType": "inline",
    "executionType": "synchronous",
    "code": "from pathlib import Path\nraw = Path(\"/mnt/data/input.csv\").read_text()\nPath(\"/mnt/data/output.txt\").write_text(raw.upper())\nprint(\"done\")"
  }
}
```

## 6. List files

```bash
curl -X GET \
  "$POOL_MANAGEMENT_ENDPOINT/files?api-version=2025-10-02-preview&identifier=<SESSION_ID>" \
  -H "Authorization: Bearer $TOKEN"
```

## 7. Get file metadata

```bash
curl -X GET \
  "$POOL_MANAGEMENT_ENDPOINT/files/output.txt?api-version=2025-10-02-preview&identifier=<SESSION_ID>" \
  -H "Authorization: Bearer $TOKEN"
```

## 8. Download a generated file

```bash
curl -X GET \
  "$POOL_MANAGEMENT_ENDPOINT/files/output.txt/content?api-version=2025-10-02-preview&identifier=<SESSION_ID>" \
  -H "Authorization: Bearer $TOKEN" \
  -o output.txt
```

## 9. Inspect session metadata

```bash
curl -X POST \
  "$POOL_MANAGEMENT_ENDPOINT/.management/getSession?identifier=<SESSION_ID>&api-version=2024-02-02-preview" \
  -H "Authorization: Bearer $TOKEN"
```

## 10. List active sessions

```bash
curl -X POST \
  "$POOL_MANAGEMENT_ENDPOINT/.management/listSessions?skip=0&api-version=2024-02-02-preview" \
  -H "Authorization: Bearer $TOKEN"
```

## 11. Delete the code-interpreter session

```bash
curl -X DELETE \
  "$POOL_MANAGEMENT_ENDPOINT/session?api-version=2025-10-02-preview&identifier=<SESSION_ID>" \
  -H "Authorization: Bearer $TOKEN"
```

## Notes

- Reuse the same `identifier` to keep state, imports, and files within the same session.
- There is no separate documented create call for code interpreter sessions.
- `DELETE /session` is the documented explicit cleanup call for code-interpreter sessions.
- A later execution request with the same `identifier` can create a fresh session again.
- `POST /.management/stopSession` is the custom-container stop operation, not the built-in code-interpreter session delete call.
