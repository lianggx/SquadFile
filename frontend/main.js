import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './src/router'
import i18n from './src/plugins/i18n'

// 引入本地 iconfont 样式
import './src/assets/iconfont/iconfont.css'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.use(i18n)

app.mount('#app')