<template>
  <div class="edit-profile-container">
    <h1 class="title">{{ t('editProfile.title') }}</h1>
    
    <div class="edit-profile-form">
      <div class="form-group">
        <label class="label">{{ t('editProfile.username') }}</label>
        <input 
          v-model="profile.username" 
          class="input" 
          disabled
        />
        <p class="hint">{{ t('editProfile.usernameHint') }}</p>
      </div>
      
      <div class="form-group">
        <label class="label">{{ t('editProfile.email') }}</label>
        <input 
          v-model="profile.email" 
          type="email" 
          class="input" 
          :placeholder="t('editProfile.enterEmail')"
        />
      </div>
      
      <div class="form-group">
        <label class="label">{{ t('editProfile.displayName') }}</label>
        <input 
          v-model="profile.displayName" 
          class="input" 
          :placeholder="t('editProfile.enterDisplayName')"
        />
      </div>
      
      <div class="form-group">
        <label class="label">{{ t('editProfile.department') }}</label>
        <input 
          v-model="profile.department" 
          class="input" 
          :placeholder="t('editProfile.enterDepartment')"
        />
      </div>
      
      <div class="form-actions">
        <button @click="saveProfile" class="save-button">
          {{ t('editProfile.save') }}
        </button>
        <button @click="cancel" class="cancel-button">
          {{ t('editProfile.cancel') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'
import { usePageAuth } from '../../composables/useAuth'
import { updateProfileApi } from '../../services/auth'

const { t } = useI18n()
const router = useRouter()
const authStore = useAuthStore()
const { checkAuthOnMounted } = usePageAuth()

// 页面加载时检查认证状态
checkAuthOnMounted(false)

// 用户资料数据
const profile = ref({
  username: '',
  email: '',
  displayName: '',
  department: ''
})

// 页面加载时获取用户资料
onMounted(() => {
  if (authStore.user) {
    profile.value = {
      username: authStore.user.username,
      email: authStore.user.email || '',
      displayName: authStore.user.displayName || '',
      department: authStore.user.department || ''
    }
  }
})

// 保存资料
const saveProfile = async () => {
  try {
    // 准备要发送的数据
    const profileData = {
      email: profile.value.email,
      displayName: profile.value.displayName,
      department: profile.value.department
    }
    
    // 调用后端API保存资料
    await authStore.updateProfile(profileData)
    
    alert(t('editProfile.profileUpdateSuccess'))
    router.push('/profile')
  } catch (error) {
    console.error('保存资料失败:', error)
    alert(t('editProfile.profileUpdateFailed'))
  }
}

// 取消编辑
const cancel = () => {
  router.push('/profile')
}
</script>

<style scoped>
.edit-profile-container {
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

.edit-profile-form {
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

.input:disabled {
  background-color: #f5f5f5;
  color: #999;
}

.hint {
  font-size: 12px;
  color: #999;
  margin-top: 5px;
}

.form-actions {
  display: flex;
  gap: 15px;
  margin-top: 30px;
}

.save-button,
.cancel-button {
  flex: 1;
  padding: 15px;
  border: none;
  border-radius: 10px;
  font-size: 16px;
  cursor: pointer;
}

.save-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.cancel-button {
  background-color: #6c757d;
  color: white;
}

.save-button:hover,
.cancel-button:hover {
  opacity: 0.9;
  transform: translateY(-2px);
}
</style>
/* 响应式设计 */
@media (max-width: 768px) {
  .edit-profile-container {
    padding: 15px;
  }
  
  .title {
    font-size: 22px;
  }
  
  .edit-profile-form {
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
  
  .hint {
    font-size: 11px;
  }
  
  .form-actions {
    gap: 10px;
    margin-top: 20px;
  }
  
  .save-button,
  .cancel-button {
    padding: 12px;
    font-size: 14px;
  }
}

@media (max-width: 480px) {
  .edit-profile-container {
    padding: 10px;
  }
  
  .title {
    font-size: 20px;
  }
  
  .edit-profile-form {
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
  
  .hint {
    font-size: 10px;
  }
  
  .save-button,
  .cancel-button {
    padding: 10px;
    font-size: 13px;
  }
}
