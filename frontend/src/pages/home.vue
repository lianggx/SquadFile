<template>
  <div class="home-container">
    <!-- 添加 Toast 组件 -->
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <!-- 面包屑导航 -->
    <div class="header" v-if="breadcrumbPath.length > 1">
      <div class="breadcrumb">
        <template v-for="(item, index) in breadcrumbPath" :key="item.id">
          <span 
            class="breadcrumb-item"
            :class="{ active: index === breadcrumbPath.length - 1 }"
            @click="navigateToFolder(item.id)"
          >
            {{ item.name }}
          </span>
          <span v-if="index < breadcrumbPath.length - 1" class="breadcrumb-separator"> / </span>
        </template>
      </div>
    </div>
    
    <!-- 批量操作工具栏 -->
    <div v-if="(hierarchicalFolders.length > 0 || currentFiles.length > 0) && (hasReadPermission || hasUploadPermission || hasDeletePermission || hasCreateFolderPermission)" class="batch-actions">
      <div class="batch-buttons">
        <label class="select-all">
          <input 
            type="checkbox" 
            :checked="selectedFiles.length === currentFiles.length && currentFiles.length > 0" 
            @change="toggleSelectAll"
          />
          {{ t('admin.selectAll') }}
        </label>

        <button 
          v-if="hasReadPermission"
          @click="downloadSelectedFiles" 
          :disabled="selectedFiles.length === 0"
          class="batch-button"
        >
          {{ t('admin.download') }} ({{ selectedFiles.length }})
        </button>
        <button 
          v-if="hasReadPermission"
          @click="showShareModalForSelected" 
          :disabled="selectedFiles.length === 0"
          class="batch-button"
        >
          {{ t('admin.share') }} ({{ selectedFiles.length }})
        </button>
        <button 
          v-if="hasUploadPermission"
          @click="triggerFileSelect"
          class="batch-button"
        >
          {{ t('admin.uploadFile') }}
        </button>
        <button 
          v-if="hasCreateFolderPermission"
          @click="showCreateFolderModal"
          class="batch-button"
        >
          {{ t('admin.createFolder') }}
        </button>
      </div>
    </div>
    
    <!-- 文件上传输入框（隐藏） -->
    <input 
      ref="fileInput"
      type="file" 
      @change="handleFileSelect" 
      style="display: none;"
      multiple
    />
    
    <!-- 用户文件夹和文件列表（合并显示） -->
    <div v-if="(hierarchicalFolders.length > 0 || currentFiles.length > 0)" class="folders-section">
      <h3>{{ getCurrentFolderName() }}</h3>
      <div class="folders-grid">
        <!-- 文件夹列表 -->
        <div 
          v-for="folder in hierarchicalFolders" 
          :key="'folder-'+folder.id" 
          class="folder-card"
          @click="goToFolder(folder.id)"
        >
          <div class="folder-icon">📁</div>
          <div class="folder-name">{{ folder.name }}</div>
          <div class="folder-info">
            <span class="file-count">{{ t('home.fileCount', { count: folder.fileCount }) }}</span>
            <span class="updated-time">{{ formatTime(folder.updatedTime) }}</span>
          </div>
        </div>
        
        <!-- 文件列表 -->
        <FileItem
          v-for="file in currentFiles" 
          :key="'file-'+file.id" 
          :file="file"
          :show-selection="true"
          :selected="selectedFiles.includes(file.id)"
          @update:selected="toggleFileSelection(file.id, $event)"
          @preview="previewFile(file)"
          @download="downloadFile"
        />
      </div>
    </div>
    
    <!-- 空状态提示 -->
    <div v-else class="empty-state">
      <p>{{ t('admin.noFoldersInFolder') }}</p>
    </div>
    
    <!-- 文件分享模态框 -->
    <div v-if="showShareModalFlag" class="modal">
      <div class="modal-content share-modal">
        <h2>{{ sharingMultipleFiles ? t('admin.shareFiles', { count: selectedFiles.length }) : t('admin.shareFile') }}</h2>
        <form @submit.prevent="shareItem">
          <div class="form-group">
            <label>{{ t('admin.sharePassword') }}:</label>
            <input 
              v-model="shareForm.password" 
              type="text" 
              :placeholder="t('admin.optional')"
            />
          </div>
          <div class="form-group">
            <label>{{ t('admin.expireTime') }}:</label>
            <input 
              v-model="shareForm.expireTime" 
              type="datetime-local" 
            />
          </div>
          
          <div class="form-actions">
            <button type="button" @click="closeShareModal" class="cancel-button">
              {{ t('admin.cancel') }}
            </button>
            <button type="submit" class="save-button">
              {{ t('admin.createShare') }}
            </button>
          </div>
          
          <!-- 分享文字信息 -->
          <div v-if="sharedFile && sharedFile.shortCode" class="form-group share-text-section">
            <label>{{ t('admin.shareText') }}:</label>
            <div class="share-text-container">
              <textarea 
                :value="generateFileShareText()" 
                readonly 
                class="share-text-area"
              ></textarea>
              <button 
                type="button" 
                @click="copyFileShareText" 
                class="copy-button"
              >
                {{ t('admin.copyShareText') }}
              </button>
            </div>
            <p class="share-text-description">{{ t('admin.shareTextDescription') }}</p>
          </div>
        </form>
      </div>
    </div>
    
    <!-- 创建文件夹模态框 -->
    <div v-if="showCreateFolderModalFlag" class="modal">
      <div class="modal-content">
        <h2>{{ t('admin.createFolder') }}</h2>
        <form @submit.prevent="createFolder">
          <div class="form-group">
            <label>{{ t('admin.folderName') }}:</label>
            <input v-model="createFolderForm.name" type="text" required />
          </div>
          <div class="form-group">
            <label>{{ t('admin.folderDescription') }}:</label>
            <textarea v-model="createFolderForm.description" rows="3"></textarea>
          </div>
          <div class="form-actions">
            <button type="submit" class="save-button">{{ t('admin.save') }}</button>
            <button type="button" @click="closeCreateFolderModal" class="cancel-button">{{ t('admin.cancel') }}</button>
          </div>
        </form>
      </div>
    </div>
    
    <!-- 预览模态框 -->
    <div v-if="showPreviewModal" class="modal preview-modal">
      <div class="modal-content preview-content">
        <div class="preview-header">
          <h2>{{ previewFileName }}</h2>
          <button @click="closePreviewModal" class="close-button">×</button>
        </div>
        <div v-if="previewLoading" class="preview-loading">
          {{ t('admin.loading') }}
        </div>
        <div v-else-if="previewError" class="preview-error">
          {{ t('admin.previewFailed') }}: {{ previewError }}
        </div>
        <div v-else-if="previewUrl && isImageFile(getFileExtension(previewFileName))" class="image-preview">
          <img :src="previewUrl" alt="预览图片" class="preview-image" />
        </div>
        <div v-else-if="previewUrl && isVideoFile(getFileExtension(previewFileName))" class="video-preview">
          <video controls class="preview-video">
            <source :src="previewUrl" :type="getVideoMimeType(getFileExtension(previewFileName))">
            您的浏览器不支持视频播放。
          </video>
        </div>
        <div v-else-if="previewUrl && isAudioFile(getFileExtension(previewFileName))" class="audio-preview">
          <audio controls class="preview-audio">
            <source :src="previewUrl" :type="getAudioMimeType(getFileExtension(previewFileName))">
            您的浏览器不支持音频播放。
          </audio>
        </div>
        <div v-else-if="previewUrl && isTextFile(getFileExtension(previewFileName))" class="text-preview">
          <iframe 
            :src="previewUrl" 
            class="preview-text-frame"
            frameborder="0"
          ></iframe>
        </div>
        <iframe 
          v-else-if="previewUrl" 
          :src="previewUrl" 
          class="preview-frame"
          frameborder="0"
        ></iframe>
        <div v-else class="preview-not-supported">
          {{ t('admin.previewNotSupported') }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { usePageAuth } from '../composables/useAuth'
import { 
  getUserFoldersApi, 
  getFilesInFolderApi, 
  generateFileDownloadUrlApi, 
  downloadFileApi,
  prepareFileUploadApi,
  uploadFileChunkApi,
  shareFileApi,
  getFolderPermissionsApi,
  createFolderApi
} from '../services/file'

// 导入公共下载服务
import { downloadFile } from '../services/download'
// 导入公共工具函数
import { 
  isImageFile, 
  isVideoFile, 
  isAudioFile, 
  isDocumentFile, 
  isArchiveFile, 
  isCodeFile, 
  isTextFile, 
  formatFileSize, 
  formatTime, 
  getFileTypeDisplayName, 
  getVideoMimeType, 
  getAudioMimeType, 
  getFileExtension 
} from '../utils/fileUtils'

// 导入组件
import FileIcon from '../components/FileIcon.vue'
import FileActions from '../components/FileActions.vue'
import FileItem from '../components/FileItem.vue'
import Toast from '../components/Toast.vue'

const { t } = useI18n()
const router = useRouter()
const authStore = useAuthStore()
const { checkAuthOnMounted, checkAuthOnShow } = usePageAuth()

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 用户文件夹列表
const userFolders = ref([])
// 文件列表
const files = ref([])
// 当前文件夹ID
const currentFolderId = ref(null)
// 面包屑路径
const breadcrumbPath = ref([])

// 选中的文件
const selectedFiles = ref([])

// 文件输入引用
const fileInput = ref(null)

// 分享模态框显示状态
const showShareModalFlag = ref(false)

// 分享表单数据
const shareForm = ref({
  password: '',
  expireTime: ''
})

// 分享多个文件标志
const sharingMultipleFiles = ref(false)
// 添加用于存储已分享文件信息的响应式变量
const sharedFile = ref(null)

// 预览相关数据
const showPreviewModal = ref(false)
const previewLoading = ref(false)
const previewError = ref('')
const previewUrl = ref('')
const previewFileName = ref('')

// 权限相关
const hasUploadPermission = ref(false)
const hasReadPermission = ref(false)
const hasDeletePermission = ref(false)

// 创建文件夹相关
const showCreateFolderModalFlag = ref(false)
const createFolderForm = ref({
  name: '',
  description: '',
  parentId: null
})

// 创建文件夹权限
const hasCreateFolderPermission = ref(false)

// 页面加载时检查认证状态
checkAuthOnMounted(false) // 不自动重定向，让我们自己处理

// 显示的用户名
const displayUserName = computed(() => {
  if (authStore.user) {
    return authStore.user.displayName || authStore.user.username || t('home.user')
  }
  return t('home.user')
})

// 是否显示管理面板
const showAdminPanel = computed(() => {
  return authStore.user && authStore.user.role === 'Admin'
})

// 计算属性：当前层级的文件夹
const hierarchicalFolders = computed(() => {
  if (currentFolderId.value === null) {
    // 根文件夹
    return userFolders.value.filter(folder => folder.parentId === null);
  } else {
    // 子文件夹
    return userFolders.value.filter(folder => folder.parentId === currentFolderId.value);
  }
});

// 计算属性：当前文件夹中的文件
const currentFiles = computed(() => {
  if (currentFolderId.value === null) {
    // 根文件夹中的文件
    return files.value.filter(file => file.folderId === 0 || file.folderId === null);
  } else {
    // 当前文件夹中的文件
    return files.value.filter(file => file.folderId === currentFolderId.value);
  }
});

// 获取当前文件夹名称
const getCurrentFolderName = () => {
  if (currentFolderId.value === null) {
    return t('home.yourFolders');
  } else {
    const currentFolder = userFolders.value.find(f => f.id === currentFolderId.value);
    return currentFolder ? currentFolder.name : t('home.folder');
  }
};

// 获取用户文件夹列表
const fetchUserFolders = async () => {
  try {
    const response = await getUserFoldersApi()
    userFolders.value = response.data.data || []
    updateBreadcrumbPath()
  } catch (error) {
    console.error('获取文件夹列表失败:', error)
  }
}

// 获取文件列表
const fetchFiles = async (folderId) => {
  try {
    const response = await getFilesInFolderApi(folderId || 0)
    files.value = response.data.data || []
  } catch (error) {
    console.error('获取文件列表失败:', error)
  }
}

// 获取文件夹权限
const fetchFolderPermissions = async (folderId) => {
  try {
    // 重置权限
    hasUploadPermission.value = false
    hasReadPermission.value = false
    hasDeletePermission.value = false
    hasCreateFolderPermission.value = false
    
    // 根目录默认只有读取和删除权限，没有上传权限
    if (folderId === null || folderId === 0) {
      hasReadPermission.value = true
      hasDeletePermission.value = true
      // 不设置上传权限为true，根目录默认不能上传文件
      // 根目录默认不能创建文件夹（只有管理员可以在根目录创建文件夹）
      return
    }
    
    const response = await getFolderPermissionsApi(folderId)
    const permissions = response.data.data || []
    
    // 查找当前用户的权限
    const userPermission = permissions.find(p => p.userId === authStore.user.id)
    
    if (userPermission) {
      hasReadPermission.value = userPermission.canRead
      hasUploadPermission.value = userPermission.canUpload
      hasDeletePermission.value = userPermission.canDelete
      hasCreateFolderPermission.value = userPermission.canCreateFolder
    } else {
      // 如果没有找到权限记录，检查是否为文件夹创建者
      const folder = userFolders.value.find(f => f.id === folderId)
      if (folder && folder.createdBy === authStore.user.id) {
        // 文件夹创建者拥有所有权限
        hasReadPermission.value = true
        hasUploadPermission.value = true
        hasDeletePermission.value = true
        hasCreateFolderPermission.value = true
      }
    }
  } catch (error) {
    console.error('获取文件夹权限失败:', error)
    // 出错时默认不显示权限按钮
    hasUploadPermission.value = false
    hasReadPermission.value = false
    hasDeletePermission.value = false
    hasCreateFolderPermission.value = false
  }
}

// 跳转到文件夹详情
const goToFolder = async (folderId) => {
  currentFolderId.value = folderId
  await fetchFiles(folderId)
  await fetchFolderPermissions(folderId)
  updateBreadcrumbPath()
}

// 返回上一级文件夹
const goBack = async () => {
  // 找到当前文件夹的父文件夹
  if (currentFolderId.value === null) return
  
  const currentFolder = userFolders.value.find(f => f.id === currentFolderId.value)
  if (currentFolder) {
    currentFolderId.value = currentFolder.parentId
    await fetchFiles(currentFolder.parentId || 0)
    await fetchFolderPermissions(currentFolder.parentId)
  } else {
    currentFolderId.value = null
    await fetchFiles(0)
    await fetchFolderPermissions(0)
  }
  updateBreadcrumbPath()
}

// 面包屑导航点击
const navigateToFolder = async (folderId) => {
  currentFolderId.value = folderId === 'root' ? null : folderId
  await fetchFiles(folderId || 0)
  await fetchFolderPermissions(folderId || 0)
  updateBreadcrumbPath()
}

// 更新面包屑路径
const updateBreadcrumbPath = () => {
  const path = []
  let folderId = currentFolderId.value
  
  // 从当前文件夹向上遍历到根目录
  while (folderId !== null) {
    const folder = userFolders.value.find(f => f.id === folderId)
    if (folder) {
      path.unshift({
        id: folder.id,
        name: folder.name
      })
      folderId = folder.parentId
    } else {
      break
    }
  }
  
  // 添加根目录
  path.unshift({
    id: null,
    name: t('home.yourFolders')
  })
  
  breadcrumbPath.value = path
}

// 跳转到文件传输页面
const goToFileTransfer = () => {
  // 使用 Toast 替代 alert
  showToast(t('home.featureComingSoon'), 'info')
}

// 跳转到文件管理页面
const goToFileManagement = () => {
  router.push('/admin/folders')
}

// 跳转到个人信息页面
const goToProfile = () => {
  router.push('/profile')
}

// 跳转到管理页面
const goToAdmin = () => {
  router.push('/admin')
}

// 预览文件
const previewFile = async (file) => {
  try {
    console.log('file',file)
    previewLoading.value = true;
    previewError.value = '';
    previewFileName.value = file.originalName;
    
    // 根据文件类型使用不同的预览方式
    if (isImageFile(file.extension) || 
        isVideoFile(file.extension) || 
        isAudioFile(file.extension) || 
        isCodeFile(file.extension) || 
        (isDocumentFile(file.extension) && (file.extension.toLowerCase() === '.txt' || file.extension.toLowerCase() === '.md'))) {
      
      const response = await downloadFileApi(file.id);    
      const downloadUrl = response.data.data?.downloadUrl;
      const fullDownloadUrl = window.location.origin + downloadUrl;
      
      previewUrl.value = fullDownloadUrl;
      showPreviewModal.value = true;
    } else {
      showToast(t('admin.previewNotSupported'), 'error');
      return;
    }
  } catch (error) {
    console.error('文件预览失败:', error);
    previewError.value = error.message;
    showToast(t('admin.previewFailed'), 'error');
  } finally {
    previewLoading.value = false;
  }
};

// 关闭预览模态框
const closePreviewModal = () => {
  showPreviewModal.value = false;
  previewUrl.value = '';
  previewError.value = '';
  previewFileName.value = '';
};

// 下载选中的文件
const downloadSelectedFiles = async () => {
  for (const fileId of selectedFiles.value) {
    await downloadFile(fileId)
  }
}

// 显示选中文件的分享模态框
const showShareModalForSelected = () => {
  sharingMultipleFiles.value = selectedFiles.value.length > 1
  shareForm.value = {
    password: '',
    expireTime: ''
  }
  // 重置已分享文件信息
  sharedFile.value = null
  showShareModalFlag.value = true
}

// 关闭分享模态框
const closeShareModal = () => {
  showShareModalFlag.value = false
  // 重置已分享文件信息
  sharedFile.value = null
}

// 分享文件
const shareItem = async () => {
  try {
    // 只支持单文件分享，多文件分享保持原有逻辑
    if (sharingMultipleFiles.value && selectedFiles.value.length > 1) {
      // 分享多个文件
      const results = []
      for (const fileId of selectedFiles.value) {
        try {
          await shareFileApi(fileId, {
            password: shareForm.value.password || null,
            expireTime: shareForm.value.expireTime || null
          })
          results.push({ fileId, success: true })
        } catch (error) {
          results.push({ fileId, success: false, error })
        }
      }
      
      const successCount = results.filter(r => r.success).length
      if (successCount > 0) {
        showToast(t('admin.filesShared', { count: successCount }), 'success')
      }
      
      if (successCount < results.length) {
        showToast(t('admin.fileShareFailed'), 'error')
      }
      
      closeShareModal()
    } else {
      // 分享单个文件，使用新的逻辑
      const fileId = selectedFiles.value[0]
      const expireTime = shareForm.value.expireTime 
        ? new Date(shareForm.value.expireTime).toISOString() 
        : null
      
      const response = await shareFileApi(fileId, {
        password: shareForm.value.password || null,
        expireTime: expireTime
      })
      
      // 从分享响应中获取短链接信息
      sharedFile.value = {
        id: fileId,
        shortLink: response.data.data.shortLink || response.data.data.ShortLink,
        shortCode: response.data.data.shortCode || response.data.data.ShortCode,
        expireTime: expireTime,
        password: shareForm.value.password || null,
        // 获取文件名用于生成分享文字
        originalName: currentFiles.value.find(f => f.id === fileId)?.originalName || ''
      }
      
      showToast(t('admin.fileShared'), 'success')
      // 不关闭模态框，让用户可以看到分享信息
    }
  } catch (error) {
    console.error('文件分享失败:', error)
    showToast(t('admin.fileShareFailed'), 'error')
  }
}

// 生成文件分享文字
const generateFileShareText = () => {
  if (!sharedFile.value || !sharedFile.value.shortCode) return ''
  
  const fileName = sharedFile.value.originalName || ''
  const shareLink = `${window.location.origin}/share/${sharedFile.value.shortCode}`
  const password = sharedFile.value.password || ''
  
  // 按照指定格式生成分享文字
  let shareText = `${fileName} ${shareLink}`
  if (password) {
    shareText += ` 密码：${password}`
  }
  
  // 添加过期时间信息
  if (sharedFile.value.expireTime) {
    const expireDate = new Date(sharedFile.value.expireTime)
    shareText += ` 过期时间：${expireDate.toLocaleString()}`
  }
  
  return shareText
}

// 复制文件分享文字
const copyFileShareText = () => {
  const shareText = generateFileShareText()
  if (shareText) {
    navigator.clipboard.writeText(shareText)
      .then(() => {
        showToast(t('admin.shareTextCopied'), 'success')
      })
      .catch(() => {
        // 降级方案
        const textArea = document.createElement('textarea')
        textArea.value = shareText
        document.body.appendChild(textArea)
        textArea.select()
        document.execCommand('copy')
        document.body.removeChild(textArea)
        showToast(t('admin.shareTextCopied'), 'success')
      })
  }
}

// 全选/取消全选
const toggleSelectAll = (event) => {
  if (event.target.checked) {
    selectedFiles.value = currentFiles.value.map(file => file.id)
  } else {
    selectedFiles.value = []
  }
}

// 切换文件选择状态
const toggleFileSelection = (fileId, selected) => {
  if (selected) {
    if (!selectedFiles.value.includes(fileId)) {
      selectedFiles.value.push(fileId)
    }
  } else {
    const index = selectedFiles.value.indexOf(fileId)
    if (index > -1) {
      selectedFiles.value.splice(index, 1)
    }
  }
}

// 触发文件选择
const triggerFileSelect = () => {
  fileInput.value?.click()
}

// 处理文件选择
const handleFileSelect = (event) => {
  const files = Array.from(event.target.files)
  if (files.length > 0) {
    uploadFiles(files)
  }
}

// 上传文件
const uploadFiles = async (filesToUpload) => {
  for (const file of filesToUpload) {
    try {
      // 准备上传
      const prepareResponse = await prepareFileUploadApi({
        folderId: currentFolderId.value || 0,
        originalName: file.name,
        size: file.size,
        extension: '.' + file.name.split('.').pop()
      })
      
      // 修复：正确提取 fileId 和 storageName
      const fileId = prepareResponse.data.data?.FileId || prepareResponse.data.FileId || prepareResponse.data?.data?.fileId;
      const storageName = prepareResponse.data.data?.StorageName || prepareResponse.data.StorageName || prepareResponse.data?.data?.storageName;
      
      // 检查 fileId 是否有效
      if (!fileId) {
        throw new Error('未能获取有效的文件ID');
      }
      
      // 分块上传
      const chunkSize = 1024 * 1024 // 1MB
      const totalChunks = Math.ceil(file.size / chunkSize)
      
      for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
        const start = chunkIndex * chunkSize
        const end = Math.min(start + chunkSize, file.size)
        const chunk = file.slice(start, end)
        
        // 上传块
        const chunkResponse = await uploadFileChunkApi(fileId, chunkIndex, totalChunks, chunk)
        
        // 检查块上传是否成功
        if (chunkResponse.data.code !== 200) {
          throw new Error(chunkResponse.data.message || '文件块上传失败')
        }
      }
      
      showToast(t('admin.fileUploadCompleted'), 'success')
    } catch (error) {
      console.error('文件上传失败:', error)
      showToast(t('admin.fileUploadFailed'), 'error')
    }
  }
  
  // 重新获取文件列表
  await fetchFiles(currentFolderId.value || 0)
}

// 页面加载时的处理
onMounted(() => {  
  // 手动检查认证状态
  const isAuthenticated = authStore.checkAuth()
  
  // 如果未认证，重定向到登录页面
  if (!isAuthenticated) {
    router.push('/login')
  } else {
    // 获取用户文件夹列表
    fetchUserFolders()
    // 获取根目录文件
    fetchFiles(0)
    // 获取根目录权限
    fetchFolderPermissions(0)
  }
})

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message
  toastType.value = type
}

// 显示创建文件夹模态框
const showCreateFolderModal = () => {
  createFolderForm.value = {
    name: '',
    description: '',
    parentId: currentFolderId.value
  }
  showCreateFolderModalFlag.value = true
}

// 关闭创建文件夹模态框
const closeCreateFolderModal = () => {
  showCreateFolderModalFlag.value = false
  createFolderForm.value = {
    name: '',
    description: '',
    parentId: null
  }
}

// 创建文件夹
const createFolder = async () => {
  try {
    await createFolderApi(createFolderForm.value)
    showToast(t('admin.folderCreated'), 'success')
    closeCreateFolderModal()
    // 重新获取文件夹列表
    await fetchUserFolders()
    // 重新获取当前文件夹中的文件
    await fetchFiles(currentFolderId.value || 0)
  } catch (error) {
    console.error('创建文件夹失败:', error)
    showToast(t('admin.createFolderFailed'), 'error')
  }
}

</script>

<style scoped>
.batch-buttons button{
  margin-right: 10px;
}
.select-all{
  margin-right: 10px;
}
.home-container {
  display: flex;
  flex-direction: column;
  padding: 30px;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: calc(100vh - 80px);
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding: 10px 0;
}

.breadcrumb {
  display: flex;
  align-items: center;
  font-size: 16px;
  flex-wrap: wrap;
}

.breadcrumb-item {
  cursor: pointer;
  color: #667eea;
  padding: 5px 10px;
  border-radius: 4px;
  transition: background-color 0.3s;
}

.breadcrumb-item:hover {
  background-color: #e0e7ff;
}

.breadcrumb-item.active {
  color: #333;
  font-weight: 600;
  cursor: default;
  background-color: transparent;
}

.breadcrumb-item.active:hover {
  background-color: transparent;
}

.breadcrumb-separator {
  margin: 0 5px;
  color: #999;
}

.back-button {
  padding: 8px 16px;
  background-color: #667eea;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s;
}

.back-button:hover {
  background-color: #5a6fd8;
}

.welcome-section {
  text-align: center;
  margin-bottom: 40px;
  padding: 40px;
  background: white;
  border-radius: 20px;
  box-shadow: 0 5px 25px rgba(0, 0, 0, 0.08);
  animation: fadeIn 0.5s ease-out;
}

.welcome-section h2 {
  font-size: 32px;
  color: #333;
  margin-bottom: 15px;
  font-weight: 700;
}

.welcome-section p {
  font-size: 18px;
  color: #666;
  line-height: 1.6;
  max-width: 600px;
  margin: 0 auto;
}

.user-greeting {
  font-size: 20px;
  color: #667eea;
  margin-top: 20px;
  font-weight: 500;
}

/* 文件夹列表样式 */
.folders-section {
  margin-bottom: 40px;
}

.folders-section h3 {
  font-size: 24px;
  color: #333;
  margin-bottom: 20px;
  font-weight: 600;
}

.folders-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 20px;
}

.folder-card {
  background: white;
  border-radius: 15px;
  padding: 20px;
  text-align: center;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
}

.folder-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.folder-icon {
  font-size: 48px;
  margin-bottom: 15px;
}

/* 文件图标样式 */
.file-icon-image,
.file-icon-video,
.file-icon-audio,
.file-icon-document,
.file-icon-archive,
.file-icon-code,
.file-icon-default {
  font-size: 48px;
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 15px;
}

/* SVG图标样式 */
.file-svg-icon {
  width: 48px;
  height: 48px;
  object-fit: contain;
}

.folder-name {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin-bottom: 10px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.folder-info {
  display: flex;
  justify-content: space-between;
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

/* 文件操作按钮样式 */
.file-actions {
  display: flex;
  gap: 5px;
  margin-top: 10px;
  justify-content: center;
}

.action-button {
  padding: 5px 10px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  white-space: nowrap;
}

.preview-button {
  background-color: #6f42c1;
  color: white;
}

.action-button:hover {
  opacity: 0.8;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 30px;
  margin-bottom: 40px;
}

.feature-card {
  background: white;
  border-radius: 20px;
  padding: 35px 25px;
  text-align: center;
  box-shadow: 0 5px 25px rgba(0, 0, 0, 0.08);
  cursor: pointer;
  transition: all 0.3s ease;
  animation: slideIn 0.5s ease-out;
}

.feature-card:hover {
  transform: translateY(-10px);
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.15);
}

.feature-icon {
  font-size: 56px;
  margin-bottom: 25px;
}

.feature-card h3 {
  font-size: 22px;
  color: #333;
  margin-bottom: 20px;
  font-weight: 700;
}

.feature-card p {
  font-size: 16px;
  color: #666;
  line-height: 1.6;
}

/* 模态框样式 */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background-color: white;
  padding: 30px;
  border-radius: 10px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-content h2 {
  margin-top: 0;
  margin-bottom: 20px;
  color: #333;
  text-align: center;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
  color: #555;
}

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 16px;
  box-sizing: border-box;
}

.form-actions {
  display: flex;
  justify-content: space-between;
  margin-top: 30px;
}

.cancel-button {
  width: auto;
  padding: 12px 24px;
  background-color: #6c757d;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
}

.cancel-button:hover {
  opacity: 0.9;
}

.save-button {
  padding: 12px 24px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
}

.save-button:hover,
.cancel-button:hover {
  opacity: 0.9;
}

/* 分享文字区域样式 */
.share-text-section {
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #eee;
}

.share-text-container {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.share-text-area {
  width: 100%;
  min-height: 80px;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-family: monospace;
  resize: vertical;
}

.copy-button {
  align-self: flex-end;
  padding: 8px 16px;
  background-color: #28a745;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
}

.copy-button:hover {
  background-color: #218838;
}

.share-text-description {
  font-size: 12px;
  color: #666;
  margin: 5px 0 0 0;
}

/* 预览模态框特殊样式 */
.preview-modal {
  z-index: 2000;
}

.preview-content {
  width: 90%;
  max-width: 1200px;
  height: 90vh;
  padding: 0;
  display: flex;
  flex-direction: column;
}

.preview-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 20px;
  background-color: #f8f9fa;
  border-bottom: 1px solid #dee2e6;
}

.preview-header h2 {
  margin: 0;
  font-size: 18px;
  color: #333;
  flex: 1;
  text-align: center;
}

.close-button {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #666;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-button:hover {
  color: #333;
}

.preview-frame {
  flex: 1;
  width: 100%;
  border: none;
}

.image-preview, .video-preview, .audio-preview, .text-preview {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.preview-image {
  max-width: 100%;
  max-height: 100%;
  object-fit: contain;
}

.preview-video {
  max-width: 100%;
  max-height: 100%;
}

.preview-audio {
  width: 100%;
  max-width: 500px;
}

.preview-text-frame {
  flex: 1;
  width: 100%;
  border: none;
}

.preview-loading,
.preview-error,
.preview-not-supported {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  color: #666;
}

.preview-error {
  color: #dc3545;
}

/* 空状态提示 */
.empty-state {
  text-align: center;
  padding: 40px 20px;
  background-color: white;
  border-radius: 10px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
}

.empty-state p {
  color: #666;
  font-size: 16px;
}
</style>

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* 响应式设计 */
@media (max-width: 768px) {
  .home-container {
    padding: 20px;
  }
  
  .welcome-section {
    padding: 30px 20px;
  }
  
  .welcome-section h2 {
    font-size: 26px;
  }
  
  .welcome-section p {
    font-size: 16px;
  }
  
  .folders-grid {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    gap: 15px;
  }
  
  .folder-card {
    padding: 15px;
  }
  
  .folder-icon {
    font-size: 36px;
    margin-bottom: 10px;
  }
  
  /* 文件图标样式 */
  .file-icon-image,
  .file-icon-video,
  .file-icon-audio,
  .file-icon-document,
  .file-icon-archive,
  .file-icon-code,
  .file-icon-default {
    font-size: 36px;
    width: 36px;
    height: 36px;
  }
  
  /* SVG图标样式 */
  .file-svg-icon {
    width: 36px;
    height: 36px;
  }
  
  .folder-name {
    font-size: 16px;
  }
  
  .folder-info {
    font-size: 12px;
  }
  
  .features-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  
  .feature-card {
    padding: 25px 20px;
  }
  
  .feature-icon {
    font-size: 48px;
  }
  
  .feature-card h3 {
    font-size: 20px;
  }
  
  .modal-content {
    padding: 20px;
    width: 95%;
    margin: 20px;
  }
  
  .preview-content {
    width: 95%;
    height: 85vh;
  }
  
  .preview-header h2 {
    font-size: 16px;
  }
}

@media (max-width: 480px) {
  .home-container {
    padding: 15px;
  }
  
  .welcome-section {
    padding: 20px 15px;
  }
  
  .welcome-section h2 {
    font-size: 22px;
  }
  
  .welcome-section p {
    font-size: 14px;
  }
  
  .user-greeting {
    font-size: 18px;
  }
  
  .folders-grid {
    grid-template-columns: repeat(auto-fill, minmax(130px, 1fr));
    gap: 10px;
  }
  
  .folder-card {
    padding: 12px;
  }
  
  .folder-icon {
    font-size: 32px;
  }
  
  /* 文件图标样式 */
  .file-icon-image,
  .file-icon-video,
  .file-icon-audio,
  .file-icon-document,
  .file-icon-archive,
  .file-icon-code,
  .file-icon-default {
    font-size: 32px;
    width: 32px;
    height: 32px;
  }
  
  /* SVG图标样式 */
  .file-svg-icon {
    width: 32px;
    height: 32px;
  }
  
  .folder-name {
    font-size: 14px;
  }
  
  .folder-info {
    flex-direction: column;
    gap: 3px;
  }
  
  .feature-card {
    padding: 20px 15px;
  }
  
  .feature-icon {
    font-size: 40px;
    margin-bottom: 15px;
  }
  
  .feature-card h3 {
    font-size: 18px;
    margin-bottom: 10px;
  }
  
  .feature-card p {
    font-size: 14px;
  }
  
  .breadcrumb {
    font-size: 14px;
  }
  
  .back-button {
    padding: 6px 12px;
    font-size: 12px;
  }
  
  .file-actions {
    flex-direction: column;
    gap: 5px;
  }
  
  .action-button {
    width: 100%;
  }
  
  .modal-content {
    padding: 15px;
  }
  
  .modal-content h2 {
    font-size: 20px;
  }
  
  .form-group label {
    font-size: 14px;
  }
  
  .form-group input,
  .form-group textarea,
  .form-group select {
    padding: 8px;
    font-size: 14px;
  }
}