# Questionaire

## Software Prerequisites

### SDKs and Coding Tools

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)
- [Node LTS](https://nodejs.org/en/download)
- [pnpm](https://pnpm.io/installation)
- [context7 CLI](https://github.com/mcp/upstash/context7) (`npx ctx7 setup`)
  - `npm install -g ctx7`
- [Aspire CLI](https://aspire.dev/get-started/install-cli/)
  - [Aspire Templates](https://aspire.dev/get-started/aspire-sdk-templates) (`dotnet new install Aspire.ProjectTemplates`) 
- [Angular CLI](https://angular.dev/tools/cli) (`npm install -g @angular/cli`)

## CLIs and Skills

- [Microsoft Learn CLI](https://github.com/mcp/microsoftdocs/mcp) (`npm install -g @microsoft/learn-cli`)
- [Playwright CLI](https://github.com/microsoft/playwright-cli) (`npm install -g @playwright/cli@latest`)
- [Playwright Skills](https://github.com/microsoft/playwright-cli) (`playwright-cli install --skills agents`)
- [Microsoft Learn Skills](https://github.com/mcp/microsoftdocs/mcp)
  - Claude Code: `/plugin install microsoft-docs@claude-plugins-official`
  - GitHub Copilot: `/plugin install microsoftdocs/mcp`
- [Anthropic Skills](https://github.com/anthropics/skills/tree/main/skills)
  - Install using `npx openskills install anthropics/skills --universal`
  - Install the following skills:
    - `doc-coauthoring`
    - `frontend-design`
    - `skill-creator`
- [Angular Skills](https://github.com/angular/skills/tree/main) (`npx openskills install https://github.com/angular/skills.git`)

You can update all skills installed using _openskills_ with `npx openskills update`.

