import { defineShikiSetup } from '@slidev/types'

// High-contrast light theme for projectors.
export default defineShikiSetup(() => ({
  themes: {
    dark: 'github-dark',
    light: 'github-light',
  },
}))
