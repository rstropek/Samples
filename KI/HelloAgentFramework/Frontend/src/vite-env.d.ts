/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Base URL of the AG-UI agent, injected by the Aspire AppHost. */
  readonly VITE_AGENT_URL?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
