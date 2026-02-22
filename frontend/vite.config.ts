import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
  server: {
    port: 5173,
    // Since the API is on https://localhost:5001, we might need HTTPS for some auth flows
    // but MSAL popup usually works fine on HTTP localhost.
  }
})
