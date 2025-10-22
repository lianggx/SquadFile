<template>
  <div class="share-container">
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <div class="share-content">
      <div v-if="loading" class="loading">
        {{ t('share.loading') }}
      </div>
      
      <div v-else-if="error" class="error">
        <h2>{{ t('share.error') }}</h2>
        <p>{{ error }}</p>
      </div>
      
      <!-- 文件夹内容显示 -->
      <div v-else-if="folderContent" class="folder-content">
        <div class="folder-header">
          <h2>{{ folderInfo.name }}</h2>
        </div>
        
        <!-- 文件夹内容列表 -->
        <div class="folder-items">
          <div 
            v-for="item in folderContent" 
            :key="item.id" 
            class="list-item"
            :class="{ 'file-item': item.extension, 'folder-item': !item.extension }"
            @click="viewItem(item)"
          >
            <div class="item-icon">
              <div v-if="isImageFile(item.extension)" class="file-icon-image">🖼️</div>
              <div v-else-if="isVideoFile(item.extension)" class="file-icon-video">🎬</div>
              <div v-else-if="isAudioFile(item.extension)" class="file-icon-audio">🎵</div>
              <div v-else-if="isDocumentFile(item.extension)" class="file-icon-document">📄</div>
              <div v-else-if="isArchiveFile(item.extension)" class="file-icon-archive">📦</div>
              <div v-else-if="isCodeFile(item.extension)" class="file-icon-code">💻</div>
              <div v-else-if="!item.extension" class="file-icon-folder">📁</div>
              <div v-else class="file-icon-default">📁</div>
            </div>
            <div class="item-info">
              <div class="item-name">{{ item.originalName }}</div>
              <div class="item-meta">
                <span class="item-size">{{ formatFileSize(item.size) }}</span>
                <span class="item-type">{{ item.extension ? getFileTypeDisplayName(item.extension) : t('admin.folder') }}</span>
                <span class="item-time">{{ formatDate(item.uploadTime) }}</span>
              </div>
            </div>
            <div class="item-actions">
              <button 
                v-if="item.extension" 
                @click.stop="downloadFileFromFolder(item)" 
                class="download-button small"
                :disabled="downloadingFileId === item.id"
              >
                <span v-if="downloadingFileId === item.id">{{ t('share.loading') }}...</span>
                <span v-else>{{ t('share.download') }}</span>
              </button>
            </div>
          </div>
        </div>
      </div>
      
      <div v-else-if="itemInfo" class="item-info">
        <h2>{{ itemType === 'file' ? t('share.fileTitle') : t('share.folderTitle') }}</h2>
        
        <div class="item-details">
          <div class="item-icon">
            <div v-if="itemType === 'file'">
              <div v-if="isImageFile(itemInfo.extension)" class="file-icon-image">🖼️</div>
              <div v-else-if="isVideoFile(itemInfo.extension)" class="file-icon-video">🎬</div>
              <div v-else-if="isAudioFile(itemInfo.extension)" class="file-icon-audio">🎵</div>
              <div v-else-if="isDocumentFile(itemInfo.extension)" class="file-icon-document">📄</div>
              <div v-else-if="isArchiveFile(itemInfo.extension)" class="file-icon-archive">📦</div>
              <div v-else-if="isCodeFile(itemInfo.extension)" class="file-icon-code">💻</div>
              <div v-else class="file-icon-default">📁</div>
            </div>
            <div v-else>
              📁
            </div>
          </div>
          
          <div class="item-name">{{ itemInfo.name }}</div>
          
          <div v-if="itemType === 'file'" class="file-meta">
            <div class="file-size">{{ formatFileSize(itemInfo.size) }}</div>
            <div class="file-type">{{ getFileTypeDisplayName(itemInfo.extension) }}</div>
            <div class="upload-time">{{ formatDate(itemInfo.createdTime) }}</div>
          </div>
        </div>
        
        <div v-if="requiresPassword" class="password-form">
          <h3>{{ t('share.enterPassword') }}</h3>
          <form @submit.prevent="validatePassword">
            <div class="form-group">
              <input 
                v-model="password" 
                type="password" 
                :placeholder="t('share.passwordPlaceholder')"
                required
              />
            </div>
            <div class="form-actions">
              <button type="submit" class="submit-button">{{ t('share.submit') }}</button>
            </div>
          </form>
        </div>
        
        <div v-else class="action-buttons">
          <button 
            v-if="itemType === 'file'" 
            @click="downloadFile" 
            class="download-button"
            :disabled="isDownloading"
          >
            <span v-if="isDownloading">{{ t('share.loading') }}...</span>
            <span v-else>{{ t('share.download') }}</span>
          </button>
          <button 
            v-else 
            @click="viewFolderContent" 
            class="view-button"
          >
            {{ t('share.folderView') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import { validateSharePasswordApi, getShareItemInfoApi, downloadFileApi, getShareFolderContentApi } from '../../services/file'
import Toast from '../../components/Toast.vue'
// 导入公共工具函数
import { 
  isImageFile, 
  isVideoFile, 
  isAudioFile, 
  isDocumentFile, 
  isArchiveFile, 
  isCodeFile, 
  formatFileSize, 
  formatDate, 
  getFileTypeDisplayName 
} from '../../utils/fileUtils'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
// 注意：分享页面不需要认证检查，所以我们不调用checkAuthOnMounted

// 状态
const loading = ref(true)
const error = ref('')
const itemInfo = ref(null)
const folderInfo = ref(null)
const folderContent = ref(null)
const itemType = ref('')
const requiresPassword = ref(false)
const password = ref('')
const shareToken = ref('')

// 下载状态
const isDownloading = ref(false)
const downloadingFileId = ref(null)

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 获取短链接代码
const shortCode = route.params.shortCode

// 页面加载时获取数据
onMounted(() => {
  if (shortCode) {
    fetchItemInfo()
  } else {
    error.value = t('share.invalidLink')
    loading.value = false
  }
})

// 获取项目信息
const fetchItemInfo = async () => {
  try {
    const response = await getShareItemInfoApi(shortCode)
    
    if (response.data.code === 200) {
      const data = response.data.data
      itemInfo.value = data
      requiresPassword.value = data.hasPassword
      
      // 如果没有密码，直接显示文件/文件夹信息
      if (!data.hasPassword) {
        if (data.extension !== undefined) {
          itemType.value = 'file'
        } else {
          itemType.value = 'folder'
          folderInfo.value = data
        }
      }
    } else {
      // 处理特定的错误消息
      if (response.data.message === 'ShareExpired') {
        error.value = t('share.shareExpired')
      } else if (response.data.message === 'InvalidShortCode') {
        error.value = t('share.invalidShortCode')
      } else {
        error.value = response.data.message || t('share.fetchFailed')
      }
    }
    
    loading.value = false
  } catch (err) {
    error.value = t('share.fetchFailed')
    loading.value = false
  }
}

// 验证密码
const validatePassword = async () => {
  try {
    const response = await validateSharePasswordApi(shortCode, password.value)
    
    // 检查密码验证结果，兼容大小写不同的字段名
    const isValid = response.data.code === 200 && 
      (response.data.data.isValid || response.data.data.IsValid);
    
    if (isValid) {
      // 密码正确，直接设置文件/文件夹类型并隐藏密码表单
      requiresPassword.value = false
      
      // 保存token用于后续请求
      shareToken.value = response.data.data.token
      
      // 设置文件/文件夹类型
      if (itemInfo.value.extension !== undefined) {
        itemType.value = 'file'
      } else {
        itemType.value = 'folder'
        folderInfo.value = itemInfo.value
        // 如果是文件夹，直接查看文件夹内容
        viewFolderContent()
      }
    } else {
      showToast(t('share.invalidPassword'), 'error')
    }
  } catch (err) {
    // 检查错误响应状态码，如果是401则显示密码错误，而不是跳转到登录页
    if (err.response && err.response.status === 401) {
      showToast(t('share.invalidPassword'), 'error')
    } else {
      showToast(t('share.passwordValidationFailed'), 'error')
    }
  }
}

// 下载文件
const downloadFile = async () => {
  // 如果正在下载，则不处理重复点击
  if (isDownloading.value) return;
  
  try {
    isDownloading.value = true;
    
    const validResponse = await validateSharePasswordApi(shortCode, password.value)
    const data = validResponse.data
    // 检查密码验证结果，兼容大小写不同的字段名
    const isValid = data.code === 200 && data.data.isValid
    if (isValid) {
      const response = await downloadFileApi(data.data.fileId, data.data.token)
      
      // 使用隐藏的下载链接确保文件被下载而不是在浏览器中打开
      const link = document.createElement('a');
      link.href = response.data.data.downloadUrl;
      link.download = itemInfo.value.name || 'download';
      link.style.display = 'none';
      document.body.appendChild(link);
      
      // 使用 blob URL 方式创建下载链接，可以更好地控制下载过程
      fetch(response.data.data.downloadUrl)
        .then(resp => resp.blob())
        .then(blob => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.style.display = 'none';
          a.href = url;
          a.download = itemInfo.value.name || 'download';
          document.body.appendChild(a);
          a.click();
          window.URL.revokeObjectURL(url);
          document.body.removeChild(a);
          
          // 下载完成后重置状态
          isDownloading.value = false;
        })
        .catch(() => {
          // 如果 fetch 失败，回退到原来的下载方式
          link.click();
          document.body.removeChild(link);
          
          // 短暂延迟后重置状态
          setTimeout(() => {
            isDownloading.value = false;
          }, 1000);
        });
    } else {
      isDownloading.value = false;
      showToast(t('share.invalidPassword'), 'error')
    }
  } catch (err) {
    isDownloading.value = false;
    // 检查错误响应状态码，如果是401则显示密码错误，而不是跳转到登录页
    if (err.response && err.response.status === 401) {
      showToast(t('share.invalidPassword'), 'error')
    } else {
      showToast(t('share.passwordValidationFailed'), 'error')
    }
  }
}

// 下载文件夹中的文件
const downloadFileFromFolder = async (file) => {
  // 如果正在下载该文件，则不处理重复点击
  if (downloadingFileId.value === file.id) return;
  
  try {
    downloadingFileId.value = file.id;
    
    // 使用已有的token下载文件
    const response = await downloadFileApi(file.id, shareToken.value)
    if (response.data.code === 200) {
      // 使用 blob URL 方式创建下载链接，可以更好地控制下载过程
      fetch(response.data.data.downloadUrl)
        .then(resp => resp.blob())
        .then(blob => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.style.display = 'none';
          a.href = url;
          a.download = file.originalName || 'download';
          document.body.appendChild(a);
          a.click();
          window.URL.revokeObjectURL(url);
          document.body.removeChild(a);
          
          // 下载完成后重置状态
          downloadingFileId.value = null;
        })
        .catch(() => {
          // 如果 fetch 失败，回退到原来的下载方式
          const link = document.createElement('a');
          link.href = response.data.data.downloadUrl;
          link.download = file.originalName || 'download';
          link.style.display = 'none';
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
          
          // 短暂延迟后重置状态
          setTimeout(() => {
            downloadingFileId.value = null;
          }, 1000);
        });
    } else {
      downloadingFileId.value = null;
      showToast(t('share.fileDownloadFailed'), 'error')
    }
  } catch (err) {
    downloadingFileId.value = null;
    console.error('下载文件失败:', err)
    showToast(t('share.fileDownloadFailed'), 'error')
  }
}

// 查看文件夹内容
const viewFolderContent = async () => {
  try {
    loading.value = true
    
    // 如果还没有token，先验证密码获取token
    let token = shareToken.value
    if (!token) {
      const validResponse = await validateSharePasswordApi(shortCode, password.value)
      const data = validResponse.data
      const isValid = data.code === 200 && (data.data.isValid || data.data.IsValid)
      
      if (isValid) {
        token = data.data.token
        shareToken.value = token
      } else {
        showToast(t('share.invalidPassword'), 'error')
        return
      }
    }
    
    // 使用token获取文件夹内容
    const response = await getShareFolderContentApi(folderInfo.value.id, token)
    if (response.data.code === 200) {
      folderContent.value = response.data.data
    } else {
      showToast(t('share.fetchFailed'), 'error')
    }
  } catch (err) {
    console.error('获取文件夹内容失败:', err)
    showToast(t('share.fetchFailed'), 'error')
  } finally {
    loading.value = false
  }
}

// 返回文件夹信息页面
const goBackToFolderInfo = () => {
  folderContent.value = null
}

// 查看项目（文件或文件夹）
const viewItem = (item) => {
  // 对于文件，可以直接下载
  // 对于文件夹，可以进一步处理（如果有嵌套文件夹的话）
  if (item.extension) {
    // 这是一个文件，直接下载
    downloadFileFromFolder(item);
  }
}

// 返回首页
const goHome = () => {
  router.push('/')
}

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message
  toastType.value = type
}

</script>

<style scoped>
.share-container {
  display: flex;
  flex-direction: column;
  padding: 20px;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: 100vh;
  align-items: center;
  justify-content: center;
}

.share-content {
  background-color: white;
  border-radius: 10px;
  padding: 30px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 800px;
}

.loading, .error {
  text-align: center;
  padding: 40px 20px;
}

.error h2 {
  color: #dc3545;
  margin-bottom: 15px;
}

.error p {
  color: #666;
  margin-bottom: 20px;
}

.item-info h2 {
  text-align: center;
  margin-bottom: 30px;
  color: #333;
}

.item-details {
  text-align: center;
  margin-bottom: 30px;
}

.item-icon {
  font-size: 48px;
  margin-bottom: 20px;
}

.item-name {
  font-size: 24px;
  font-weight: bold;
  color: #333;
  margin-bottom: 15px;
  word-break: break-word;
}

.file-meta {
  display: flex;
  justify-content: center;
  gap: 20px;
  font-size: 14px;
  color: #666;
}

.file-type {
  background-color: #e9ecef;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

.password-form {
  background-color: #f8f9fa;
  padding: 20px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.password-form h3 {
  text-align: center;
  margin-bottom: 20px;
  color: #333;
}

.form-group {
  margin-bottom: 20px;
}

.form-group input {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 16px;
  box-sizing: border-box;
}

.form-actions {
  text-align: center;
}

.submit-button {
  padding: 12px 30px;
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
}

.submit-button:hover {
  opacity: 0.9;
}

.action-buttons {
  display: flex;
  justify-content: center;
  gap: 15px;
}

.download-button, .view-button {
  padding: 12px 30px;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  min-width: 120px;
}

.download-button {
  background: linear-gradient(135deg, #17a2b8 0%, #138496 100%);
  color: white;
}

.view-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.download-button:hover, .view-button:hover {
  opacity: 0.9;
}

.download-button:disabled,
.view-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.download-button.small {
  padding: 6px 12px;
  font-size: 14px;
  min-width: auto;
}

.download-button.small:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* 文件夹内容列表样式 */
.folder-items {
  margin-top: 20px;
}

.list-item {
  display: flex;
  align-items: center;
  padding: 15px;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  margin-bottom: 10px;
  background-color: white;
  transition: all 0.3s ease;
  cursor: pointer;
}

.list-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.15);
}

.item-icon {
  font-size: 24px;
  margin-right: 15px;
  flex-shrink: 0;
}

.item-info {
  flex: 1;
  min-width: 0;
}

.item-name {
  font-weight: bold;
  color: #333;
  margin-bottom: 5px;
  word-break: break-word;
}

.item-meta {
  display: flex;
  gap: 15px;
  font-size: 14px;
  color: #666;
}

.item-size,
.item-type,
.item-time {
  white-space: nowrap;
}

.item-type {
  background-color: #e9ecef;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

.item-actions {
  margin-left: 15px;
  flex-shrink: 0;
}

.folder-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.back-button {
  padding: 8px 16px;
  background-color: #6c757d;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.back-button:hover {
  background-color: #5a6268;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .share-container {
    padding: 15px;
  }
  
  .share-content {
    padding: 20px;
  }
  
  .item-name {
    font-size: 20px;
  }
  
  .file-meta {
    flex-direction: column;
    gap: 5px;
  }
  
  .action-buttons {
    flex-direction: column;
    gap: 10px;
  }
  
  .download-button, .view-button {
    width: 100%;
  }
  
  .folder-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }
  
  .list-item {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .item-icon {
    margin-right: 0;
    margin-bottom: 10px;
  }
  
  .item-meta {
    flex-wrap: wrap;
    gap: 10px;
    margin-bottom: 10px;
  }
  
  .item-actions {
    margin-left: 0;
    width: 100%;
    text-align: center;
  }
  
  .download-button.small {
    padding: 6px 10px;
    font-size: 13px;
  }
  
  .download-button.small:disabled {
    opacity: 0.6;
  }
}

@media (max-width: 480px) {
  .share-container {
    padding: 10px;
  }
  
  .share-content {
    padding: 15px;
  }
  
  .item-name {
    font-size: 18px;
  }
  
  .submit-button, .download-button, .view-button {
    padding: 10px 20px;
    font-size: 14px;
  }
  
  .download-button.small {
    padding: 5px 8px;
    font-size: 12px;
  }
  
  .download-button.small:disabled {
    opacity: 0.6;
  }
}
</style>