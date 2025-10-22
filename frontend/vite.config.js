import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    port: 8168,
    proxy: {
      '/squadfile-api': {
        target: 'http://localhost:8169',
        changeOrigin: true,
        secure: false
      },
      '/auth': {
        target: 'http://localhost:8169',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/auth/, '/squadfile-api/Auth')
      },
      '/user': {
        target: 'http://localhost:8169',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/user/, '/squadfile-api/User')
      },
      '/captcha': {
        target: 'http://localhost:8169',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/captcha/, '/squadfile-api/Captcha')
      },
      '/system': {
        target: 'http://localhost:8169',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/system/, '/squadfile-api/System')
      },
      '/filemanagement': {
        target: 'http://localhost:8169',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/filemanagement/, '/squadfile-api/filemanagement')
      },
      '/fileshare': {
        target: 'http://localhost:8169',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/fileshare/, '/squadfile-api/fileshare')
      },
      // 添加对/files路径的代理，用于文件下载
      '/files': {
        target: 'http://localhost:8169',
        changeOrigin: true,
        secure: false
      }
    }
  }
})