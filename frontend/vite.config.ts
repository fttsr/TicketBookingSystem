import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

export default defineConfig(({ command }) => ({
  base: command === 'build' ? '/TicketBookingSystem/' : '/',
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'https://nearest-gives-stars-growing.trycloudflare.com',
        changeOrigin: true,
      },
    },
  },
}))
