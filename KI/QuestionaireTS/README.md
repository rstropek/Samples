## Software Prerequisites

* [Node.js](https://nodejs.org/en)
* [pnpm](https://pnpm.io/installation)
* [`next-devtools-mcp`](https://nextjs.org/docs/app/guides/mcp) ([GitHub Repository](https://github.com/vercel/next-devtools-mcp)): `npx add-mcp next-devtools-mcp@latest`
* [Context7 CLI](https://github.com/upstash/context7): `npx ctx7 setup`
* [pnpm](https://pnpm.io/installation): `npm install -g pnpm`
* [Install Playwright CLI](https://github.com/microsoft/playwright-cli?tab=readme-ov-file#installation)

## Tests for exec policies

```sh
codex execpolicy check --pretty \
  --rules ./.codex/rules/playwright.rules \
  -- playwright-cli open https://orf.at/
```
