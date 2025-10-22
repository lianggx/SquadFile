<template>
  <div class="reports-container">
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <h1 class="title">{{ t('admin.reports') }}</h1>
    
    <div class="reports-content">
      <!-- 统计概览卡片 -->
      <div class="stats-cards">
        <div class="stat-card">
          <div class="stat-icon">👥</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.totalUsers }}</div>
            <div class="stat-label">{{ t('admin.totalUsers') }}</div>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">📁</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.totalFiles }}</div>
            <div class="stat-label">{{ t('admin.totalFiles') }}</div>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">💾</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.storageUsed }}</div>
            <div class="stat-label">{{ t('admin.storageUsed') }}</div>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">📥</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.totalDownloads }}</div>
            <div class="stat-label">{{ t('admin.totalDownloads') }}</div>
          </div>
        </div>
      </div>
      
      <!-- 新增的统计模块 -->
      <!-- 最新上传文件列表 -->
      <div class="section-card">
        <h3>{{ t('admin.latestUploads') }}</h3>
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>{{ t('admin.fileName') }}</th>
                <th>{{ t('admin.fileSize') }}</th>
                <th>{{ t('admin.uploadTime') }}</th>
                <th>{{ t('admin.uploader') }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="file in stats.latestFiles" :key="file.id">
                <td>{{ file.name }}</td>
                <td>{{ formatFileSize(file.size) }}</td>
                <td>{{ formatDateTime(file.uploadTime) }}</td>
                <td>{{ file.uploadedBy }}</td>
              </tr>
              <tr v-if="stats.latestFiles.length === 0">
                <td colspan="4" class="no-data">{{ t('admin.noLogs') }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      
      <!-- 文件类型分布 -->
      <div class="section-card">
        <h3>{{ t('admin.fileTypeDistribution') }}</h3>
        <div class="chart-placeholder">
          <div v-if="stats.fileTypeDistribution.length > 0" class="distribution-list">
            <div v-for="item in stats.fileTypeDistribution" :key="item.extension" class="distribution-item">
              <span class="extension">{{ item.extension || t('admin.fileTypes.unknown') }}</span>
              <span class="count">{{ item.count }}</span>
              <div class="progress-bar">
                <div class="progress" :style="{ width: getFileTypePercentage(item.count) + '%' }"></div>
              </div>
            </div>
          </div>
          <p v-else>{{ t('admin.chartComingSoon') }}</p>
        </div>
      </div>
      
      <!-- 分享短链接统计 -->
      <div class="stats-cards">
        <div class="stat-card">
          <div class="stat-icon">🔗</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.totalShareLinks }}</div>
            <div class="stat-label">{{ t('admin.totalShareLinks') }}</div>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">✅</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.activeShareLinks }}</div>
            <div class="stat-label">{{ t('admin.activeShareLinks') }}</div>
          </div>
        </div>
      </div>
      
      <!-- 下载统计 -->
      <div class="section-card">
        <h3>{{ t('admin.downloadStats') }}</h3>
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>{{ t('admin.fileName') }}</th>
                <th>{{ t('admin.downloadCount') }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="stat in stats.downloadStats" :key="stat.fileId">
                <td>{{ stat.fileName }}</td>
                <td>{{ stat.downloadCount }}</td>
              </tr>
              <tr v-if="stats.downloadStats.length === 0">
                <td colspan="2" class="no-data">{{ t('admin.noLogs') }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { usePageAuth } from '../../composables/useAuth'
import { getAdminStatsApi } from '../../services/system'
import Toast from '../../components/Toast.vue'

const { t } = useI18n()
const { checkAuthOnMounted } = usePageAuth()

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 统计数据
const stats = ref({
  totalUsers: 0,
  totalFiles: 0,
  storageUsed: '0 MB',
  totalDownloads: 0,
  latestFiles: [],
  fileTypeDistribution: [],
  totalShareLinks: 0,
  activeShareLinks: 0,
  downloadStats: [],
  lastReportDate: new Date()
})

// 页面加载时检查认证状态
checkAuthOnMounted(true)

// 获取统计数据
const fetchStats = async () => {
  try {
    const response = await getAdminStatsApi()
    const data = response.data.data
    stats.value = {
      totalUsers: data.totalUsers,
      totalFiles: data.totalFiles,
      storageUsed: data.storageUsedDisplay,
      totalDownloads: data.totalDownloads || 0,
      latestFiles: data.latestFiles || [],
      fileTypeDistribution: data.fileTypeDistribution || [],
      totalShareLinks: data.totalShareLinks || 0,
      activeShareLinks: data.activeShareLinks || 0,
      downloadStats: data.downloadStats || [],
      lastReportDate: new Date()
    }
  } catch (error) {
    console.error('获取统计数据失败:', error)
    showToast(t('admin.fetchStatsFailed'), 'error')
  }
}

// 格式化日期时间
const formatDateTime = (date) => {
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

// 计算文件类型分布的百分比
const getFileTypePercentage = (count) => {
  if (stats.value.fileTypeDistribution.length === 0) return 0
  const maxCount = Math.max(...stats.value.fileTypeDistribution.map(item => item.count))
  return (count / maxCount) * 100
}

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message
  toastType.value = type
}

// 页面加载时获取统计数据
onMounted(() => {
  fetchStats()
})
</script>

<style scoped>
.reports-container {
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

.stats-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.stat-card {
  background: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  transition: transform 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-5px);
}

.stat-icon {
  font-size: 32px;
  margin-right: 15px;
}

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: 24px;
  font-weight: bold;
  color: #333;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 14px;
  color: #666;
}

.section-card {
  background: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  margin-bottom: 30px;
}

.section-card h3 {
  margin-top: 0;
  margin-bottom: 15px;
  color: #333;
}

.table-container {
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th,
.data-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.data-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #555;
}

.data-table tbody tr:hover {
  background-color: #f5f7fa;
}

.no-data {
  text-align: center;
  color: #999;
  font-style: italic;
}

.chart-placeholder {
  min-height: 200px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f8f9fa;
  border-radius: 8px;
  padding: 20px;
}

.chart-placeholder p {
  color: #999;
  font-style: italic;
}

.distribution-list {
  width: 100%;
}

.distribution-item {
  margin-bottom: 15px;
}

.extension {
  display: inline-block;
  width: 80px;
  font-weight: bold;
}

.count {
  display: inline-block;
  width: 50px;
  text-align: right;
  margin-right: 10px;
}

.progress-bar {
  display: inline-block;
  width: calc(100% - 150px);
  height: 8px;
  background-color: #e9ecef;
  border-radius: 4px;
  overflow: hidden;
}

.progress {
  height: 100%;
  background-color: #667eea;
  border-radius: 4px;
}

@media (max-width: 768px) {
  .reports-container {
    padding: 15px;
  }
  
  .title {
    font-size: 22px;
  }
  
  .stats-cards {
    grid-template-columns: 1fr;
  }
  
  .stat-card {
    padding: 15px;
  }
  
  .stat-value {
    font-size: 20px;
  }
  
  .stat-label {
    font-size: 12px;
  }
  
  .section-card {
    padding: 15px;
  }
  
  .data-table {
    font-size: 14px;
  }
  
  .data-table th,
  .data-table td {
    padding: 8px 10px;
  }
  
  .extension {
    width: 60px;
  }
  
  .count {
    width: 40px;
  }
  
  .progress-bar {
    width: calc(100% - 120px);
  }
}

@media (max-width: 480px) {
  .reports-container {
    padding: 10px;
  }
  
  .title {
    font-size: 20px;
  }
  
  .stat-card {
    padding: 12px;
  }
  
  .stat-icon {
    font-size: 24px;
    margin-right: 10px;
  }
  
  .stat-value {
    font-size: 18px;
  }
  
  .stat-label {
    font-size: 11px;
  }
  
  .section-card {
    padding: 12px;
  }
  
  .data-table {
    font-size: 12px;
  }
  
  .data-table th,
  .data-table td {
    padding: 6px 8px;
  }
  
  .extension {
    width: 50px;
  }
  
  .count {
    width: 30px;
  }
  
  .progress-bar {
    width: calc(100% - 90px);
  }
}
</style>