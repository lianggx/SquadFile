<template>
  <header class="header">
    <div class="header-left">
      <!-- 增加点击LOGO回到首页的功能 -->
      <img 
        :src="systemConfig.logoPath" 
        :alt="systemConfig.siteName" 
        class="logo"
        @error="handleLogoError"
        @click="goToHome"
      />
      <h1 class="site-name" @click="goToHome">{{ systemConfig.siteName }}</h1>
    </div>
    
    <!-- 桌面端导航菜单 -->
    <nav class="header-nav desktop-nav">
      <ul class="nav-list">
        <!-- 增加首页菜单项 -->
        <li class="nav-item">
          <button class="nav-button" @click="goToHome">
            {{ t('home.home') }}
          </button>
        </li>
        <!-- 仅当用户是管理员时显示管理面板菜单项 -->
        <li v-if="isAdmin" class="nav-item">
          <button class="nav-button" @click="goToAdmin">
            {{ t('home.adminPanel') }}
          </button>
        </li>
      </ul>
    </nav>
    
    <div class="header-right">
      <div class="user-info" @click="goToProfile">
        <img 
          :src="userAvatar" 
          :alt="displayUserName" 
          class="avatar"
          @error="handleAvatarError"
        />
        <div class="user-details">
          <div class="user-name">{{ displayUserName }}</div>
        </div>
      </div>
      
      <!-- 移动端菜单按钮 -->
      <div class="mobile-menu-button" @click="toggleMobileMenu">
        <div class="hamburger-icon">
          <span></span>
          <span></span>
          <span></span>
        </div>
      </div>
    </div>
    
    <!-- 移动端导航菜单 -->
    <div v-if="isMobileMenuOpen" class="mobile-nav-overlay" @click="closeMobileMenu">
      <div class="mobile-nav-menu" @click.stop>
        <div class="mobile-nav-header">
          <div class="mobile-site-info">
            <img 
              :src="systemConfig.logoPath" 
              :alt="systemConfig.siteName" 
              class="mobile-logo"
              @error="handleLogoError"
            />
            <h2 class="mobile-site-name">{{ systemConfig.siteName }}</h2>
          </div>
          <button class="close-button" @click="closeMobileMenu">×</button>
        </div>
        
        <ul class="mobile-nav-list">
          <li class="mobile-nav-item">
            <button class="mobile-nav-button" @click="goToHomeAndClose">
              {{ t('home.home') }}
            </button>
          </li>
          <!-- 仅当用户是管理员时显示管理面板菜单项 -->
          <li v-if="isAdmin" class="mobile-nav-item">
            <button class="mobile-nav-button" @click="goToAdminAndClose">
              {{ t('home.adminPanel') }}
            </button>
          </li>
          <li class="mobile-nav-item">
            <button class="mobile-nav-button" @click="goToProfileAndClose">
              {{ t('profile.title') }}
            </button>
          </li>
        </ul>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { getSystemConfigApi } from '../services/system'

const { t, locale } = useI18n()
const router = useRouter()
const authStore = useAuthStore()

// 系统配置
const systemConfig = ref({
  siteName: '小队快传',
  logoPath: '/static/images/logo.png'
})

// 移动端菜单状态
const isMobileMenuOpen = ref(false)

// 计算属性：显示的用户名
const displayUserName = computed(() => {
  if (authStore.user) {
    return authStore.user.displayName || authStore.user.username || t('home.user')
  }
  return t('home.user')
})

// 用户头像
const userAvatar = computed(() => {
  return authStore.user?.avatar || '/static/images/default-avatar.png'
})

// 计算属性：判断用户是否为管理员
const isAdmin = computed(() => {
  return authStore.user && authStore.user.role === 'Admin'
})

// 处理LOGO加载错误
const handleLogoError = (event) => {
  event.target.src = '/static/images/logo.png'
}

// 处理头像加载错误
const handleAvatarError = (event) => {
  event.target.src = '/static/images/default-avatar.png'
}

// 获取系统配置
const fetchSystemConfig = async () => {
  try {
    const response = await getSystemConfigApi()
    if (response && response.data) {
      systemConfig.value = {
        siteName: response.data.data.siteName || '小队快传',
        logoPath: response.data.data.homeLogoPath || response.data.logoPath || '/static/images/logo.png'
      }
    }
  } catch (error) {
    console.error('获取系统配置失败:', error)
    // 使用默认配置
    systemConfig.value = {
      siteName: '小队快传',
      logoPath: '/static/images/logo.png'
    }
  }
}

// 跳转到首页
const goToHome = () => {
  router.push('/')
}

// 跳转到管理页面
const goToAdmin = () => {
  router.push('/admin')
}

// 跳转到个人信息页面
const goToProfile = () => {
  router.push('/profile')
}

// 移动端菜单控制
const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
}

// 带关闭菜单的导航函数
const goToHomeAndClose = () => {
  goToHome()
  closeMobileMenu()
}

const goToAdminAndClose = () => {
  goToAdmin()
  closeMobileMenu()
}

const goToProfileAndClose = () => {
  goToProfile()
  closeMobileMenu()
}

// 页面加载时获取系统配置
onMounted(() => {
  fetchSystemConfig()
})
</script>

<style scoped>
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 30px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  box-shadow: 0 2px 15px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 15px;
}

.logo {
  height: 45px;
  width: 45px;
  border-radius: 50%;
  object-fit: cover;
  cursor: pointer;
}

.site-name {
  font-size: 22px;
  font-weight: 700;
  margin: 0;
  letter-spacing: 0.5px;
  cursor: pointer;
}

.header-nav {
  flex: 1;
  display: flex;
  justify-content: center;
}

.nav-list {
  display: flex;
  list-style: none;
  gap: 20px;
}

.nav-item {
  margin: 0;
}

.nav-button {
  background: none;
  border: none;
  color: white;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  padding: 8px 16px;
  border-radius: 20px;
  transition: background-color 0.3s ease;
}

.nav-button:hover {
  background-color: rgba(255, 255, 255, 0.2);
}

.header-right {
  display: flex;
  align-items: center;
  gap: 25px;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  padding: 8px 15px;
  border-radius: 30px;
  transition: background-color 0.3s ease;
  background-color: rgba(255, 255, 255, 0.15);
}

.user-info:hover {
  background-color: rgba(255, 255, 255, 0.25);
}

.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid rgba(255, 255, 255, 0.3);
}

.user-details {
  display: flex;
  flex-direction: column;
}

.user-name {
  font-weight: 600;
  font-size: 15px;
}

/* 移动端菜单按钮 */
.mobile-menu-button {
  display: none;
  cursor: pointer;
}

.hamburger-icon {
  display: flex;
  flex-direction: column;
  justify-content: space-around;
  width: 25px;
  height: 25px;
}

.hamburger-icon span {
  display: block;
  height: 3px;
  width: 100%;
  background-color: white;
  border-radius: 2px;
  transition: all 0.3s ease;
}

/* 移动端导航菜单 */
.mobile-nav-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 1001;
  display: flex;
  justify-content: flex-end;
}

.mobile-nav-menu {
  background: white;
  width: 80%;
  max-width: 300px;
  height: 100%;
  box-shadow: -2px 0 10px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
}

.mobile-nav-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.mobile-site-info {
  display: flex;
  align-items: center;
  gap: 10px;
}

.mobile-logo {
  height: 40px;
  width: 40px;
  border-radius: 50%;
  object-fit: cover;
}

.mobile-site-name {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
}

.close-button {
  background: none;
  border: none;
  color: white;
  font-size: 24px;
  cursor: pointer;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.mobile-nav-list {
  list-style: none;
  padding: 20px 0;
  margin: 0;
  flex: 1;
}

.mobile-nav-item {
  margin-bottom: 10px;
}

.mobile-nav-button {
  display: block;
  width: 100%;
  text-align: left;
  padding: 15px 20px;
  background: none;
  border: none;
  font-size: 16px;
  color: #333;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.mobile-nav-button:hover {
  background-color: #f5f5f5;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .header {
    padding: 12px 20px;
  }
  
  .logo {
    height: 40px;
    width: 40px;
  }
  
  .site-name {
    font-size: 20px;
  }
  
  .user-info {
    padding: 6px 12px;
  }
  
  .avatar {
    width: 35px;
    height: 35px;
  }
  
  .user-name {
    font-size: 14px;
  }
  
  .nav-button {
    font-size: 14px;
    padding: 6px 12px;
  }
  
  /* 在移动端隐藏桌面导航 */
  .desktop-nav {
    display: none;
  }
  
  /* 显示移动端菜单按钮 */
  .mobile-menu-button {
    display: block;
  }
  
  .header-right {
    gap: 15px;
  }
}

@media (max-width: 480px) {
  .header {
    padding: 10px 15px;
  }
  
  .logo {
    height: 35px;
    width: 35px;
  }
  
  .site-name {
    font-size: 18px;
  }
  
  .user-info {
    padding: 5px 10px;
  }
  
  .avatar {
    width: 30px;
    height: 30px;
  }
  
  .user-details {
    display: none;
  }
  
  .nav-button {
    font-size: 13px;
    padding: 5px 10px;
  }
  
  .mobile-nav-menu {
    width: 100%;
    max-width: none;
  }
}
</style>