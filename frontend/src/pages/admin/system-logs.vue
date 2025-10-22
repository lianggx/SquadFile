<template>
  <div class="system-logs-container">
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <h1 class="title">{{ t('admin.systemLogs') }}</h1>
    
    <div class="tabs">
      <button 
        class="tab-button" 
        :class="{ active: activeTab === 'login' }" 
        @click="switchTab('login')"
      >
        {{ t('admin.loginLogs') }}
      </button>
      <button 
        class="tab-button" 
        :class="{ active: activeTab === 'download' }" 
        @click="switchTab('download')"
      >
        {{ t('admin.downloadLogs') }}
      </button>
    </div>
    
    <div class="logs-content">
      <!-- 搜索和筛选区域 -->
      <div class="search-bar">
        <input
          v-model="searchQuery"
          :placeholder="t('admin.searchPlaceholder')"
          class="search-input"
          @keyup.enter="fetchLogs"
        />
        <button class="search-button" @click="fetchLogs">
          {{ t('admin.search') }}
        </button>
        <button class="reset-button" @click="resetSearch">
          {{ t('admin.reset') }}
        </button>
      </div>
      
      <!-- 登录日志表格 -->
      <div v-if="activeTab === 'login'" class="logs-table-container">
        <table class="logs-table">
          <thead>
            <tr>
              <th>{{ t('admin.username') }}</th>
              <th>{{ t('admin.loginTime') }}</th>
              <th>{{ t('admin.ipAddress') }}</th>
              <th>{{ t('admin.loginResult') }}</th>
              <th>{{ t('admin.failureReason') }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="log in logs" :key="log.id">
              <td>{{ log.username }}</td>
              <td>{{ formatDate(log.loginTime) }}</td>
              <td>{{ log.ipAddress }}</td>
              <td>
                <span :class="log.loginResult ? 'success' : 'failed'">
                  {{ log.loginResult ? t('admin.success') : t('admin.failed') }}
                </span>
              </td>
              <td>{{ log.failureReason || '-' }}</td>
            </tr>
            <tr v-if="logs.length === 0">
              <td colspan="5" class="no-data">{{ t('admin.noLogs') }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- 下载日志表格 -->
      <div v-if="activeTab === 'download'" class="logs-table-container">
        <table class="logs-table">
          <thead>
            <tr>
              <th>{{ t('admin.username') }}</th>
              <th>{{ t('admin.downloadTime') }}</th>
              <th>{{ t('admin.originalFileName') }}</th>
              <th>{{ t('admin.fileSize') }}</th>
              <th>{{ t('admin.ipAddress') }}</th>
              <th>{{ t('admin.deviceType') }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="log in logs" :key="log.id">
              <td>{{ log.username }}</td>
              <td>{{ formatDate(log.downloadTime) }}</td>
              <td>{{ log.originalFileName }}</td>
              <td>{{ formatFileSize(log.fileSize) }}</td>
              <td>{{ log.ipAddress }}</td>
              <td>{{ log.deviceType }}</td>
            </tr>
            <tr v-if="logs.length === 0">
              <td colspan="6" class="no-data">{{ t('admin.noLogs') }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- 分页组件 -->
      <div class="pagination-container" v-if="totalPages > 1">
        <Pagination
          :current-page="currentPage"
          :total-pages="totalPages"
          @page-changed="handlePageChange"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { usePageAuth } from '../../composables/useAuth'
import { getLoginLogsApi, getDownloadLogsApi } from '../../services/system'
import Toast from '../../components/Toast.vue'
import Pagination from '../../components/Pagination.vue'

const { t } = useI18n()
const { checkAuthOnMounted } = usePageAuth()

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 日志数据
const activeTab = ref('login')
const logs = ref([])
const currentPage = ref(1)
const pageSize = ref(20)
const totalPages = ref(0)
const totalCount = ref(0)
const searchQuery = ref('')

// 页面加载时检查认证状态
checkAuthOnMounted(true)

// 切换标签页
const switchTab = (tab) => {
  activeTab.value = tab
  currentPage.value = 1
  fetchLogs()
}

// 获取日志数据
const fetchLogs = async () => {
  try {
    let response
    if (activeTab.value === 'login') {
      response = await getLoginLogsApi({
        page: currentPage.value,
        pageSize: pageSize.value,
        searchQuery: searchQuery.value
      })
    } else {
      response = await getDownloadLogsApi({
        page: currentPage.value,
        pageSize: pageSize.value,
        searchQuery: searchQuery.value
      })
    }
    
    const data = response.data.data
    logs.value = data.items
    totalCount.value = data.totalCount
    totalPages.value = data.totalPages
  } catch (error) {
    console.error('获取日志失败:', error)
    showToast(t('admin.fetchLogsFailed'), 'error')
    logs.value = []
  }
}

// 重置搜索
const resetSearch = () => {
  searchQuery.value = ''
  currentPage.value = 1
  fetchLogs()
}

// 处理分页变化
const handlePageChange = (page) => {
  currentPage.value = page
  fetchLogs()
}

// 格式化日期
const formatDate = (date) => {
  return new Date(date).toLocaleString()
}

// 格式化文件大小
const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message
  toastType.value = type
}

// 页面加载时获取日志数据
onMounted(() => {
  fetchLogs()
})
</script>

<style scoped>
.system-logs-container {
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

.tabs {
  display: flex;
  margin-bottom: 20px;
  border-bottom: 1px solid #ddd;
}

.tab-button {
  padding: 10px 20px;
  background-color: #f0f0f0;
  border: none;
  border-bottom: 3px solid transparent;
  cursor: pointer;
  font-size: 16px;
  color: #666;
  transition: all 0.3s ease;
}

.tab-button:hover {
  background-color: #e0e0e0;
}

.tab-button.active {
  background-color: #fff;
  border-bottom: 3px solid #667eea;
  color: #333;
  font-weight: bold;
}

.search-bar {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
  align-items: center;
}

.search-input {
  flex: 1;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.search-button, .reset-button {
  padding: 10px 15px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s ease;
}

.search-button {
  background-color: #667eea;
  color: white;
}

.search-button:hover {
  background-color: #5a6fd8;
}

.reset-button {
  background-color: #f0f0f0;
  color: #333;
}

.reset-button:hover {
  background-color: #e0e0e0;
}

.logs-table-container {
  overflow-x: auto;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.logs-table {
  width: 100%;
  border-collapse: collapse;
}

.logs-table th,
.logs-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.logs-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #555;
}

.logs-table tbody tr:hover {
  background-color: #f5f7fa;
}

.success {
  color: #4caf50;
  font-weight: bold;
}

.failed {
  color: #f44336;
  font-weight: bold;
}

.no-data {
  text-align: center;
  color: #999;
  font-style: italic;
  padding: 40px;
}

.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: center;
}

@media (max-width: 768px) {
  .system-logs-container {
    padding: 15px;
  }
  
  .title {
    font-size: 22px;
  }
  
  .search-bar {
    flex-direction: column;
    align-items: stretch;
  }
  
  .search-input {
    margin-bottom: 10px;
  }
  
  .logs-table {
    font-size: 12px;
  }
  
  .logs-table th,
  .logs-table td {
    padding: 8px 10px;
  }
}

@media (max-width: 480px) {
  .system-logs-container {
    padding: 10px;
  }
  
  .title {
    font-size: 20px;
  }
  
  .tab-button {
    padding: 8px 12px;
    font-size: 14px;
  }
  
  .logs-table {
    font-size: 11px;
  }
  
  .logs-table th,
  .logs-table td {
    padding: 6px 8px;
  }
}
</style>