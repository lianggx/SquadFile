<template>
  <div class="profile-container">
    <h1 class="title">{{ t('profile.title') }}</h1>
    
    <div class="profile-card">
      <div class="profile-header">
        <img 
          :src="userAvatar" 
          :alt="displayUserName" 
          class="avatar"
          @error="handleAvatarError"
        />
        <div class="user-info">
          <h2>{{ displayUserName }}</h2>
          <p class="username">@{{ authStore.user?.username }}</p>
          <p class="role">{{ userRole }}</p>
        </div>
      </div>
      
      <div class="profile-details">
        <div class="detail-item">
          <span class="label">{{ t('profile.email') }}:</span>
          <span class="value">{{ authStore.user?.email || '-' }}</span>
        </div>
        
        <div class="detail-item">
          <span class="label">{{ t('profile.department') }}:</span>
          <span class="value">{{ authStore.user?.department || '-' }}</span>
        </div>
        
        <div class="detail-item">
          <span class="label">{{ t('profile.isFirstLogin') }}:</span>
          <span class="value" :class="{ 'first-login': authStore.user?.isFirstLogin }">
            {{ authStore.user?.isFirstLogin ? t('profile.yes') : t('profile.no') }}
          </span>
        </div>
      </div>
      
      <div class="profile-actions">
        <button @click="goToEditProfile" class="action-button edit-button">
          {{ t('profile.editProfile') }}
        </button>
        <button @click="goToChangePassword" class="action-button password-button">
          {{ t('profile.changePassword') }}
        </button>
        <!-- 添加退出登录按钮 -->
        <button @click="handleLogout" class="action-button logout-button">
          {{ t('home.logout') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'
import { usePageAuth } from '../../composables/useAuth'

const { t } = useI18n()
const router = useRouter()
const authStore = useAuthStore()
const { checkAuthOnMounted } = usePageAuth()

// 页面加载时检查认证状态
checkAuthOnMounted(false)

// 用户头像
const userAvatar = computed(() => {
  return authStore.user?.avatar || '/static/images/default-avatar.png'
})

// 显示的用户名
const displayUserName = computed(() => {
  if (authStore.user) {
    return authStore.user.displayName || authStore.user.username || t('home.user')
  }
  return t('home.user')
})

// 用户角色
const userRole = computed(() => {
  if (authStore.user) {
    return authStore.user.role === 'Admin' ? t('home.admin') : t('home.normalUser')
  }
  return t('home.normalUser')
})

// 处理头像加载错误
const handleAvatarError = (event) => {
  event.target.src = '/static/images/default-avatar.png'
}

// 跳转到编辑资料页面
const goToEditProfile = () => {
  router.push('/profile/edit')
}

// 跳转到修改密码页面
const goToChangePassword = () => {
  router.push('/profile/change-password')
}

// 添加退出登录处理函数
const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}

</script>

<style scoped>
.profile-container {
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
  margin-bottom: 20px;
  text-align: center;
}

.profile-card {
  background-color: white;
  border-radius: 15px;
  padding: 30px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  max-width: 600px;
  margin: 0 auto;
  width: 100%;
}

.profile-header {
  display: flex;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 1px solid #eee;
}

.avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  object-fit: cover;
  margin-right: 20px;
}

.user-info h2 {
  font-size: 24px;
  color: #333;
  margin: 0 0 5px 0;
}

.username {
  font-size: 16px;
  color: #666;
  margin: 0 0 10px 0;
}

.role {
  font-size: 14px;
  color: #999;
  background-color: #f0f0f0;
  padding: 3px 10px;
  border-radius: 15px;
  display: inline-block;
}

.profile-details {
  margin-bottom: 30px;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  padding: 12px 0;
  border-bottom: 1px solid #f5f5f5;
}

.detail-item:last-child {
  border-bottom: none;
}

.label {
  font-weight: bold;
  color: #555;
}

.value {
  color: #333;
}

.first-login {
  color: #ff4d4f;
  font-weight: bold;
}

.profile-actions {
  display: flex;
  gap: 15px;
  flex-direction: column; /* 改为垂直排列 */
}

.action-button {
  flex: 1;
  padding: 15px;
  border: none;
  border-radius: 10px;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.edit-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.password-button {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
  color: white;
}

/* 添加退出登录按钮样式 */
.logout-button {
  background: linear-gradient(135deg, #ff416c, #ff4b2b);
  color: white;
}

.action-button:hover {
  opacity: 0.9;
  transform: translateY(-2px);
}
</style>