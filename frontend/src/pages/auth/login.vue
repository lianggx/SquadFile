<template>
  <div class="login-container">
    <div class="login-header">
      <img 
        :src="systemConfig.loginLogoPath" 
        :alt="systemConfig.siteName" 
        class="login-logo"
        @error="handleLogoError"
      />
      <h1 class="site-name">{{ systemConfig.siteName }}</h1>
    </div>
    
    <!-- 将语言选择器移到登录表单区域之外 -->
    <div class="language-selector">
      <select v-model="selectedLanguage" @change="changeLanguage" class="language-dropdown">
        <option value="zh-Hans">中文</option>
        <option value="en-US">English</option>
      </select>
    </div>
    
    <div class="login-form">
     
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="username">{{ t('login.username') }}</label>
          <input 
            id="username" 
            v-model="loginForm.username" 
            type="text" 
            :placeholder="t('login.usernamePlaceholder')" 
            class="login-input"
            required
          />
        </div>
        <div class="form-group">
          <label for="password">{{ t('login.password') }}</label>
          <input 
            id="password" 
            v-model="loginForm.password" 
            type="password" 
            :placeholder="t('login.passwordPlaceholder')" 
            class="login-input"
            required
          />
        </div>
        <!-- 验证码部分 -->
        <div class="form-group captcha-group">
          <label for="captcha">{{ t('login.captcha') }}</label>
          <div class="captcha-container">
            <input 
              id="captcha" 
              v-model="loginForm.captcha" 
              type="text" 
              :placeholder="t('login.captchaPlaceholder')" 
              class="captcha-input login-input"
              required
            />
            <img 
              :src="captchaImage" 
              :alt="t('login.captchaAlt')" 
              class="captcha-image"
              @click="refreshCaptcha"
            />
          </div>
        </div>
        <button 
          type="submit" 
          class="login-button"
          :disabled="isLoading"
        >
          {{ isLoading ? t('login.loggingIn') : t('login.loginButton') }}
        </button>
      </form>
      <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'
import { useI18n } from 'vue-i18n'
import { getSystemConfigApi } from '../../services/system'
import { getCaptchaImageApi } from '../../services/system'

const loginForm = ref({
  username: '',
  password: '',
  captcha: '',
  captchaId: '' // 验证码ID字段
})

const errorMessage = ref('')
const selectedLanguage = ref('zh-Hans')
const systemConfig = ref({
  siteName: '小队快传',
  loginLogoPath: '/static/images/logo.png'
})

// 添加加载状态
const isLoading = ref(false)

// 验证码图片base64数据
const captchaImage = ref('')

const authStore = useAuthStore()
const router = useRouter()
const { locale, t } = useI18n()

// 处理LOGO加载错误
const handleLogoError = (event) => {
  event.target.src = '/static/images/logo.png'
}

// 刷新验证码
const refreshCaptcha = async () => {
  try {
    // 获取新的验证码图片和ID
    const response = await getCaptchaImageApi()
    // 注意：response.data是完整的响应数据，包含code, message, data
    // 我们需要的是response.data.data，其中包含id和image
    if (response && response.data && response.data.data) {
      loginForm.value.captchaId = response.data.data.id
      captchaImage.value = response.data.data.image
    } else {
      // 如果无法获取验证码，清空数据
      loginForm.value.captchaId = ''
      captchaImage.value = ''
    }
  } catch (error) {
    // 出错时清空数据
    loginForm.value.captchaId = ''
    captchaImage.value = ''
  }
}

// 获取系统配置
const fetchSystemConfig = async () => {
  try {
    const response = await getSystemConfigApi()
    if (response && response.data) {
      systemConfig.value = {
        siteName: response.data.data.siteName || '小队快传',
        loginLogoPath: response.data.data.loginLogoPath || '/static/images/logo.png'
      }
    }
  } catch (error) {
    // 使用默认配置
    systemConfig.value = {
      siteName: '小队快传',
      loginLogoPath: '/static/images/logo.png'
    }
  }
}

// 语言切换功能
const changeLanguage = () => {
  locale.value = selectedLanguage.value
  // 保存用户语言偏好到localStorage
  localStorage.setItem('preferredLanguage', selectedLanguage.value)
}

// 页面加载时获取用户语言偏好和系统配置
onMounted(async () => {
  // 获取系统配置
  await fetchSystemConfig()
  
  // 获取用户语言偏好
  const savedLanguage = localStorage.getItem('preferredLanguage')
  if (savedLanguage) {
    selectedLanguage.value = savedLanguage
    locale.value = savedLanguage
  }
  
  // 获取验证码
  await refreshCaptcha()
})

const handleLogin = async () => {  
  // 重置错误信息
  errorMessage.value = ''
  
  // 基本验证
  if (!loginForm.value.username.trim()) {
    errorMessage.value = t('login.usernameRequired')
    return
  }
  
  if (!loginForm.value.password.trim()) {
    errorMessage.value = t('login.passwordRequired')
    return
  }
  
  if (!loginForm.value.captcha.trim()) {
    errorMessage.value = t('login.captchaRequired')
    return
  }
  
  // 确保验证码ID存在
  if (!loginForm.value.captchaId) {
    errorMessage.value = t('login.captchaError')
    refreshCaptcha()
    return
  }
  
  try {    
    // 设置加载状态
    isLoading.value = true
    
    await authStore.login(loginForm.value)
    
    // 登录成功后跳转到首页
    router.push('/')
  } catch (error) {
    // 处理错误消息
    if (error.message) {
      // 检查是否是本地化键
      if (error.message.startsWith('login.')) {
        errorMessage.value = t(error.message)
      } else {
        // 直接显示错误消息
        errorMessage.value = error.message
      }
    } else {
      errorMessage.value = t('login.loginFailed')
    }
    // 登录失败时刷新验证码
    refreshCaptcha()
  } finally {
    // 重置加载状态
    isLoading.value = false
  }
}
</script>

<style scoped>
.login-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  padding: 20px;
  position: relative;
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-logo {
  height: 80px;
  width: 80px;
  object-fit: cover;
  margin: 0 auto 15px;
  display: block;
}

.site-name {
  font-size: 32px;
  font-weight: 700;
  color: #333;
  margin: 0 0 20px 0;
  user-select: none;
}

/* 添加语言选择器样式 */
.language-selector {
  position: absolute;
  top: 20px;
  right: 20px;
  z-index: 100;
}

.language-dropdown {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  background-color: white;
  font-size: 14px;
  cursor: pointer;
  outline: none;
  transition: border-color 0.3s ease;
  appearance: none;
  background-image: url("data:image/svg+xml;charset=UTF-8,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%23333' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3e%3cpolyline points='6 9 12 15 18 9'%3e%3c/polyline%3e%3c/svg%3e");
  background-repeat: no-repeat;
  background-position: right 8px center;
  background-size: 12px;
  padding-right: 30px;
}

.language-dropdown:focus {
  border-color: #667eea;
}

/* 添加移动端媒体查询 */
@media (max-width: 768px) {
  .login-form {
    max-width: 90%;
    padding: 30px 20px;
    margin: 0 auto 30px;
  }
  
  .login-title {
    font-size: 24px;
  }
  
  .login-logo {
    height: 60px;
    width: 60px;
  }
  
  .site-name {
    font-size: 28px;
  }
}

/* 添加更小屏幕的适配 */
@media (max-width: 480px) {
  .login-form {
    max-width: 95%;
    padding: 20px 15px;
    margin: 0 auto 20px;
  }
  
  .login-title {
    font-size: 20px;
    margin-bottom: 20px;
  }
  
  .form-group {
    margin-bottom: 20px;
  }
  
  .login-input {
    padding: 12px;
    font-size: 16px;
  }
  
  .login-button {
    padding: 12px;
    font-size: 16px;
  }
  
  .login-logo {
    height: 50px;
    width: 50px;
  }
  
  .site-name {
    font-size: 24px;
  }
  
  .language-dropdown {
    padding: 6px 10px;
    font-size: 13px;
    padding-right: 25px;
  }
}

.login-title {
  text-align: center;
  margin-bottom: 30px;
  color: #333;
  font-size: 28px;
  font-weight: 600;
  user-select: none;
}

.form-group {
  margin-bottom: 25px;
}

label {
  display: block;
  margin-bottom: 8px;
  color: #555;
  font-weight: 500;
  font-size: 14px;
  user-select: none;
}

.login-input {
  width: 100%;
  padding: 15px;
  border: 2px solid #e1e1e1;
  border-radius: 8px;
  font-size: 16px;
  transition: all 0.3s ease;
  background-color: #fff;
  color: #333;
  outline: none;
}

.login-input:focus {
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.login-input::placeholder {
  color: #aaa;
}

/* 验证码样式 */
.captcha-group {
  margin-bottom: 20px;
}

.captcha-container {
  display: flex;
  align-items: center;
  gap: 10px;
}

.captcha-input {
  flex: 1;
  margin-bottom: 0;
}

.captcha-image {
  height: 40px;
  width: 120px;
  border: 1px solid #e1e1e1;
  border-radius: 4px;
  cursor: pointer;
  user-select: none;
}

.captcha-image:hover {
  opacity: 0.8;
}

.login-button {
  width: 100%;
  padding: 15px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 16px;
  font-weight: 600;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.login-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.login-button:active {
  transform: translateY(0);
}

.login-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

.error-message {
  color: #e74c3c;
  margin-top: 20px;
  text-align: center;
  font-size: 14px;
  padding: 10px;
  border-radius: 5px;
  background-color: rgba(231, 76, 60, 0.1);
  user-select: none;
}
</style>