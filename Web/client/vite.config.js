import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  clearScreen: false,
  root: './src',
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5010'
      }
    },
  },
  build: {
    outDir: '../wwwroot',
  }
})