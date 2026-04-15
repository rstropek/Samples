# Aspire CLI Reference

Install / update the CLI:
```bash
curl -sSL https://aspire.dev/install.sh | bash
aspire --version
```

---

## Lifecycle

### Start in the background (preferred for automation)

```bash
aspire start --project AppHost/AppHost.csproj
```

Returns immediately. Prints dashboard URL, log file path, and PID. The apphost keeps running after the shell exits.

### Run interactively (blocks the terminal)

```bash
aspire run --project AppHost/AppHost.csproj
```

Press `Ctrl+C` to stop everything.

### List running apphosts

```bash
aspire ps
```

### Stop a running apphost

```bash
aspire stop --project AppHost/AppHost.csproj
```

---

## Inspecting resources

### Table view (human-readable)

```bash
aspire describe --apphost AppHost/AppHost.csproj --format Table
```

Shows name, type, state, health, and endpoints for every resource. This is the primary way to get the URLs of running services:

```
┌─────────────┬─────────┬─────────┬─────────┬──────────────────────────────────────────────┐
│ Name        │ Type    │ State   │ Health  │ Endpoints                                    │
├─────────────┼─────────┼─────────┼─────────┼──────────────────────────────────────────────┤
│ weatherapi  │ Project │ Running │ Healthy │ https://localhost:7188, http://localhost:5066 │
└─────────────┴─────────┴─────────┴─────────┴──────────────────────────────────────────────┘
```

### JSON output (scripting / jq)

```bash
aspire describe --apphost AppHost/AppHost.csproj --format Json
```

The JSON shape is `{ "resources": [ { "displayName", "resourceType", "state", "healthStatus", "urls": [{"name", "url"}], ... } ] }`.

Extract all URLs with `jq`:
```bash
aspire describe --apphost AppHost/AppHost.csproj --format Json \
  | jq -r '.resources[].urls[].url'
```

Extract the HTTPS URL of a specific resource:
```bash
aspire describe weatherapi --apphost AppHost/AppHost.csproj --format Json \
  | jq -r '.resources[] | select(.displayName=="weatherapi") | .urls[] | select(.name=="https") | .url'
```

### Single resource

```bash
aspire describe weatherapi --apphost AppHost/AppHost.csproj
```

### Follow / stream state changes

```bash
aspire describe --apphost AppHost/AppHost.csproj --follow
```

Continuously streams updates — useful for watching services come up or go unhealthy.

---

## Logs

### All resources

```bash
aspire logs --apphost AppHost/AppHost.csproj
```

### Single resource

```bash
aspire logs weatherapi --apphost AppHost/AppHost.csproj
```

### Follow (stream)

```bash
aspire logs weatherapi --apphost AppHost/AppHost.csproj --follow
```
