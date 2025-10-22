<template>
  <div class="settings-container">
    <!-- 添加 Toast 组件 -->
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <h1 class="title">{{ t('admin.systemSettings') }}</h1>
    
    <div class="settings-form">
      <div class="form-group">
        <label class="label">{{ t('admin.siteName') }}</label>
        <input 
          v-model="settings.siteName" 
          class="input" 
          :placeholder="t('admin.siteNamePlaceholder')"
        />
      </div>
      
      <!-- 登录页LOGO上传 -->
      <div class="form-group">
        <label class="label">{{ t('admin.loginLogo') }}</label>
        <div class="logo-upload-container">
          <div class="logo-preview">
            <img 
              :src="settings.loginLogoPath" 
              :alt="t('admin.loginLogo')" 
              class="logo-image"
              @error="handleLogoError"
            />
            <p class="logo-description">{{ t('admin.loginLogoDesc') }}</p>
          </div>
          <div class="logo-upload-controls">
            <input 
              ref="loginLogoInput"
              type="file" 
              accept="image/*" 
              @change="handleLoginLogoChange"
              style="display: none;"
            />
            <button type="button" @click="triggerLoginLogoSelect" class="upload-button">
              {{ t('admin.uploadLogo') }}
            </button>
          </div>
        </div>
      </div>
      
      <!-- 首页LOGO上传 -->
      <div class="form-group">
        <label class="label">{{ t('admin.homeLogo') }}</label>
        <div class="logo-upload-container">
          <div class="logo-preview">
            <img 
              :src="settings.homeLogoPath" 
              :alt="t('admin.homeLogo')" 
              class="logo-image"
              @error="handleLogoError"
            />
            <p class="logo-description">{{ t('admin.homeLogoDesc') }}</p>
          </div>
          <div class="logo-upload-controls">
            <input 
              ref="homeLogoInput"
              type="file" 
              accept="image/*" 
              @change="handleHomeLogoChange"
              style="display: none;"
            />
            <button type="button" @click="triggerHomeLogoSelect" class="upload-button">
              {{ t('admin.uploadLogo') }}
            </button>
          </div>
        </div>
      </div>
      
      <div class="form-group">
        <label class="label">{{ t('admin.maxFileSize') }}</label>
        <input 
          v-model="settings.maxFileSize" 
          type="number" 
          class="input" 
          :placeholder="t('admin.maxFileSizePlaceholder')"
        />
      </div>
      
      <div class="form-group">
        <label class="label">{{ t('admin.storageLimit') }}</label>
        <input 
          v-model="settings.storageLimit" 
          type="number" 
          class="input" 
          :placeholder="t('admin.storageLimitPlaceholder')"
        />
      </div>
      
      <div class="form-group">
        <label class="label">{{ t('admin.defaultLanguage') }}</label>
        <select 
          v-model="settings.defaultLanguage"
          class="select"
        >
          <option 
            v-for="lang in languages" 
            :key="lang.value" 
            :value="lang.value"
          >
            {{ lang.name }}
          </option>
        </select>
      </div>
            
      <div class="form-actions">
        <button @click="saveSettings" class="save-button">
          {{ t('admin.saveSettings') }}
        </button>
        <button @click="resetSettings" class="reset-button">
          {{ t('admin.resetSettings') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { usePageAuth } from '../../composables/useAuth'
import { getSystemConfigApi, updateSystemConfigApi, uploadLogoApi } from '../../services/system'

// 导入 Toast 组件
import Toast from '../../components/Toast.vue'

const { t } = useI18n()
const router = useRouter()
const { checkAuthOnMounted } = usePageAuth()

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 页面加载时检查认证状态
checkAuthOnMounted(false)

// 系统设置数据
const settings = ref({
  siteName: '',
  loginLogoPath: '/static/images/logo.png',
  homeLogoPath: '/static/images/logo.png',
  maxFileSize: 0,
  storageLimit: 0,
  defaultLanguage: 'zh-Hans'
})

// 语言选项
const languages = ref([
  { value: 'zh-Hans', name: '中文' },
  { value: 'en-US', name: 'English' }
])

// Logo文件输入引用
const loginLogoInput = ref(null)
const homeLogoInput = ref(null)

// 页面加载时获取系统设置
onMounted(() => {
  fetchSettings()
})

// 获取系统设置
const fetchSettings = async () => {
  try {
    const response = await getSystemConfigApi()
    // 根据API响应结构调整，正确提取数据
    const data = response.data.data || response.data || {}
    
    settings.value = {
      siteName: data.siteName || '小队快传',
      loginLogoPath: data.loginLogoPath || '/static/images/logo.png',
      homeLogoPath: data.homeLogoPath || '/static/images/logo.png',
      maxFileSize: data.maxFileSize || 100,
      storageLimit: data.storageLimit || 10240,
      defaultLanguage: data.defaultLanguage || 'zh-Hans'
    }
  } catch (error) {
    showToast(t('admin.fetchSettingsFailed'), 'error')
  }
}

// 处理LOGO加载错误
const handleLogoError = (event) => {
  event.target.src = '/static/images/logo.png'
}

// 触发登录页LOGO选择
const triggerLoginLogoSelect = () => {
  loginLogoInput.value?.click()
}

// 触发首页LOGO选择
const triggerHomeLogoSelect = () => {
  homeLogoInput.value?.click()
}

// 处理登录页LOGO变更
const handleLoginLogoChange = async (event) => {
  const file = event.target.files[0]
  if (file) {
    await uploadLogo(file, 'login')
  }
}

// 处理首页LOGO变更
const handleHomeLogoChange = async (event) => {
  const file = event.target.files[0]
  if (file) {
    await uploadLogo(file, 'home')
  }
}

// 上传Logo
const uploadLogo = async (file, type) => {
  try {
    const formData = new FormData()
    formData.append('logo', file)
    formData.append('type', type) // 'login' 或 'home'
    
    const response = await uploadLogoApi(formData)
    
    if (response.data.code === 200) {
      // 更新对应的Logo路径
      if (type === 'login') {
        settings.value.loginLogoPath = response.data.data.logoPath
      } else {
        settings.value.homeLogoPath = response.data.data.logoPath
      }
      
      showToast(t('admin.logoUploadSuccess'), 'success')
    } else {
      showToast(t('admin.logoUploadFailed'), 'error')
    }
  } catch (error) {
    showToast(t('admin.logoUploadFailed'), 'error')
  }
}

// 保存设置
const saveSettings = async () => {
  try {
    // 准备要发送的数据
    const configData = {
      siteName: settings.value.siteName,
      loginLogoPath: settings.value.loginLogoPath,
      homeLogoPath: settings.value.homeLogoPath,
      maxFileSize: settings.value.maxFileSize,
      storageLimit: settings.value.storageLimit,
      defaultLanguage: settings.value.defaultLanguage
    }
    
    // 调用后端API保存设置
    await updateSystemConfigApi(configData)
    
    showToast(t('admin.settingsSaved'), 'success')
  } catch (error) {
    console.error('保存系统设置失败:', error)
    showToast(t('admin.saveSettingsFailed'), 'error')
  }
}

// 重置设置
const resetSettings = () => {
  if (confirm(`${t('admin.confirmReset')}\n${t('admin.confirmResetContent')}`)) {
    fetchSettings()
    showToast(t('admin.settingsReset'), 'success')
  }
}

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message
  toastType.value = type
}
</script>

<style scoped>
.settings-container {
  display: flex;
  flex-direction: column;
  padding: 20px;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: 100vh;
}

.title {
  font-size: 24px;
  font-weight: bold;
  color: #333;
  margin-bottom: 30px;
  text-align: center;
}

.settings-form {
  background-color: white;
  border-radius: 15px;
  padding: 25px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  max-width: 800px;
  width: 100%;
  margin: 0 auto;
  box-sizing: border-box;
}

.form-group {
  margin-bottom: 25px;
}

.form-group:last-child {
  margin-bottom: 0;
}

.label {
  display: block;
  margin-bottom: 10px;
  font-weight: bold;
  color: #555;
  font-size: 16px;
}

.input {
  width: 100%;
  padding: 15px;
  border: 1px solid #e1e1e1;
  border-radius: 10px;
  font-size: 16px;
  box-sizing: border-box;
}

.select {
  width: 100%;
  padding: 15px;
  border: 1px solid #e1e1e1;
  border-radius: 10px;
  font-size: 16px;
  background-color: white;
  box-sizing: border-box;
}

.logo-upload-container {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.logo-preview {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
}

.logo-image {
  width: 80px;
  height: 80px;
  object-fit: contain;
  border: 1px solid #e1e1e1;
  border-radius: 8px;
  padding: 5px;
}

.logo-description {
  font-size: 14px;
  color: #666;
  text-align: center;
  margin: 0;
}

.logo-upload-controls {
  display: flex;
  gap: 10px;
  justify-content: center;
}

.upload-button,
.reset-button {
  padding: 8px 12px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
}

.upload-button {
  background-color: #1890ff;
  color: white;
}

.reset-button {
  background-color: #ff4d4f;
  color: white;
}

.upload-button:hover,
.reset-button:hover {
  opacity: 0.8;
}

.form-actions {
  display: flex;
  gap: 15px;
  margin-top: 30px;
}

.save-button,
.reset-button {
  flex: 1;
  padding: 15px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 16px;
  cursor: pointer;
  box-shadow: 0 2px 10px rgba(102, 126, 234, 0.3);
  min-width: 120px;
}

.reset-button {
  background-color: #ff4d4f;
  box-shadow: 0 2px 10px rgba(255, 77, 79, 0.3);
}

.save-button:hover,
.reset-button:hover {
  opacity: 0.9;
  transform: translateY(-2px);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .settings-container {
    padding: 15px;
  }
  
  .title {
    font-size: 22px;
  }
  
  .settings-form {
    padding: 20px;
    max-width: 100%;
    width: 100%;
  }
  
  .input,
  .select {
    padding: 12px;
    font-size: 14px;
  }
  
  .logo-image {
    width: 60px;
    height: 60px;
  }
  
  .form-actions {
    flex-direction: column;
    gap: 10px;
  }
  
  .save-button,
  .reset-button {
    padding: 12px;
    font-size: 14px;
    width: 100%;
  }
}

@media (max-width: 480px) {
  .settings-container {
    padding: 10px;
  }
  
  .title {
    font-size: 20px;
  }
  
  .settings-form {
    padding: 15px;
    max-width: 100%;
    width: 100%;
  }
  
  .form-group {
    margin-bottom: 20px;
  }
  
  .label {
    font-size: 14px;
  }
  
  .input,
  .select {
    padding: 10px;
    font-size: 13px;
  }
  
  .logo-image {
    width: 50px;
    height: 50px;
  }
  
  .logo-upload-controls {
    flex-direction: column;
    align-items: center;
  }
  
  .upload-button,
  .reset-button {
    width: 100%;
    padding: 10px;
    font-size: 13px;
  }
  
  .save-button,
  .reset-button {
    padding: 10px;
    font-size: 13px;
  }
}
</style>