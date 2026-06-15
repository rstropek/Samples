import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// When launched by the Aspire AppHost, the port is provided via the PORT env var.
// Falls back to 5173 for a plain `npm run dev`.
// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    host: true,
    port: Number(process.env.PORT) || 5173,
  },
})
