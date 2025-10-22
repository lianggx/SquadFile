<template>
  <div class="folder-management-container">
    <!-- 添加 Toast 组件 -->
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <div class="header">
      <!-- 面包屑导航 -->
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
    
    <!-- 批量操作工具栏和搜索控件 -->
    <div class="toolbar-container">
      <div v-if="(filteredFolders && filteredFolders.length > 0) || (filteredFiles && filteredFiles.length > 0)" class="batch-actions">
        <div class="batch-buttons">
          <label class="select-all">
            <input 
              type="checkbox" 
              :checked="selectedFiles.length === filteredFiles.length && filteredFiles.length > 0" 
              @change="toggleSelectAll"
            />
            {{ t('admin.selectAll') }}
          </label>

          <button 
            @click="downloadSelected" 
            :disabled="selectedFiles.length === 0 || selectedFiles.every(id => id.startsWith('folder-'))"
            class="batch-button"
          >
            {{ t('admin.download') }} ({{ selectedFiles.filter(id => id.startsWith('file-')).length }})
          </button>
          <button 
            @click="showShareModalForSelected" 
            :disabled="selectedFiles.length === 0"
            class="batch-button"
          >
            {{ t('admin.share') }} ({{ selectedFiles.length }})
          </button>
          <button 
            @click="deleteSelected" 
            :disabled="selectedFiles.length === 0"
            class="batch-button delete-button"
          >
            {{ t('admin.delete') }} ({{ selectedFiles.length }})
          </button>
        </div>
      </div>
      <div v-else class="batch-actions-placeholder"></div>
      
      <!-- 搜索和排序控件 -->
      <div class="search-sort-container">
        <div class="search-box">
          <input 
            v-model="searchQuery" 
            :placeholder="t('admin.searchPlaceholder')" 
            class="search-input"
            @keyup.enter="handleSearch"
          />
          <button @click="handleSearch" class="search-button">
            {{ t('admin.search') }}
          </button>
        </div>
      </div>

      <!-- 创建文件夹按钮 -->
      <div class="toolbar">
        <button @click="showCreateFolderModal(null)" class="create-button">
          {{ t('admin.createFolder') }}
        </button>
      </div>
    </div>
    
    <!-- 文件夹和文件列表（列表式布局） -->
    <div class="content-list">
      <h2 class="section-title">{{ getCurrentFolderName() }}</h2>
      
      <!-- 列表表头 -->
      <div class="list-header">
        <div class="list-header-item list-header-checkbox"></div>
        <div class="list-header-item list-header-name" @click="handleHeaderClick('name')">
          {{ t('admin.folderName') }}
          <span v-if="sortField === 'name'" class="sort-indicator">
            {{ sortOrder === 'asc' ? '↑' : '↓' }}
          </span>
        </div>
        <div class="list-header-item list-header-size" @click="handleHeaderClick('size')">
          {{ t('admin.fileSize') }}
          <span v-if="sortField === 'size'" class="sort-indicator">
            {{ sortOrder === 'asc' ? '↑' : '↓' }}
          </span>
        </div>
        <div class="list-header-item list-header-type">{{ t('admin.fileType') }}</div>
        <div class="list-header-item list-header-time" @click="handleHeaderClick('uploadTime')">
          {{ t('admin.uploadTime') }}
          <span v-if="sortField === 'uploadTime'" class="sort-indicator">
            {{ sortOrder === 'asc' ? '↑' : '↓' }}
          </span>
        </div>
        <div class="list-header-item list-header-user">{{ t('admin.userName') }}</div>
        <div class="list-header-item list-header-actions">{{ t('admin.actions') }}</div>
      </div>
      
      <!-- 文件夹列表 -->
      <div 
        v-for="folder in filteredFolders" 
        :key="'folder-'+folder.id" 
        class="list-item folder-item"
        :class="{ selected: selectedFiles.includes('folder-'+folder.id) }"
      >
        <div class="list-item-checkbox">
          <input 
            type="checkbox" 
            :checked="selectedFiles.includes('folder-'+folder.id)" 
            @change="toggleFileSelection('folder-'+folder.id, $event.target.checked)"
          />
        </div>
        <div class="list-item-name" @click="goToFolder(folder.id)">
          <div class="item-icon">
            <div class="folder-icon">📁</div>
          </div>
          <span class="folder-name">{{ folder.name }}</span>
        </div>
        <div class="list-item-size">-</div>
        <div class="list-item-type">{{ t('admin.folder') }}</div>
        <div class="list-item-time">{{ formatDate(folder.createdTime) }}</div>
        <div class="list-item-user">{{ folder.userName || '-' }}</div>
        <div class="list-item-actions">
          <button @click.stop="showFolderShareModalFunc(folder)" class="action-button share-button">
            {{ t('admin.share') }}
          </button>
          <button @click.stop="showEditFolderModal(folder)" class="action-button edit-button">
            {{ t('admin.edit') }}
          </button>
          <button @click.stop="showDeleteFolderModal(folder)" class="action-button delete-button">
            {{ t('admin.delete') }}
          </button>
        </div>
      </div>
      
      <!-- 文件列表 -->
      <div 
        v-for="file in filteredFiles" 
        :key="'file-'+file.id" 
        class="list-item file-item"
        :class="{ selected: selectedFiles.includes('file-'+file.id) }"
      >
        <div class="list-item-checkbox">
          <input 
            type="checkbox" 
            :checked="selectedFiles.includes('file-'+file.id)" 
            @change="toggleFileSelection('file-'+file.id, $event.target.checked)"
          /> 
        </div>
        <div class="list-item-name" @click="previewFile(file.id)">
          <!-- <input 
            type="checkbox" 
            :checked="selectedFiles.includes('file-'+file.id)" 
            @change="toggleFileSelection('file-'+file.id, $event.target.checked)"
          /> -->
          <FileIcon 
            :extension="file.extension" 
            size="sm"
            class="file-icon"
          />
          <span class="file-name">{{ file.originalName }}</span>
        </div>
        <div class="list-item-size">{{ formatFileSize(file.size) }}</div>
        <div class="list-item-type">{{ getFileTypeDisplayName(file.extension) }}</div>
        <div class="list-item-time">{{ formatDate(file.uploadTime) }}</div>
        <div class="list-item-user">{{ file.userName || '-' }}</div>
        <div class="list-item-actions">
          <button v-if="canPreview(file)" @click.stop="previewFile(file.id)" class="action-button preview-button">
            {{ t('admin.preview') }}
          </button>
          <button @click.stop="showShareModal(file)" class="action-button share-button">
            {{ t('admin.share') }}
          </button>
          <button @click.stop="showDeleteFileModal(file)" class="action-button delete-button">
            {{ t('admin.delete') }}
          </button>
        </div>
      </div>
      
      <!-- 空状态提示 -->
      <div v-if="(!filteredFolders || filteredFolders.length === 0) && (!filteredFiles || filteredFiles.length === 0)" class="empty-state">
        <p>{{ t('admin.noFoldersInFolder') }}</p>
      </div>
    </div>
    
    <!-- 创建/编辑文件夹模态框 -->
    <div v-if="showFolderModal" class="modal">
      <div class="modal-content">
        <h2>{{ editingFolder ? t('admin.editFolder') : t('admin.createFolder') }}</h2>
        <form @submit.prevent="saveFolder">
          <div class="form-group">
            <label>{{ t('admin.folderName') }}:</label>
            <input v-model="folderForm.name" type="text" required />
          </div>
          <div class="form-group">
            <label>{{ t('admin.folderDescription') }}:</label>
            <textarea v-model="folderForm.description" rows="3"></textarea>
          </div>
          <div v-if="folderForm.parentId" class="form-group">
            <label>{{ t('admin.parentFolder') }}:</label>
            <select v-model="folderForm.parentId">
              <option value="">{{ t('admin.noParentFolder') }}</option>
              <option 
                v-for="folder in availableParentFolders" 
                :key="folder.id" 
                :value="folder.id"
              >
                {{ folder.name }}
              </option>
            </select>
          </div>
          <div class="form-group checkbox-group">
            <label class="checkbox-label">
              {{ t('admin.makePublic') }}
              <input type="checkbox" v-model="folderForm.isPublic" />
            </label>
            <div class="checkbox-description">
              {{ t('admin.publicFolderDesc') }}
            </div>
          </div>
          <div class="form-actions">
            <button type="submit" class="save-button">{{ t('admin.save') }}</button>
            <button type="button" @click="closeFolderModal" class="cancel-button">{{ t('admin.cancel') }}</button>
          </div>
        </form>
      </div>
    </div>
    
    <!-- 删除确认模态框 -->
    <div v-if="showDeleteModal" class="modal">
      <div class="modal-content delete-modal">
        <h2 v-if="folderToDelete?.type === 'folder'">{{ t('admin.confirmDeleteFolder', { name: folderToDelete?.data?.name }) }}</h2>
        <h2 v-else-if="folderToDelete?.type === 'file'">{{ t('admin.confirmDeleteFile', { name: folderToDelete?.data?.originalName }) }}</h2>
        <h2 v-else-if="folderToDelete?.type === 'multiple'">{{ t('admin.confirmDeleteMultiple', { count: folderToDelete?.data?.length }) }}</h2>
        <p>{{ t('admin.confirmDeleteFolderContent') }}</p>
        <div class="modal-actions">
          <button @click="confirmDelete" class="delete-button action-button">{{ t('admin.delete') }}</button>
          <button @click="closeDeleteModal" class="cancel-button action-button">{{ t('admin.cancel') }}</button>
        </div>
      </div>
    </div>
    
    <!-- 文件分享模态框 -->
    <div v-if="showShareModalFlag" class="modal">
      <div class="modal-content share-modal">
        <h2>{{ sharingMultipleItems ? t('admin.shareItems', { count: selectedFiles.filter(id => id.startsWith('file-')).length }) : (sharingFolder ? t('admin.shareFolder') : t('admin.shareFile')) }}</h2>
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
              type="date" 
              class="datetime-input"
            />
          </div>
          <div class="form-actions">
            <button type="submit" class="save-button">{{ t('admin.createShare') }}</button>
            <button type="button" @click="closeShareModal" class="cancel-button">{{ t('admin.cancel') }}</button>
          </div>
          
          <!-- 分享文字信息 -->
          <div v-if="sharingItem && sharingItem.shortCode" class="form-group share-text-section">
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
    
    <!-- 文件夹分享模态框 -->
    <div v-if="showFolderShareModal" class="modal">
      <div class="modal-content share-modal">
        <h2>{{ t('admin.shareFolder') }}</h2>
        <form @submit.prevent="shareFolder">
          <div class="form-group">
            <label>{{ t('admin.sharePassword') }}:</label>
            <input 
              v-model="folderShareForm.password" 
              type="text" 
              :placeholder="t('admin.optional')"
            />
          </div>
          <div class="form-group">
            <label>{{ t('admin.expireTime') }}:</label>
            <input 
              v-model="folderShareForm.expireTime" 
              type="date" 
              class="datetime-input"
            />
          </div>
          <div class="form-actions">
            <button type="submit" class="save-button">{{ t('admin.createShare') }}</button>
            <button type="button" @click="closeFolderShareModal" class="cancel-button">{{ t('admin.cancel') }}</button>
          </div>
          
          <!-- 分享文字信息 -->
          <div v-if="sharingFolder && sharingFolder.shortCode" class="form-group share-text-section">
            <label>{{ t('admin.shareText') }}:</label>
            <div class="share-text-container">
              <textarea 
                :value="generateFolderShareText()" 
                readonly 
                class="share-text-area"
              ></textarea>
              <button 
                type="button" 
                @click="copyFolderShareText" 
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
import { ref, onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { 
  getUserFoldersApi, 
  getFilesInFolderApi,
  createFolderApi,
  updateFolderApi,
  generateFileDownloadUrlApi,
  deleteFolderApi,
  shareFileApi,
  shareFolderApi,
  deleteFileApi, // 添加删除文件API
  downloadFileApi,
  searchItemsApi // 添加搜索API
} from '../../services/file'

// 导入公共下载服务
import { downloadFile } from '../../services/download'
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
  formatDate, 
  getFileTypeDisplayName, 
  getVideoMimeType, 
  getAudioMimeType, 
  getFileExtension 
} from '../../utils/fileUtils'

// 导入 Toast 组件
import Toast from '../../components/Toast.vue'
import FileIcon from '../../components/FileIcon.vue'

const { t } = useI18n()

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 搜索和排序相关数据
const searchQuery = ref('')
const sortField = ref('uploadTime')
const sortOrder = ref('desc') // 'asc' 或 'desc'

// 预览相关数据
const showPreviewModal = ref(false)
const previewLoading = ref(false)
const previewError = ref('')
const previewUrl = ref('')
const previewFileName = ref('')

// 当前文件夹ID
const currentFolderId = ref(null)

// 文件夹和文件数据
const folders = ref([])
const files = ref([])

// 面包屑路径
const breadcrumbPath = ref([])

// 模态框显示状态
const showFolderModal = ref(false)
const showDeleteModal = ref(false)
const showShareModalFlag = ref(false)
const showFolderShareModal = ref(false)

// 当前文件夹
const currentFolder = ref(null)

// 编辑中的文件夹
const editingFolder = ref(null)

// 文件夹表单数据
const folderForm = ref({
  name: '',
  description: '',
  parentId: null,
  isPublic: false
})

// 要删除的文件夹/文件信息
const folderToDelete = ref(null)

// 分享的项目
const sharingItem = ref(null)
const sharingFolder = ref(null)
const sharingMultipleItems = ref(false)

// 分享表单数据
const shareForm = ref({
  password: '',
  expireTime: ''
})
const folderShareForm = ref({
  password: '',
  expireTime: ''
})

// 日期选择器相关
const showFileDatePicker = ref(false)
const showFolderDatePicker = ref(false)
const currentPickerType = ref('')
const tempDate = ref('')
const tempTime = ref('')

// 选中的文件和文件夹
const selectedFiles = ref([])

// 切换文件夹选择
const toggleSelection = (id) => {
  const index = selectedFiles.value.indexOf(id)
  if (index > -1) {
    // 如果已选中，则取消选中
    selectedFiles.value.splice(index, 1)
  } else {
    // 如果未选中，则选中
    selectedFiles.value.push(id)
  }
}

// 切换文件选择
const toggleFileSelection = (id, selected) => {
  const index = selectedFiles.value.indexOf(id)
  if (selected && index === -1) {
    // 如果要选中且未选中，则添加
    selectedFiles.value.push(id)
  } else if (!selected && index > -1) {
    // 如果要取消选中且已选中，则移除
    selectedFiles.value.splice(index, 1)
  }
}

// 全选/取消全选
const toggleSelectAll = () => {
  // 如果当前已全选，则清空选择
  if (selectedFiles.value.length === currentFiles.value.length && currentFiles.value.length > 0) {
    selectedFiles.value = []
  } else {
    // 否则选择所有文件（只选择文件，不选择文件夹）
    selectedFiles.value = [
      ...currentFiles.value.map(file => `file-${file.id}`)
    ]
  }
}

// 计算属性 - 过滤和排序后的文件夹
const filteredFolders = computed(() => {
  if (!hierarchicalFolders.value) return [];
  
  // 先过滤
  let result = hierarchicalFolders.value;
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter(folder => 
      folder.name.toLowerCase().includes(query) ||
      (folder.description && folder.description.toLowerCase().includes(query))
    );
  }
  
  // 再排序
  return result.sort((a, b) => {
    let aValue, bValue;
    
    switch (sortField.value) {
      case 'name':
        aValue = a.name.toLowerCase();
        bValue = b.name.toLowerCase();
        break;
      case 'size':
        // 文件夹没有大小概念，按名称排序
        aValue = a.name.toLowerCase();
        bValue = b.name.toLowerCase();
        break;
      case 'uploadTime':
        aValue = new Date(a.createdTime);
        bValue = new Date(b.createdTime);
        break;
      default:
        return 0;
    }
    
    if (sortOrder.value === 'asc') {
      return aValue > bValue ? 1 : aValue < bValue ? -1 : 0;
    } else {
      return aValue < bValue ? 1 : aValue > bValue ? -1 : 0;
    }
  });
});

const hierarchicalFolders = computed(() => {
  if (!folders.value) return [];
  
  // 如果正在进行搜索，显示所有搜索结果文件夹，不进行层级过滤
  if (searchQuery.value.trim() !== '') {
    return folders.value;
  }
  
  if (currentFolderId.value === null) {
    // 根文件夹
    return folders.value.filter(folder => folder.parentId === null);
  } else {
    // 子文件夹
    return folders.value.filter(folder => folder.parentId === currentFolderId.value);
  }
});

const currentFiles = computed(() => {
  if (!files.value) return [];
  
  // 如果正在进行搜索，显示所有搜索结果文件，不进行层级过滤
  if (searchQuery.value.trim() !== '') {
    return files.value;
  }
  
  if (currentFolderId.value === null) {
    // 根文件夹中的文件
    return files.value.filter(file => file.folderId === 0 || file.folderId === null);
  } else {
    // 当前文件夹中的文件
    return files.value.filter(file => file.folderId === currentFolderId.value);
  }
});

// 过滤和排序后的文件
const filteredFiles = computed(() => {
  if (!currentFiles.value) return [];
  
  // 先过滤
  let result = currentFiles.value;
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter(file => 
      file.originalName.toLowerCase().includes(query) ||
      (file.description && file.description.toLowerCase().includes(query))
    );
  }
  
  // 再排序
  return result.sort((a, b) => {
    let aValue, bValue;
    
    switch (sortField.value) {
      case 'name':
        aValue = a.originalName.toLowerCase();
        bValue = b.originalName.toLowerCase();
        break;
      case 'size':
        aValue = a.size;
        bValue = b.size;
        break;
      case 'uploadTime':
        aValue = new Date(a.uploadTime);
        bValue = new Date(b.uploadTime);
        break;
      default:
        return 0;
    }
    
    if (sortOrder.value === 'asc') {
      return aValue > bValue ? 1 : aValue < bValue ? -1 : 0;
    } else {
      return aValue < bValue ? 1 : aValue > bValue ? -1 : 0;
    }
  });
});

// 页面加载时获取数据
onMounted(() => {
  fetchFolders()
  fetchFiles(currentFolderId.value || 0) // 加载根文件夹的文件
  updateBreadcrumbPath()
})

// 获取文件夹列表
const fetchFolders = async () => {
  try {
    const response = await getUserFoldersApi()
    folders.value = response.data.data || []
  } catch (error) {
    console.error('获取文件夹列表失败:', error)
    showToast(t('admin.fetchFoldersFailed'), 'error')
  }
}

// 获取文件列表
const fetchFiles = async (folderId) => {
  try {
    const response = await getFilesInFolderApi(folderId || 0)
    files.value = response.data.data || []
  } catch (error) {
    console.error('获取文件列表失败:', error)
    showToast(t('admin.fetchFilesFailed'), 'error')
  }
}

// 处理搜索
const handleSearch = async () => {
  if (searchQuery.value.trim() === '') {
    // 如果搜索关键词为空，重新加载数据
    await fetchFolders();
    await fetchFiles(currentFolderId.value || 0);
    return;
  }
  
  try {
    const response = await searchItemsApi(searchQuery.value, currentFolderId.value || 0);
    if (response.data.code === 200) {
      const result = response.data.data;
      folders.value = result.folders || [];
      files.value = result.files || [];
    } else {
      showToast(t('admin.searchFailed'), 'error');
    }
  } catch (error) {
    console.error('搜索失败:', error);
    showToast(t('admin.searchFailed'), 'error');
  }
};

// 显示创建文件夹模态框
const showCreateFolderModal = (parentFolder) => {
  editingFolder.value = null
  folderForm.value = {
    name: '',
    description: '',
    parentId: parentFolder ? parentFolder.id : currentFolderId.value,
    isPublic: false
  }
  showFolderModal.value = true
}

// 显示编辑文件夹模态框
const showEditFolderModal = (folder) => {
  editingFolder.value = folder
  folderForm.value = {
    name: folder.name,
    description: folder.description,
    parentId: folder.parentId,
    isPublic: folder.isPublic
  }
  showFolderModal.value = true
}

// 关闭文件夹模态框
const closeFolderModal = () => {
  showFolderModal.value = false
  editingFolder.value = null
}

// 保存文件夹（创建或更新）
const saveFolder = async () => {
  try {
    if (editingFolder.value) {
      // 更新文件夹
      await updateFolderApi(editingFolder.value.id, folderForm.value)
      showToast(t('admin.folderUpdated'), 'success')
    } else {
      // 创建文件夹
      await createFolderApi(folderForm.value)
      showToast(t('admin.folderCreated'), 'success')
    }
    
    closeFolderModal()
    await fetchFolders()
  } catch (error) {
    console.error('保存文件夹失败:', error)
    showToast(editingFolder.value ? t('admin.updateFolderFailed') : t('admin.createFolderFailed'), 'error')
  }
}

// 获取父文件夹名称
const getParentFolderName = (parentId) => {
  const parentFolder = folders.value.find(f => f.id === parentId)
  return parentFolder ? parentFolder.name : ''
}

// 跳转到文件夹（进入子文件夹）
const goToFolder = async (folderId) => {
  currentFolderId.value = folderId
  await fetchFiles(folderId)
  updateBreadcrumbPath()
}

// 返回上一级文件夹
const goBack = async () => {
  // 找到当前文件夹的父文件夹
  if (currentFolderId.value === null) return
  
  const currentFolder = folders.value.find(f => f.id === currentFolderId.value)
  if (currentFolder) {
    currentFolderId.value = currentFolder.parentId
    await fetchFiles(currentFolder.parentId || 0)
  } else {
    currentFolderId.value = null
    await fetchFiles(0)
  }
  updateBreadcrumbPath()
}

// 更新面包屑路径
const updateBreadcrumbPath = () => {
  const path = []
  let folderId = currentFolderId.value
  
  // 从当前文件夹向上遍历到根目录
  while (folderId !== null) {
    const folder = folders.value.find(f => f.id === folderId)
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
    name: t('admin.folderManagement')
  })
  
  breadcrumbPath.value = path
}

// 面包屑导航点击
const navigateToFolder = async (folderId) => {
  currentFolderId.value = folderId
  await fetchFiles(folderId || 0)
  updateBreadcrumbPath()
}

// 批量下载
const downloadSelected = async () => {
  const selectedFileIds = selectedFiles.value
    .filter(id => id.startsWith('file-'))
    .map(id => parseInt(id.replace('file-', '')));
  
  // 只下载文件，不处理文件夹
  if (selectedFileIds.length > 0) {
    try {
      // 逐个下载选中的文件
      for (const fileId of selectedFileIds) {
        const response = await downloadFileApi(fileId);
        if (response.data.code === 200) {
          const downloadUrl = response.data.data.downloadUrl;
          // 使用公共下载服务
          const file = currentFiles.value.find(f => f.id === fileId);
          if (file) {
            await downloadFile(downloadUrl, file.originalName, fileId);
          } else {
            // 回退到原来的方式
            const link = document.createElement('a');
            link.href = downloadUrl;
            link.target = '_blank';
            link.click();
          }
        } else {
          showToast(t('admin.fileDownloadFailed'), 'error');
        }
      }
      showToast(t('admin.filesDownloaded'), 'success');
    } catch (error) {
      console.error('批量下载失败:', error);
      showToast(t('admin.fileDownloadFailed'), 'error');
    }
  }
};

// 显示文件分享模态框
const showShareModal = async (file) => {
  sharingItem.value = file
  sharingFolder.value = null
  shareForm.value = {
    password: '',
    expireTime: ''
  }
  
  showShareModalFlag.value = true
}

// 显示文件夹分享模态框
const showFolderShareModalFunc = async (folder) => {
  sharingFolder.value = folder;
  sharingItem.value = null;
  folderShareForm.value = {
    password: '',
    expireTime: ''
  };
  
  showFolderShareModal.value = true;
}

// 关闭文件分享模态框
const closeShareModal = () => {
  showShareModalFlag.value = false
  sharingItem.value = null
}

// 关闭文件夹分享模态框
const closeFolderShareModal = () => {
  showFolderShareModal.value = false
  sharingFolder.value = null
}

// 批量分享
const showShareModalForSelected = () => {
  const selectedFileIds = selectedFiles.value
    .filter(id => id.startsWith('file-'))
    .map(id => parseInt(id.replace('file-', '')));
    
  const selectedFolderIds = selectedFiles.value
    .filter(id => id.startsWith('folder-'))
    .map(id => parseInt(id.replace('folder-', '')));
  
  // 如果同时选择了文件和文件夹，分别处理
  if (selectedFileIds.length > 0 || selectedFolderIds.length > 0) {
    sharingMultipleItems.value = (selectedFileIds.length + selectedFolderIds.length) > 1;
    shareForm.value = {
      password: '',
      expireTime: ''
    };
    folderShareForm.value = {
      password: '',
      expireTime: ''
    };
    // 重置已分享文件信息
    sharingItem.value = null;
    sharingFolder.value = null;
    
    // 如果只选择了文件夹，显示文件夹分享模态框
    if (selectedFileIds.length === 0 && selectedFolderIds.length > 0) {
      // 对于文件夹分享，我们只处理第一个选中的文件夹
      const folderId = selectedFolderIds[0];
      const folder = hierarchicalFolders.value.find(f => f.id === folderId);
      if (folder) {
        showFolderShareModalFunc(folder);
      }
    } else {
      // 显示文件分享模态框
      showShareModalFlag.value = true;
    }
  }
};

// 分享文件（优化版本，参考 home.vue 实现）
const shareItem = async () => {
  try {
    const selectedFileIds = selectedFiles.value
      .filter(id => id.startsWith('file-'))
      .map(id => parseInt(id.replace('file-', '')));
    
    // 支持单文件和多文件分享
    if (sharingMultipleItems.value && selectedFileIds.length > 1) {
      // 分享多个文件
      const results = [];
      for (const fileId of selectedFileIds) {
        try {
          const expireTime = shareForm.value.expireTime 
            ? new Date(shareForm.value.expireTime).toISOString() 
            : null;
          
          await shareFileApi(fileId, {
            password: shareForm.value.password || null,
            expireTime: expireTime
          });
          results.push({ fileId, success: true });
        } catch (error) {
          results.push({ fileId, success: false, error });
        }
      }
      
      const successCount = results.filter(r => r.success).length;
      if (successCount > 0) {
        showToast(t('admin.filesShared', { count: successCount }), 'success');
      }
      
      if (successCount < results.length) {
        showToast(t('admin.itemShareFailed'), 'error');
      }
      
      closeShareModal();
    } else if (selectedFileIds.length === 1) {
      // 分享单个文件，使用改进的逻辑
      const fileId = selectedFileIds[0];
      const expireTime = shareForm.value.expireTime 
        ? new Date(shareForm.value.expireTime).toISOString() 
        : null;

      const response = await shareFileApi(fileId, {
        password: shareForm.value.password || null,
        expireTime: expireTime
      });

      // 从分享响应中获取短链接信息
      // 获取文件名用于生成分享文字
      const file = currentFiles.value.find(f => f.id === fileId);
      
      sharingItem.value = {
        id: fileId,
        shortLink: response.data.data.shortLink || response.data.data.ShortLink,
        shortCode: response.data.data.shortCode || response.data.data.ShortCode,
        expireTime: expireTime,
        password: shareForm.value.password || null,
        originalName: file?.originalName || ''
      };

      showToast(t('admin.fileShared'), 'success');
      // 不关闭模态框，让用户可以看到分享信息
    }
  } catch (error) {
    console.error('文件分享失败:', error);
    showToast(t('admin.fileShareFailed'), 'error');
  }
};

// 生成文件分享文字（优化版本，参考 home.vue 实现）
const generateFileShareText = () => {
  if (!sharingItem.value || !sharingItem.value.shortCode) return '';
  
  const fileName = sharingItem.value.originalName || '';
  const shareLink = `${window.location.origin}/share/${sharingItem.value.shortCode}`;
  const password = sharingItem.value.password || '';
  
  // 按照指定格式生成分享文字
  let shareText = `${fileName} ${shareLink}`;
  if (password) {
    shareText += ` 密码：${password}`;
  }
  
  // 添加过期时间信息
  if (sharingItem.value.expireTime) {
    const expireDate = new Date(sharingItem.value.expireTime);
    shareText += ` 过期时间：${expireDate.toLocaleDateString()}`;
  }
  
  return shareText;
}

// 分享文件夹
const shareFolder = async () => {
  try {
    const expireTime = folderShareForm.value.expireTime 
      ? new Date(folderShareForm.value.expireTime).toISOString() 
      : null;

    if (sharingFolder.value) {
      // 分享文件夹
      const response = await shareFolderApi(sharingFolder.value.id, {
        password: folderShareForm.value.password || null,
        expireTime: expireTime
      });

      // 从分享响应中获取短链接信息
      // 使用展开运算符创建新对象以确保响应性
      sharingFolder.value = {
        ...sharingFolder.value,
        shortLink: response.data.data.shortLink || response.data.data.ShortLink,
        shortCode: response.data.data.shortCode || response.data.data.ShortCode,
        expireTime: expireTime,
        password: folderShareForm.value.password || null // 添加密码信息
      };

      showToast(t('admin.folderShared'), 'success');
    }
    
    // 注意：不要关闭模态框，让用户可以看到分享信息
  } catch (error) {
    console.error('文件夹分享失败:', error);
    showToast(t('admin.folderShareFailed'), 'error');
  }
}

// 生成文件夹分享文字
const generateFolderShareText = () => {
  if (!sharingFolder.value || !sharingFolder.value.shortCode) return ''
  
  const folderName = sharingFolder.value.name || ''
  const shareLink = `${window.location.origin}/share/${sharingFolder.value.shortCode}`
  // 使用存储在sharingFolder中的密码值
  const password = sharingFolder.value.password || ''
  
  // 按照指定格式生成分享文字
  let shareText = `${folderName} ${shareLink}`
  if (password) {
    shareText += ` 密码：${password}`
  }
  
  // 添加过期时间信息
  if (sharingFolder.value.expireTime) {
    const expireDate = new Date(sharingFolder.value.expireTime);
    shareText += ` 过期时间：${expireDate.toLocaleDateString()}`;
  }
  
  return shareText
}

// 复制分享链接
const copyShareLink = () => {
  if (sharingItem.value && sharingItem.value.shortLink) {
    navigator.clipboard.writeText(sharingItem.value.shortLink)
      .then(() => {
        showToast(t('admin.linkCopied'), 'success')
      })
      .catch(() => {
        // 降级方案
        const textArea = document.createElement('textarea')
        textArea.value = sharingItem.value.shortLink
        document.body.appendChild(textArea)
        textArea.select()
        document.execCommand('copy')
        document.body.removeChild(textArea)
        showToast(t('admin.linkCopied'), 'success')
      })
  }
}

// 复制文件分享文字（优化版本，参考 home.vue 实现）
const copyFileShareText = () => {
  const shareText = generateFileShareText();
  if (shareText) {
    navigator.clipboard.writeText(shareText)
      .then(() => {
        showToast(t('admin.shareTextCopied'), 'success');
      })
      .catch(() => {
        // 降级方案
        const textArea = document.createElement('textarea');
        textArea.value = shareText;
        document.body.appendChild(textArea);
        textArea.select();
        document.execCommand('copy');
        document.body.removeChild(textArea);
        showToast(t('admin.shareTextCopied'), 'success');
      });
  }
};

// 复制文件夹分享文字
const copyFolderShareText = () => {
  const shareText = generateFolderShareText()
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

// 处理表头点击事件
const handleHeaderClick = (field) => {
  if (sortField.value === field) {
    // 如果点击的是当前排序字段，则切换排序顺序
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc';
  } else {
    // 如果点击的是其他字段，则按该字段升序排序
    sortField.value = field;
    sortOrder.value = 'asc';
  }
};

// 显示删除文件模态框
const showDeleteFileModal = (file) => {
  folderToDelete.value = {
    type: 'file',
    data: file
  };
  showDeleteModal.value = true;
};

// 确认删除文件
const confirmDeleteFile = async () => {
  if (folderToDelete.value && folderToDelete.value.type === 'file') {
    const file = folderToDelete.value.data;
    try {
      await deleteFileApi(file.id);
      await fetchFiles(currentFolderId.value || 0);
      showToast(t('admin.fileDeleted'), 'success');
    } catch (error) {
      console.error('删除文件失败:', error);
      showToast(t('admin.deleteFileFailed'), 'error');
    }
  }
  closeDeleteModal();
};

// 批量删除
const deleteSelected = () => {
  // 获取选中的文件和文件夹
  const selectedFileIds = selectedFiles.value
    .filter(id => id.startsWith('file-'))
    .map(id => parseInt(id.replace('file-', '')));
    
  const selectedFolderIds = selectedFiles.value
    .filter(id => id.startsWith('folder-'))
    .map(id => parseInt(id.replace('folder-', '')));
    
  if (selectedFileIds.length > 0 || selectedFolderIds.length > 0) {
    folderToDelete.value = {
      type: 'multiple',
      data: {
        fileIds: selectedFileIds,
        folderIds: selectedFolderIds
      }
    };
    showDeleteModal.value = true;
  }
};

// 修改确认删除函数以支持批量删除
const confirmDelete = async () => {
  if (folderToDelete.value && folderToDelete.value.type === 'multiple') {
    await confirmDeleteMultiple();
  } else if (folderToDelete.value && folderToDelete.value.type === 'folder') {
    const folder = folderToDelete.value.data;
    try {
      await deleteFolderApi(folder.id);
      await fetchFolders();
      showToast(t('admin.folderDeleted'), 'success');
    } catch (error) {
      console.error('删除文件夹失败:', error);
      showToast(t('admin.deleteFolderFailed'), 'error');
    }
  } else if (folderToDelete.value && folderToDelete.value.type === 'file') {
    // 处理文件删除
    await confirmDeleteFile();
    return;
  }
  closeDeleteModal();
};

// 确认批量删除
const confirmDeleteMultiple = async () => {
  if (folderToDelete.value && folderToDelete.value.type === 'multiple') {
    const { fileIds, folderIds } = folderToDelete.value.data;
    let successCount = 0;
    let failCount = 0;
    
    // 删除文件
    for (const fileId of fileIds) {
      try {
        await deleteFileApi(fileId);
        successCount++;
      } catch (error) {
        console.error('删除文件失败:', error);
        failCount++;
      }
    }
    
    // 删除文件夹
    for (const folderId of folderIds) {
      try {
        await deleteFolderApi(folderId);
        successCount++;
      } catch (error) {
        console.error('删除文件夹失败:', error);
        failCount++;
      }
    }
    
    // 刷新数据
    await fetchFolders();
    await fetchFiles(currentFolderId.value || 0);
    
    // 清空选择
    selectedFiles.value = [];
    
    if (successCount > 0) {
      showToast(t('admin.itemsDeleted', { count: successCount }), 'success');
    }
    
    if (failCount > 0) {
      showToast(t('admin.itemDeleteFailed'), 'error');
    }
  }
};

// 关闭删除模态框
const closeDeleteModal = () => {
  showDeleteModal.value = false
  folderToDelete.value = null
}

// 预览文本文件
const previewTextFile = async (fileId) => {
  try {
    // 生成临时下载链接
    const response = await generateFileDownloadUrlApi(fileId)
    // 修复：正确提取 downloadUrl
    const downloadUrl = response.data.data?.downloadUrl || response.data.downloadUrl
    const fullDownloadUrl = window.location.origin + downloadUrl
    
    // 获取文件内容
    const fileResponse = await fetch(fullDownloadUrl)
    const fileContent = await fileResponse.text()
    
    // 对于文本文件，我们将内容设置为previewUrl，然后在模态框中特殊处理
    previewUrl.value = `data:text/plain;charset=utf-8,${encodeURIComponent(fileContent)}`
    showPreviewModal.value = true
  } catch (error) {
    console.error('文本文件预览失败:', error)
    throw error
  }
}

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message
  toastType.value = type
}

// 获取当前文件夹名称（用于面包屑导航和标题）
const getCurrentFolderName = () => {
  if (currentFolderId.value === null) {
    return t('admin.folderManagement')
  }
  
  const folder = folders.value.find(f => f.id === currentFolderId.value)
  return folder ? folder.name : t('admin.folder')
}

// 显示日期选择器
const showDatePicker = (type) => {
  currentPickerType.value = type
  if (type === 'file') {
    showFileDatePicker.value = true
    showFolderDatePicker.value = false
  } else {
    showFolderDatePicker.value = true
    showFileDatePicker.value = false
  }
  
  // 初始化临时日期和时间
  const now = new Date()
  tempDate.value = now.toISOString().split('T')[0]
  tempTime.value = now.toTimeString().slice(0, 5)
}

// 隐藏日期选择器
const hideDatePicker = () => {
  showFileDatePicker.value = false
  showFolderDatePicker.value = false
  currentPickerType.value = ''
}

// 确认选择的日期
const confirmDate = (type) => {
  if (tempDate.value && tempTime.value) {
    // 合并日期和时间
    const dateTime = `${tempDate.value}T${tempTime.value}`
    
    // 设置到相应的表单字段
    if (type === 'file') {
      shareForm.value.expireTime = dateTime
    } else {
      folderShareForm.value.expireTime = dateTime
    }
  }
  
  // 隐藏选择器
  hideDatePicker()
}

const canPreview = (file) => { 
  return isImageFile(file.extension) || 
        isVideoFile(file.extension) || 
        isAudioFile(file.extension) || 
        isCodeFile(file.extension) || 
        isTextFile(file.extension)
}

// 预览文件
const previewFile = async (fileId) => {
  try {
    // 获取要预览的文件信息
    const file = currentFiles.value.find(f => f.id === fileId);
    if (!file) {
      showToast(t('admin.fileNotFound'), 'error');
      return;
    }
    
     if (!canPreview(file)) {
      // showToast(t('admin.previewNotSupported'), 'error');
      return;
    }

    previewFileName.value = file.originalName;
    const response = await downloadFileApi(fileId);    
    const downloadUrl = response.data.data?.downloadUrl;
    previewUrl.value = downloadUrl;
    showPreviewModal.value = true;
  } catch (error) {
    console.error('文件预览失败:', error);
    previewError.value = error.message;
    showToast(t('admin.previewFailed'), 'error');
  }
};

// 关闭预览模态框
const closePreviewModal = () => {
  showPreviewModal.value = false;
  previewUrl.value = '';
  previewError.value = '';
  previewFileName.value = '';
};

// 计算可用的父文件夹列表（排除当前正在编辑的文件夹及其子文件夹）
const availableParentFolders = computed(() => {
  if (!folders.value) return [];
  
  // 如果是创建新文件夹，所有文件夹都可以作为父文件夹
  if (!editingFolder.value) {
    return folders.value;
  }
  
  // 如果是编辑文件夹，需要排除当前文件夹及其子文件夹
  const excludeFolderIds = [editingFolder.value.id];
  
  // 递归查找所有子文件夹
  const findChildFolders = (parentId) => {
    const children = folders.value.filter(f => f.parentId === parentId);
    children.forEach(child => {
      excludeFolderIds.push(child.id);
      findChildFolders(child.id);
    });
  };
  
  findChildFolders(editingFolder.value.id);
  
  // 返回不包含当前文件夹及其子文件夹的列表
  return folders.value.filter(f => !excludeFolderIds.includes(f.id));
});

</script>

<style>
.breadcrumb-item:hover {
  background-color: #f0f0f0;
}

.breadcrumb-item.active {
  color: #333;
  font-weight: bold;
  cursor: default;
  background-color: transparent;
}

.breadcrumb-separator {
  margin: 0 4px;
  color: #666;
}

.back-button {
  padding: 8px 16px;
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.2s;
}

.back-button:hover {
  background-color: #e0e0e0;
}

.batch-actions {
  padding: 12px 16px;
}

.batch-buttons {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.select-all {
  display: flex;
  align-items: center;
  gap: 6px;
  font-weight: 500;
  cursor: pointer;
  user-select: none;
}

.batch-button {
  padding: 8px 16px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.2s;
  display: flex;
  align-items: center;
  gap: 4px;
}

.batch-button:hover:not(:disabled) {
  background-color: #0056b3;
}

.batch-button:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
  opacity: 0.6;
}

.delete-button {
  background-color: #dc3545;
}

.delete-button:hover:not(:disabled) {
  background-color: #c82333;
}

.toolbar {
  display: flex;
  justify-content: flex-end;
}

.create-button {
  padding: 10px 20px;
  background-color: #28a745;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background-color 0.2s;
}

.create-button:hover {
  background-color: #218838;
}

.content-list {
  margin-bottom: 30px;
}

.section-title {
  font-size: 20px;
  font-weight: 600;
  margin-bottom: 16px;
  color: #333;
}

.items-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
}

.item {
  display: flex;
  padding: 16px;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  transition: box-shadow 0.2s, border-color 0.2s;
  background-color: white;
  position: relative;
  cursor: pointer;
}

.item:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border-color: #ccc;
}

.folder-item {
  flex-direction: column;
}

.item-selection {
  position: absolute;
  top: 12px;
  left: 12px;
  z-index: 1;
}

.item-icon {
  font-size: 24px;
  margin-bottom: 12px;
  text-align: center;
}

.item-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.item-name {
  font-size: 16px;
  font-weight: 500;
  margin-bottom: 8px;
  color: #333;
  word-break: break-word;
}

.item-meta {
  display: flex;
  gap: 12px;
  font-size: 12px;
  color: #666;
  margin-bottom: 8px;
}

.file-count,
.created-time {
  white-space: nowrap;
}

.item-description {
  font-size: 13px;
  color: #666;
  margin-bottom: 8px;
  word-break: break-word;
}

.public-badge {
  display: inline-block;
  padding: 2px 6px;
  background-color: #e7f3ff;
  color: #1a73e8;
  font-size: 12px;
  border-radius: 4px;
  margin-top: 4px;
  align-self: flex-start;
}

.item-actions {
  display: flex;
  gap: 8px;
  margin-top: 12px;
  flex-wrap: wrap;
}

.action-button {
  padding: 6px 12px;
  font-size: 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  background-color: white;
  transition: background-color 0.2s;
}

.action-button:hover {
  background-color: #f5f5f5;
}

.create-sub-button {
  background-color: #17a2b8;
  color: white;
  border-color: #17a2b8;
}

.create-sub-button:hover {
  background-color: #138496;
  border-color: #117a8b;
}

.edit-button {
  background-color: #ffc107;
  color: #212529;
  border-color: #ffc107;
}

.edit-button:hover {
  background-color: #e0a800;
  border-color: #d39e00;
}



.share-button {
  background-color: #28a745;
  color: white;
  border-color: #28a745;
}

.share-button:hover {
  background-color: #218838;
  border-color: #1e7e34;
}

.delete-button {
  background-color: #dc3545;
  color: white;
  border-color: #dc3545;
}

.delete-button:hover {
  background-color: #c82333;
  border-color: #bd2130;
}

.empty-state {
  grid-column: 1 / -1;
  text-align: center;
  padding: 40px 20px;
  color: #666;
  font-style: italic;
}

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
  border-radius: 8px;
  padding: 24px;
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-content h2 {
  margin-top: 0;
  margin-bottom: 20px;
  color: #333;
}

.form-group {
  margin-bottom: 16px;
}

.form-group label {
  display: block;
  margin-bottom: 6px;
  font-weight: 500;
  color: #333;
}

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  box-sizing: border-box;
}

.form-group textarea {
  resize: vertical;
  min-height: 80px;
}

.checkbox-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: normal;
  cursor: pointer;
  user-select: none;
}

.checkbox-label input[type="checkbox"] {
 width: auto;
}

.checkbox-description {
  font-size: 12px;
  color: #666;
  margin-left: 24px;
}

.user-select {
  height: 120px;
}

.form-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 24px;
}

.save-button {
  padding: 10px 20px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background-color 0.2s;
}

.save-button:hover {
  background-color: #0056b3;
}

.cancel-button {
  padding: 10px 20px;
  background-color: #6c757d;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background-color 0.2s;
}

.cancel-button:hover {
  background-color: #545b62;
}

.grant-button {
  padding: 8px 16px;
  background-color: #28a745;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background-color 0.2s;
  margin-top: 12px;
}

.grant-button:hover {
  background-color: #218838;
}

.permission-content {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.user-selection,
.granted-permissions {
  border: 1px solid #e0e0e0;
  border-radius: 6px;
  padding: 16px;
}

.user-selection h3,
.granted-permissions h3 {
  margin-top: 0;
  margin-bottom: 12px;
  color: #333;
}

.permission-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  margin-bottom: 8px;
}

.permission-user {
  font-weight: 500;
}

.permission-actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 24px;
}

.delete-modal .modal-actions {
  justify-content: center;
}

.datetime-input {
  padding: 8px;
}

.share-text-section {
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #e0e0e0;
}

.share-text-container {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.share-text-area {
  width: 100%;
  min-height: 80px;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-family: monospace;
  font-size: 13px;
  resize: vertical;
}

.copy-button {
  align-self: flex-start;
  padding: 8px 16px;
  background-color: #6f42c1;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.2s;
}

.copy-button:hover {
  background-color: #5a32a3;
}

.share-text-description {
  font-size: 12px;
  color: #666;
  margin: 0;
}

.preview-modal {
  z-index: 1100;
}

.preview-content {
  max-width: 90%;
  width: auto;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
}

.preview-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.preview-header h2 {
  margin: 0;
  font-size: 18px;
  color: #333;
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
  border-radius: 50%;
  transition: background-color 0.2s;
}

.close-button:hover {
  background-color: #f0f0f0;
}

.preview-frame,
.preview-text-frame {
  flex: 1;
  width: 100%;
  min-height: 400px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.preview-image {
  max-width: 100%;
  max-height: 70vh;
  object-fit: contain;
}

.preview-video,
.preview-audio {
  width: 100%;
  max-width: 800px;
}

.preview-loading,
.preview-error,
.preview-not-supported {
  text-align: center;
  padding: 40px 20px;
  color: #666;
}

.preview-error {
  color: #dc3545;
}

.toolbar-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  gap: 20px;  
  background-color: #f8f9fa;
  border-radius: 6px;
  border: 1px solid #e9ecef;
  padding: 12px 16px;
}

.batch-actions {
  border-radius: 6px;
}

.batch-buttons {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.batch-actions-placeholder {
  flex: 1;
}

.search-sort-container {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 20px;
}

.search-box {
  display: flex;
  gap: 10px;
}

.search-input {
  padding: 10px 15px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 14px;
  outline: none;
  transition: border-color 0.2s;
  width: 200px;
}

.search-input:focus {
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
}

.search-button {
  padding: 10px 20px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background-color 0.2s;
}

.search-button:hover {
  background-color: #0056b3;
}

.list-header {
  display: grid;
  grid-template-columns: 40px 2fr 1fr 1fr 1fr 1fr 150px;
  gap: 10px;
  padding: 12px 15px;
  background-color: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 4px;
  font-weight: 600;
  color: #495057;
}

.list-header-item:not(.list-header-checkbox):not(.list-header-type):not(.list-header-user):not(.list-header-actions) {
  cursor: pointer;
  user-select: none;
  transition: background-color 0.2s;
  padding: 4px 8px;
  border-radius: 4px;
}

.list-header-item:not(.list-header-checkbox):not(.list-header-type):not(.list-header-user):not(.list-header-actions):hover {
  background-color: #e9ecef;
}

.sort-indicator {
  margin-left: 5px;
  font-size: 12px;
}

.list-header {
  display: grid;
  grid-template-columns: 40px 2fr 1fr 1fr 1fr 1fr 150px;
  gap: 10px;
}

.list-item {
  display: grid;
  grid-template-columns: 40px 2fr 1fr 1fr 1fr 1fr 150px;
  gap: 10px;
  padding: 15px;
  border: 1px solid #e9ecef;
  border-radius: 4px;
  margin-bottom: 8px;
  background-color: white;
  transition: box-shadow 0.2s;
  align-items: center;
}

/* 确保所有列表项内容垂直居中对齐 */
.list-item > div {
  display: flex;
  align-items: center;
  min-height: 24px; /* 确保最小高度一致 */
}

/* 特别处理文件名容器 */
.list-item-name {
  display: flex;
  align-items: center;
  min-width: 0; /* 允许容器收缩 */
}

/* 确保文件大小等信息在容器中正确对齐 */
.list-item-size,
.list-item-type,
.list-item-time,
.list-item-user {
  justify-content: flex-start; /* 左对齐内容 */
  min-width: 60px; /* 确保最小宽度 */
}

.list-item:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.list-item.selected {
  background-color: #e3f2fd;
  border-color: #2196f3;
}

.list-item-checkbox {
  display: flex;
  align-items: center;
  justify-content: center;
}

.list-item-name {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  font-weight: 500;
  color: #212529;
}

.list-item-name:hover {
  color: #007bff;
}

.file-name,
.folder-name {
  flex: 1;
  min-width: 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.list-item-name .file-icon,
.list-item-name .folder-icon {
  flex-shrink: 0;
}

.list-item-checkbox {
  display: flex;
  align-items: center;
  justify-content: center;
}

.file-icon {
  width: 24px;
  height: 24px;
}

.folder-icon {
  font-size: 24px;
}

.list-item-size,
.list-item-type,
.list-item-time,
.list-item-user {
  display: flex;
  align-items: center;
  color: #495057;
  font-size: 14px;
  white-space: nowrap;
  min-width: 0; /* 允许容器收缩 */
  overflow: hidden; /* 防止内容溢出 */
  text-overflow: ellipsis; /* 文本过长时显示省略号 */
}

.list-item-actions {
  display: flex;
  gap: 5px;
  flex-wrap: wrap;
  min-width: 120px; /* 确保操作列有最小宽度 */
}

.preview-button {
  background-color: #17a2b8;
  color: white;
  border-color: #17a2b8;
}

.preview-button:hover {
  background-color: #138496;
  border-color: #117a8b;
}

.download-button {
  background-color: #28a745;
  color: white;
  border-color: #28a745;
}

.download-button:hover {
  background-color: #218838;
  border-color: #1e7e34;
}

/* 调整响应式断点，避免按钮堆叠 */
@media (max-width: 1300px) and (min-width: 1101px) {
  .list-header,
  .list-item {
    grid-template-columns: 40px 2fr 1fr 1fr 1fr 1fr 130px;
  }
  
  .list-item-actions {
    flex-direction: row; /* 保持横向排列 */
    flex-wrap: nowrap; /* 防止换行 */
    gap: 3px; /* 减小按钮间距 */
  }
  
  .action-button {
    width: auto;
    font-size: 12px;
    padding: 4px 6px;
    min-width: 0; /* 允许按钮收缩 */
    flex: 1; /* 允许按钮均匀分布 */
  }
}

@media (max-width: 1100px) and (min-width: 901px) {
  .list-header,
  .list-item {
    grid-template-columns: 40px 2fr 1fr 1fr 1fr 1fr 120px;
  }
  
  .list-item-actions {
    flex-direction: row;
    flex-wrap: wrap; /* 允许换行 */
    gap: 3px;
  }
  
  .action-button {
    width: auto;
    font-size: 11px;
    padding: 3px 5px;
    flex: 1;
  }
}

@media (max-width: 900px) {
  .list-item-actions {
    flex-direction: column; /* 在小屏幕上垂直排列 */
    flex-wrap: nowrap;
    gap: 2px;
  }
  
  .action-button {
    width: 100%;
    font-size: 11px;
    padding: 4px 8px;
  }
}

@media (max-width: 992px) {
  .search-sort-container {
    flex-direction: column;
    align-items: stretch;
  }
  
  .search-box {
    max-width: none;
  }
  
  .list-header,
  .list-item {
    grid-template-columns: 40px 2fr 1fr 1fr 1fr 1fr 100px;
  }
  
  .list-item-type,
  .list-item-user {
    font-size: 12px;
  }
  
  /* 确保小屏幕上内容正确显示 */
  .list-item-size,
  .list-item-type,
  .list-item-time,
  .list-item-user {
    min-width: 60px;
  }
}

@media (max-width: 768px) {
  .list-header {
    display: none;
  }
  
  .list-item {
    grid-template-columns: 40px 1fr;
    grid-template-rows: auto auto;
    gap: 10px;
    padding: 15px;
  }
  
  .list-item-name {
    grid-column: 1 / -1;
    grid-row: 1;
  }
  
  .list-item-size,
  .list-item-type,
  .list-item-time,
  .list-item-user {
    grid-column: 1 / -1;
    font-size: 13px;
    padding: 2px 0;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  
  .list-item-actions {
    grid-column: 1 / -1;
    grid-row: 2;
  }
  
  /* 移动端文件名优化 */
  .file-name,
  .folder-name {
    font-size: 14px;
  }
  
  .list-item-checkbox input {
    width: 16px;
    height: 16px;
  }
  
  .search-sort-container {
    flex-direction: column;
    gap: 10px;
  }
  
  /* 添加移动端优化样式 */
  .toolbar-container {
    flex-direction: column;
    align-items: stretch;
    gap: 12px;
    padding: 12px;
  }
  
  .batch-buttons {
    gap: 8px;
    flex-wrap: wrap;
  }
  
  .batch-button {
    padding: 6px 12px;
    font-size: 13px;
  }
  
  .search-box {
    flex-direction: column;
    gap: 8px;
  }
  
  .search-input {
    width: 100%;
  }
  
  .search-button {
    padding: 8px 16px;
  }
  
  .create-button {
    padding: 8px 16px;
    font-size: 14px;
    width: 100%;
  }
  
  .section-title {
    font-size: 18px;
    margin-bottom: 12px;
  }
  
  .modal-content {
    padding: 16px;
    margin: 10px;
  }
  
  .form-group input,
  .form-group textarea,
  .form-group select {
    padding: 8px;
    font-size: 14px;
  }
  
  .form-actions {
    flex-direction: column;
    gap: 10px;
  }
  
  .save-button,
  .cancel-button {
    width: 100%;
    padding: 10px;
  }
  
  .header {
    flex-direction: column;
    gap: 10px;
  }
  
  .breadcrumb {
    flex-wrap: wrap;
  }
  
  .breadcrumb-item {
    font-size: 14px;
    padding: 4px 6px;
  }
  
  .back-button {
    padding: 6px 12px;
    font-size: 14px;
    align-self: flex-start;
  }
}

@media (max-width: 576px) {
  .batch-buttons {
    flex-direction: column;
    align-items: stretch;
  }
  
  .batch-button {
    width: 100%;
    text-align: center;
  }
  
  .select-all {
    justify-content: center;
    padding: 8px 0;
  }
  
  .action-button {
    padding: 4px 8px;
    font-size: 12px;
    width: 100%;
    margin-bottom: 4px;
  }
  
  .list-item {
    padding: 12px;
  }
  
  .list-item-name {
    font-size: 14px;
  }
  
  .folder-icon,
  .file-icon {
    font-size: 20px;
    width: 20px;
    height: 20px;
  }
  
  .list-item-size,
  .list-item-type,
  .list-item-time,
  .list-item-user {
    font-size: 12px;
    padding: 1px 0;
  }
  
  .preview-content {
    max-width: 95%;
    margin: 5px;
    padding: 12px;
  }
  
  .preview-header h2 {
    font-size: 16px;
  }
  
  .close-button {
    width: 24px;
    height: 24px;
    font-size: 20px;
  }
  
  .form-group label {
    font-size: 14px;
  }
  
  .share-text-area {
    font-size: 12px;
    min-height: 60px;
  }
  
  .copy-button {
    padding: 6px 12px;
    font-size: 13px;
  }
  
  /* 超小屏幕文件名优化 */
  .file-name,
  .folder-name {
    font-size: 13px;
  }
}

@media (max-width: 480px) {
  .list-item {
    padding: 10px;
    gap: 8px;
  }
  
  .list-item-name {
    font-size: 13px;
  }
  
  .folder-icon,
  .file-icon {
    font-size: 18px;
    width: 18px;
    height: 18px;
  }
  
  .list-item-size,
  .list-item-type,
  .list-item-time,
  .list-item-user {
    font-size: 11px;
    min-height: 20px; /* 确保有最小高度 */
  }
  
  .action-button {
    padding: 3px 6px;
    font-size: 11px;
    margin-bottom: 3px;
    min-height: 28px; /* 确保按钮有最小高度 */
  }
  
  .search-input {
    padding: 8px 12px;
    font-size: 13px;
  }
  
  .search-button,
  .create-button {
    padding: 6px 12px;
    font-size: 13px;
  }
  
  /* 超小屏幕操作按钮优化 */
  .list-item-actions {
    display: grid;
    grid-template-columns: 1fr 1fr; /* 两列布局 */
    gap: 4px;
  }
}
</style>
