<template>
  <div class="change-password-container">
    <!-- 添加 Toast 组件 -->
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <h1 class="title">{{ t('changePassword.title') }}</h1>
    
    <div class="change-password-form">
      <div class="form-group">
        <label class="label">{{ t('changePassword.oldPassword') }}</label>
        <input 
          v-model="passwords.oldPassword" 
          type="password" 
          class="input" 
          :placeholder="t('changePassword.enterOldPassword')"
        />
      </div>
      
      <div class="form-group">
        <label class="label">{{ t('changePassword.newPassword') }}</label>
        <input 
          v-model="passwords.newPassword" 
          type="password" 
          class="input" 
          :placeholder="t('changePassword.enterNewPassword')"
        />
      </div>
      
      <div class="form-group">
        <label class="label">{{ t('changePassword.confirmPassword') }}</label>
        <input 
          v-model="passwords.confirmPassword" 
          type="password" 
          class="input" 
          :placeholder="t('changePassword.enterConfirmPassword')"
        />
      </div>
      
      <div class="form-actions">
        <button @click="changePassword" class="change-button">
          {{ t('changePassword.change') }}
        </button>
        <button @click="cancel" class="cancel-button">
          {{ t('changePassword.cancel') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'
import { usePageAuth } from '../../composables/useAuth'
// 导入 Toast 组件
import Toast from '../../components/Toast.vue'

const { t } = useI18n()
const router = useRouter()
const authStore = useAuthStore()
const { checkAuthOnMounted } = usePageAuth()

// 页面加载时检查认证状态
checkAuthOnMounted(false)

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 密码数据
const passwords = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message
  toastType.value = type
}

// 修改密码
const changePassword = async () => {
  // 验证密码
  if (!passwords.value.oldPassword) {
    showToast(t('changePassword.enterOldPassword'), 'error')
    return
  }
  
  if (!passwords.value.newPassword) {
    showToast(t('changePassword.enterNewPassword'), 'error')
    return
  }
  
  if (passwords.value.newPassword !== passwords.value.confirmPassword) {
    showToast(t('changePassword.passwordMismatch'), 'error')
    return
  }
  
  try {
    // 调用后端API修改密码
    await authStore.changePassword(passwords.value)
    
    showToast(t('changePassword.passwordChangeSuccess'), 'success')
    setTimeout(() => {
      router.push('/profile')
    }, 1500)
  } catch (error) {
    console.error('修改密码失败:', error)
    showToast(t('changePassword.passwordChangeFailed'), 'error')
  }
}

// 取消修改
const cancel = () => {
  router.push('/profile')
}
</script>

<style scoped>
.change-password-container {
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

.change-password-form {
  background-color: white;
  border-radius: 15px;
  padding: 25px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  max-width: 500px;
  margin: 0 auto;
  width: 100%;
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

.form-actions {
  display: flex;
  gap: 15px;
  margin-top: 30px;
}

.change-button,
.cancel-button {
  flex: 1;
  padding: 15px;
  border: none;
  border-radius: 10px;
  font-size: 16px;
  cursor: pointer;
}

.change-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.cancel-button {
  background-color: #6c757d;
  color: white;
}

.change-button:hover,
.cancel-button:hover {
  opacity: 0.9;
  transform: translateY(-2px);
}
</style>

/* 响应式设计 */
@media (max-width: 768px) {
  .change-password-container {
    padding: 15px;
  }
  
  .title {
    font-size: 22px;
  }
  
  .change-password-form {
    padding: 20px;
  }
  
  .form-group {
    margin-bottom: 20px;
  }
  
  .label {
    font-size: 14px;
  }
  
  .input {
    padding: 12px;
    font-size: 14px;
  }
  
  .form-actions {
    gap: 10px;
    margin-top: 20px;
  }
  
  .change-button,
  .cancel-button {
    padding: 12px;
    font-size: 14px;
  }
}

@media (max-width: 480px) {
  .change-password-container {
    padding: 10px;
  }
  
  .title {
    font-size: 20px;
  }
  
  .change-password-form {
    padding: 15px;
  }
  
  .form-group {
    margin-bottom: 15px;
  }
  
  .label {
    font-size: 13px;
  }
  
  .input {
    padding: 10px;
    font-size: 13px;
  }
  
  .change-button,
  .cancel-button {
    padding: 10px;
    font-size: 13px;
  }
}
