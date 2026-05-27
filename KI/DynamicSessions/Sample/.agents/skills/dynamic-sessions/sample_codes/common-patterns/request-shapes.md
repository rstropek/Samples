# Request Shapes

Use these shapes when generating client code in any programming language.

## Required Headers

```http
Authorization: Bearer <TOKEN>
```

Add `Content-Type: application/json` for JSON POST requests.

## Execute Python Code

```json
{
  "properties": {
    "codeInputType": "inline",
    "executionType": "synchronous",
    "code": "print('hello')"
  }
}
```

## Upload File

- Method: `POST`
- Path: `/files`
- Encoding: `multipart/form-data`
- Form field name: `file`

## List Files

- Method: `GET`
- Path: `/files`

## File Metadata

- Method: `GET`
- Path: `/files/{filename}`

## Download File Content

- Method: `GET`
- Path: `/files/{filename}/content`

## Get Session Metadata

- Method: `POST`
- Path: `/.management/getSession`
- Query: `identifier=<SESSION_ID>&api-version=2024-02-02-preview`

## List Sessions

- Method: `POST`
- Path: `/.management/listSessions`
- Query: `skip=<OFFSET>&api-version=2024-02-02-preview`

## Identifier Rules

- Required for per-session operations.
- Reuse the same identifier to keep working in the same session.
- Use a new identifier to create a new session implicitly.
- Treat identifiers as sensitive and application-controlled.
