<template>
  <div class="admin-container">
    <!-- 添加 Toast 组件 -->
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <h1 class="title">{{ t('admin.title') }}</h1>
    
    <div class="admin-stats">
      <div class="stat-card">
        <span class="stat-value">{{ stats.users }}</span>
        <span class="stat-label">{{ t('admin.totalUsers') }}</span>
      </div>
      <div class="stat-card">
        <span class="stat-value">{{ stats.files }}</span>
        <span class="stat-label">{{ t('admin.totalFiles') }}</span>
      </div>
      <div class="stat-card">
        <span class="stat-value">{{ stats.storage }}</span>
        <span class="stat-label">{{ t('admin.storageUsed') }}</span>
      </div>
    </div>
    
    <div class="admin-menu">
      <div class="menu-item" @click="goToUserManagement">
        <div class="menu-icon">👥</div>
        <div class="menu-content">
          <span class="menu-title">{{ t('admin.userManagement') }}</span>
          <span class="menu-desc">{{ t('admin.userManagementDesc') }}</span>
        </div>
        <div class="menu-arrow">›</div>
      </div>
      
      <div class="menu-item" @click="goToFolderManagement">
        <div class="menu-icon">📁</div>
        <div class="menu-content">
          <span class="menu-title">{{ t('admin.folderManagement') }}</span>
          <span class="menu-desc">{{ t('admin.folderManagementDesc') }}</span>
        </div>
        <div class="menu-arrow">›</div>
      </div>
      
      <div class="menu-item" @click="goToShortLinkManagement">
        <div class="menu-icon">🔗</div>
        <div class="menu-content">
          <span class="menu-title">{{ t('admin.shortLinkManagement') }}</span>
          <span class="menu-desc">{{ t('admin.shortLinkManagementDesc') }}</span>
        </div>
        <div class="menu-arrow">›</div>
      </div>
      
      <div class="menu-item" @click="goToSystemSettings">
        <div class="menu-icon">⚙️</div>
        <div class="menu-content">
          <span class="menu-title">{{ t('admin.systemSettings') }}</span>
          <span class="menu-desc">{{ t('admin.systemSettingsDesc') }}</span>
        </div>
        <div class="menu-arrow">›</div>
      </div>
      
      <div class="menu-item" @click="goToLogs">
        <div class="menu-icon">📝</div>
        <div class="menu-content">
          <span class="menu-title">{{ t('admin.systemLogs') }}</span>
          <span class="menu-desc">{{ t('admin.systemLogsDesc') }}</span>
        </div>
        <div class="menu-arrow">›</div>
      </div>
      
      <div class="menu-item" @click="goToReports">
        <div class="menu-icon">📊</div>
        <div class="menu-content">
          <span class="menu-title">{{ t('admin.reports') }}</span>
          <span class="menu-desc">{{ t('admin.reportsDesc') }}</span>
        </div>
        <div class="menu-arrow">›</div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '../stores/auth'
import { usePageAuth } from '../composables/useAuth'
import { getAdminStatsApi } from '../services/system'

// 导入 Toast 组件
import Toast from '../components/Toast.vue'

const { t } = useI18n()
const router = useRouter()
const authStore = useAuthStore()
const { checkAuthOnMounted } = usePageAuth()

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 管理页面统计数据
const stats = ref({
  users: 0,
  files: 0,
  storage: '0 MB'
})

// 页面加载时检查认证状态（不使用switchTab跳转，因为这是普通页面）
checkAuthOnMounted(false)

// 检查用户权限
const checkAdminPermission = () => {
  // 页面加载时检查用户是否有管理员权限
  if (authStore.user && authStore.user.role !== 'Admin') {
    // 使用 Toast 替代 alert
    showToast(t('admin.noAdminPermission'), 'error')
    
    // 延迟跳转回首页
    setTimeout(() => {
      router.push('/home')
    }, 1500)
  }
}

// 获取管理统计数据
const fetchAdminStats = async () => {
  try {
    const response = await getAdminStatsApi()
    const data = response.data.data
    stats.value = {
      users: data.totalUsers,
      files: data.totalFiles,
      storage: data.storageUsedDisplay
    }
  } catch (error) {
    console.error('获取管理统计数据失败:', error)
    // 如果获取失败，使用默认值
    stats.value = {
      users: 0,
      files: 0,
      storage: '0 MB'
    }
  }
}

// 页面加载时检查权限和获取统计数据
onMounted(() => {
  checkAdminPermission()
  fetchAdminStats()
})

const goToUserManagement = () => {
  router.push('/admin/users')
}

const goToFolderManagement = () => {
  router.push('/admin/folders')
}

const goToShortLinkManagement = () => {
  router.push('/admin/shortlinks')
}

const goToSystemSettings = () => {
  router.push('/admin/settings')
}

const goToLogs = () => {
  router.push('/admin/logs')
}

const goToReports = () => {
  router.push('/admin/reports')
}

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message
  toastType.value = type
}
</script>

<style scoped>
.admin-container {
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

.admin-stats {
  display: flex;
  justify-content: space-between;
  gap: 15px;
  margin-bottom: 30px;
}

.stat-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 10px;
  padding: 15px;
  flex: 1;
  text-align: center;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  color: white;
}

.stat-value {
  font-size: 24px;
  font-weight: bold;
  display: block;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 14px;
  opacity: 0.9;
}

.admin-menu {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.menu-item {
  background-color: white;
  border-radius: 15px;
  padding: 20px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.menu-item:hover {
  transform: translateY(-3px);
  box-shadow: 0 6px 25px rgba(0, 0, 0, 0.15);
}

.menu-icon {
  font-size: 28px;
  margin-right: 15px;
  width: 40px;
  text-align: center;
}

.menu-content {
  flex: 1;
}

.menu-title {
  font-size: 18px;
  font-weight: bold;
  color: #333;
  display: block;
  margin-bottom: 5px;
}

.menu-desc {
  font-size: 14px;
  color: #666;
}

.menu-arrow {
  font-size: 24px;
  color: #999;
}
</style>
/* 响应式设计 */
@media (max-width: 768px) {
  .admin-container {
    padding: 15px;
  }
  
  .title {
    font-size: 22px;
  }
  
  .admin-stats {
    flex-direction: column;
    gap: 10px;
  }
  
  .stat-card {
    padding: 12px;
  }
  
  .stat-value {
    font-size: 20px;
  }
  
  .stat-label {
    font-size: 12px;
  }
  
  .menu-item {
    padding: 15px;
  }
  
  .menu-icon {
    font-size: 24px;
    margin-right: 10px;
  }
  
  .menu-title {
    font-size: 16px;
  }
  
  .menu-desc {
    font-size: 12px;
  }
  
  .menu-arrow {
    font-size: 20px;
  }
}

@media (max-width: 480px) {
  .admin-container {
    padding: 10px;
  }
  
  .title {
    font-size: 20px;
  }
  
  .stat-card {
    padding: 10px;
  }
  
  .stat-value {
    font-size: 18px;
  }
  
  .stat-label {
    font-size: 11px;
  }
  
  .menu-item {
    padding: 12px;
  }
  
  .menu-icon {
    font-size: 20px;
    margin-right: 8px;
  }
  
  .menu-title {
    font-size: 15px;
  }
  
  .menu-desc {
    font-size: 11px;
  }
  
  .menu-arrow {
    font-size: 18px;
  }
}
