---
name: azure-identity-defaultazurecredential
description: Retrieve Microsoft Entra access tokens with Azure Identity's DefaultAzureCredential. Use when agents need to get a bearer token in Python or TypeScript, choose a tenant explicitly, understand scopes versus resources, or troubleshoot local developer authentication through Azure CLI and other developer credentials.
---

# Azure Identity DefaultAzureCredential

This skill explains how to retrieve Microsoft Entra access tokens with `DefaultAzureCredential` in a way that works across local development and Azure-hosted deployments. It focuses on direct token acquisition, not on using service clients that hide token handling.

## When To Use

- You need a raw bearer token instead of an Azure SDK client.
- You need to acquire a token for a specific tenant.
- You need working token examples for Python or TypeScript.
- You need to understand what scope to pass to `get_token` or `getToken`.
- You need to troubleshoot why local `DefaultAzureCredential` does or does not work.

## Core Ideas

### Credential chain

`DefaultAzureCredential` is a chain. In local development it commonly succeeds through Azure CLI, Azure PowerShell, Azure Developer CLI, or IDE-backed credentials. In Azure-hosted environments it usually succeeds through managed identity or workload identity.

### Scope format

For Microsoft Entra token acquisition, pass OAuth scopes, not resource URIs. If not specified otherwise, for most Azure first-party resources, use:

```text
<resource>/.default
```

Example:

```text
https://management.azure.com/.default
```

### Tenant selection

There are two places tenant selection can matter:

- on the credential configuration itself
- on the token request

For direct token retrieval, the most reliable pattern is:

- create the credential
- request the token with an explicit tenant ID

## Python Pattern

Use [get_token.py](sample_codes/python/get_token.py).

Key point from the Azure Identity Python docs:

- `DefaultAzureCredential.get_token(..., tenant_id=...)` accepts an explicit tenant per token request.

Minimal pattern:

```python
from azure.identity import DefaultAzureCredential

tenant_id = "..."
scope = "https://management.azure.com/.default"

credential = DefaultAzureCredential()
token = credential.get_token(scope, tenant_id=tenant_id)

print(token.token)
```

## TypeScript Pattern

Use [get-token.ts](sample_codes/typescript/get-token.ts).

Key points from the Azure Identity JavaScript docs:

- `DefaultAzureCredentialOptions` supports `tenantId`
- `GetTokenOptions` supports `tenantId`

Minimal pattern:

```ts
import { DefaultAzureCredential } from "@azure/identity";

const tenantId = "...";
const scope = "https://management.azure.com/.default";

const credential = new DefaultAzureCredential({ tenantId });
const token = await credential.getToken(scope, { tenantId });

console.log(token?.token);
```

## Recommended Approach

### Local development

If the user is already signed in with Azure CLI, this is usually enough:

- Python: `DefaultAzureCredential().get_token(scope, tenant_id=tenant_id)`
- TypeScript: `new DefaultAzureCredential({ tenantId }).getToken(scope, { tenantId })`

### Hosted workloads

Prefer keeping `DefaultAzureCredential` with no local-developer assumptions and let managed identity or workload identity satisfy the chain.

## Troubleshooting

### No token returned locally

Check whether the user is signed in:

```bash
az account show
```

### Wrong tenant

Pass the tenant explicitly on the token request, and when supported, on the credential options too.

### Wrong scope

Many Azure resource tokens (NOT all) need `/.default` appended. For example:

- correct: `https://management.azure.com/.default`
- incorrect: `https://management.azure.com/`

### CLI works but SDK fails

That usually means:

- the wrong scope was requested
- the tenant was not specified correctly
- the environment is using a different credential in the chain than expected

## Best Practices

- Prefer raw token retrieval only when you actually need the token string.
- Prefer Azure SDK clients over manual bearer-token plumbing when a client library exists.
- Keep the target tenant explicit if cross-tenant behavior matters.
- Keep scopes explicit and centralized in configuration.
- Never send access tokens to untrusted clients or logs.

## Learn More

| Topic                                   | How to Find                                                                                          |
| --------------------------------------- | ---------------------------------------------------------------------------------------------------- |
| Python `DefaultAzureCredential` API     | `microsoft_docs_search(query="azure identity DefaultAzureCredential python get_token tenant_id")`    |
| JavaScript `DefaultAzureCredential` API | `microsoft_docs_search(query="DefaultAzureCredential TypeScript tenantId getToken @azure/identity")` |
| JavaScript `GetTokenOptions`            | `microsoft_docs_search(query="GetTokenOptions tenantId @azure/identity")`                            |
| Python Azure Identity overview          | `microsoft_docs_search(query="Azure Identity client library for Python DefaultAzureCredential")`     |
| TypeScript Azure Identity samples       | `microsoft_code_sample_search(query="Azure Identity DefaultAzureCredential", language="typescript")` |
