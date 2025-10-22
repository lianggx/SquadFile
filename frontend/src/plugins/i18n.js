import { createI18n } from 'vue-i18n'
import zhHans from '../locales/zh-Hans.json'
import enUS from '../locales/en-US.json'

const messages = {
  'zh-Hans': zhHans,
  'en-US': enUS
}

// 从localStorage获取保存的语言，如果没有则使用默认语言
const savedLanguage = localStorage.getItem('preferredLanguage') || 'zh-Hans'

const i18n = createI18n({
  legacy: false, // 使用Composition API模式
  locale: savedLanguage,
  fallbackLocale: 'zh-Hans',
  messages
})

export default i18n