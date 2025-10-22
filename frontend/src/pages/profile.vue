<template>
  <div class="profile-container">
    <!-- 添加 Toast 组件 -->
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <h1 class="title">{{ t('profile.title') }}</h1>
    <div class="profile-card">
      <div class="avatar-section">
        <img 
          class="avatar" 
          :src="avatarUrl" 
          @error="handleAvatarError"
        />
      </div>
      <div class="profile-info">
        <div class="info-item">
          <span class="label">{{ t('profile.username') }}:</span>
          <span class="value">{{ displayUserInfo.username }}</span>
        </div>
        <div class="info-item">
          <span class="label">{{ t('profile.email') }}:</span>
          <span class="value">{{ displayUserInfo.email }}</span>
        </div>
        <div class="info-item">
          <span class="label">{{ t('profile.role') }}:</span>
          <span class="value">{{ displayUserInfo.role }}</span>
        </div>
        <div class="info-item">
          <span class="label">{{ t('profile.joinDate') }}:</span>
          <span class="value">{{ displayUserInfo.joinDate }}</span>
        </div>
      </div>
    </div>
    
    <div class="profile-actions">
      <button @click="goToChangePassword" class="action-button">
        {{ t('profile.changePassword') }}
      </button>
      <button @click="goToEditProfile" class="action-button">
        {{ t('profile.editProfile') }}
      </button>
      <button @click="handleLogout" class="logout-button">
        {{ t('home.logout') }}
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '../stores/auth'
import { usePageAuth } from '../composables/useAuth'

// 导入 Toast 组件
import Toast from '../components/Toast.vue'

const { t } = useI18n()
const router = useRouter()
const authStore = useAuthStore()
const { checkAuthOnMounted, checkAuthOnShow } = usePageAuth()

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 用户信息
const userInfo = ref({
  username: '',
  email: '',
  role: '',
  createdTime: ''
})

// 头像URL
const avatarUrl = ref('/static/images/default-avatar.png')

// 计算属性，用于显示用户信息，确保即使没有数据也能显示默认值
const displayUserInfo = computed(() => {
  return {
    username: userInfo.value.username || t('profile.notSet'),
    email: userInfo.value.email || t('profile.notSet'),
    role: userInfo.value.role || t('profile.user'),
    joinDate: userInfo.value.createdTime ? new Date(userInfo.value.createdTime).toLocaleDateString() : t('profile.notSet')
  }
})

// 页面加载时检查认证状态（使用redirectTo跳转，因为登录页不在tabBar中）
checkAuthOnMounted(false)

// 页面显示时检查认证状态（使用redirectTo跳转，因为登录页不在tabBar中）
checkAuthOnShow(false)

// 处理头像加载错误
const handleAvatarError = (event) => {
  avatarUrl.value = '/static/images/default-avatar.png'
}

// 获取用户信息
const fetchUserInfo = () => {
  
  // 检查认证状态并获取用户信息
  const isAuthenticated = authStore.checkAuth()
  
  if (isAuthenticated && authStore.user) {
    userInfo.value = {
      username: authStore.user.username || '',
      email: authStore.user.email || '',
      role: authStore.user.role || '',
      createdTime: authStore.user.createdTime || ''
    }
    
    // 如果有头像信息，设置头像URL
    if (authStore.user.avatar) {
      avatarUrl.value = authStore.user.avatar
    }
  } else {
    // 尝试从localStorage获取用户信息
    try {
      const userStr = localStorage.getItem('user')
      if (userStr) {
        const user = JSON.parse(userStr)
        userInfo.value = {
          username: user.username || '',
          email: user.email || '',
          role: user.role || '',
          createdTime: user.createdTime || ''
        }
        
        // 如果有头像信息，设置头像URL
        if (user.avatar) {
          avatarUrl.value = user.avatar
        }
      } else {
        // 如果没有用户信息，设置默认值
        userInfo.value = {
          username: t('profile.guest'),
          email: t('profile.notSet'),
          role: t('profile.user'),
          createdTime: ''
        }
      }
    } catch (e) {
      // 如果解析失败，设置默认值
      userInfo.value = {
        username: t('profile.guest'),
        email: t('profile.notSet'),
        role: t('profile.user'),
        createdTime: ''
      }
    }
  }
}

// 页面加载时获取用户信息
onMounted(() => {
  fetchUserInfo()
})

// 跳转到修改密码页面
const goToChangePassword = () => {
  router.push('/profile/change-password')
}

// 跳转到编辑资料页面
const goToEditProfile = () => {
  router.push('/profile/edit-profile')
}

// 修改密码
const changePassword = () => {
  // 使用 Toast 替代 alert
  showToast(t('profile.changePasswordComingSoon'), 'info')
}

// 编辑个人资料
const editProfile = () => {
  // 使用 Toast 替代 alert
  showToast(t('profile.editProfileComingSoon'), 'info')
}

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message
  toastType.value = type
}
</script>

<style scoped>
.profile-container {
  display: flex;
  flex-direction: column;
  padding: 20px;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: 100vh;
  box-sizing: border-box;
}

.title {
  font-size: 24px;
  font-weight: bold;
  color: #333;
  margin-bottom: 30px;
  text-align: center;
}

.profile-card {
  background-color: white;
  border-radius: 15px;
  padding: 25px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  margin-bottom: 30px;
  width: 100%;
  box-sizing: border-box;
}

.avatar-section {
  display: flex;
  justify-content: center;
  margin-bottom: 25px;
}

.avatar {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  border: 3px solid #667eea;
  object-fit: cover;
  background-color: #f0f0f0;
}

.profile-info {
  display: flex;
  flex-direction: column;
}

.info-item {
  display: flex;
  margin-bottom: 20px;
  align-items: center;
}

.info-item:last-child {
  margin-bottom: 0;
}

.label {
  font-weight: bold;
  color: #555;
  width: 80px;
  flex-shrink: 0;
  font-size: 16px;
}

.value {
  color: #333;
  flex: 1;
  font-size: 16px;
  word-break: break-word;
}

.profile-actions {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.action-button {
  padding: 15px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 16px;
  cursor: pointer;
  box-shadow: 0 2px 10px rgba(102, 126, 234, 0.3);
  width: 100%;
  box-sizing: border-box;
}

.action-button:hover {
  opacity: 0.9;
  transform: translateY(-2px);
}

.logout-button {
  padding: 15px;
  background-color: #ff4d4f;
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 16px;
  cursor: pointer;
  box-shadow: 0 2px 10px rgba(255, 77, 79, 0.3);
  width: 100%;
  box-sizing: border-box;
}

.logout-button:hover {
  opacity: 0.9;
  transform: translateY(-2px);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .profile-container {
    padding: 15px;
  }
  
  .profile-card {
    padding: 20px;
  }
  
  .label {
    width: 70px;
    font-size: 14px;
  }
  
  .value {
    font-size: 14px;
  }
  
  .action-button,
  .logout-button {
    padding: 12px;
    font-size: 14px;
  }
}
</style>