<template>
  <div class="shortlink-management-container">
    <!-- 添加 Toast 组件 -->
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <h1 class="title">{{ t('admin.shortLinkManagement') }}</h1>
    
    <!-- 批量操作工具栏 -->
    <div class="toolbar-container">
      <div v-if="filteredShareRecords && filteredShareRecords.length > 0" class="batch-actions">
        <div class="batch-buttons">
          <label class="select-all">
            <input 
              type="checkbox" 
              :checked="selectedShareRecords.length === filteredShareRecords.length && filteredShareRecords.length > 0" 
              @change="toggleSelectAll"
            />
            {{ t('admin.selectAll') }}
          </label>

          <button 
            @click="deleteSelected" 
            :disabled="selectedShareRecords.length === 0"
            class="batch-button delete-button"
          >
            {{ t('admin.delete') }} ({{ selectedShareRecords.length }})
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
    </div>
    
    <!-- 短链接列表 -->
    <div class="content-list">
      <h2 class="section-title">{{ t('admin.shortLinkList') }}</h2>
      
      <!-- 列表表头 -->
      <div class="list-header">
        <div class="list-header-item list-header-checkbox"></div>
        <div class="list-header-item list-header-shortcode" @click="handleHeaderClick('shortCode')">
          {{ t('admin.shortCode') }}
          <span v-if="sortField === 'shortCode'" class="sort-indicator">
            {{ sortOrder === 'asc' ? '↑' : '↓' }}
          </span>
        </div>
        <div class="list-header-item list-header-original" @click="handleHeaderClick('originalName')">
          {{ t('admin.originalName') }}
          <span v-if="sortField === 'originalName'" class="sort-indicator">
            {{ sortOrder === 'asc' ? '↑' : '↓' }}
          </span>
        </div>
        <div class="list-header-item list-header-type">{{ t('admin.type') }}</div>
        <div class="list-header-item list-header-createdby" @click="handleHeaderClick('createdBy')">
          {{ t('admin.createdBy') }}
          <span v-if="sortField === 'createdBy'" class="sort-indicator">
            {{ sortOrder === 'asc' ? '↑' : '↓' }}
          </span>
        </div>
        <div class="list-header-item list-header-createtime" @click="handleHeaderClick('createTime')">
          {{ t('admin.createTime') }}
          <span v-if="sortField === 'createTime'" class="sort-indicator">
            {{ sortOrder === 'asc' ? '↑' : '↓' }}
          </span>
        </div>
        <div class="list-header-item list-header-expiretime" @click="handleHeaderClick('expireTime')">
          {{ t('admin.expireTime') }}
          <span v-if="sortField === 'expireTime'" class="sort-indicator">
            {{ sortOrder === 'asc' ? '↑' : '↓' }}
          </span>
        </div>
        <div class="list-header-item list-header-actions">{{ t('admin.actions') }}</div>
      </div>
      
      <!-- 短链接列表项 -->
      <div 
        v-for="record in filteredShareRecords" 
        :key="record.id" 
        class="list-item"
        :class="{ selected: selectedShareRecords.includes(record.id) }"
      >
        <div class="list-item-checkbox">
          <input 
            type="checkbox" 
            :checked="selectedShareRecords.includes(record.id)" 
            @change="toggleSelection(record.id, $event.target.checked)"
          />
        </div>
        <div class="list-item-shortcode" :data-label="t('admin.shortCode') + ':'">
          <span class="shortcode-text">{{ record.shortCode }}</span>
        </div>
        <div class="list-item-original" :data-label="t('admin.originalName') + ':'">
          <span class="original-name">{{ record.originalName }}</span>
        </div>
        <div class="list-item-type" :data-label="t('admin.type') + ':'">
          <span :class="['type-badge', (record.itemType || '').toLowerCase()]">
            {{ record.itemType === 'File' ? t('admin.file') : t('admin.folder') }}
          </span>
        </div>
        <div class="list-item-createdby" :data-label="t('admin.createdBy') + ':'">
          <span class="createdby-name">{{ record.createdBy }}</span>
        </div>
        <div class="list-item-createtime" :data-label="t('admin.createTime') + ':'">
          <span class="createtime-text">{{ formatDate(record.createTime) }}</span>
        </div>
        <div class="list-item-expiretime" :data-label="t('admin.expireTime') + ':'">
          <span class="expiretime-text">{{ record.expireTime ? formatDate(record.expireTime) : t('admin.noExpire') }}</span>
        </div>
        <div class="list-item-actions" :data-label="t('admin.actions') + ':'">
          <button @click.stop="copyShortLink(record)" class="action-button copy-button">
            {{ t('admin.copy') }}
          </button>
          <button @click.stop="showDeleteModalForRecord(record)" class="action-button delete-button">
            {{ t('admin.delete') }}
          </button>
        </div>
      </div>
      
      <!-- 空状态提示 -->
      <div v-if="!filteredShareRecords || filteredShareRecords.length === 0" class="empty-state">
        <p>{{ t('admin.noShareRecords') }}</p>
      </div>
      
      <!-- 分页控件 -->
      <div v-if="pagination.totalPages > 1" class="pagination-container">
        <button 
          @click="goToPage(pagination.currentPage - 1)" 
          :disabled="pagination.currentPage === 1"
          class="pagination-button"
        >
          {{ t('admin.previous') }}
        </button>
        
        <span class="pagination-info">
          {{ t('admin.pageInfo', { current: pagination.currentPage, total: pagination.totalPages }) }}
        </span>
        
        <button 
          @click="goToPage(pagination.currentPage + 1)" 
          :disabled="pagination.currentPage === pagination.totalPages"
          class="pagination-button"
        >
          {{ t('admin.next') }}
        </button>
      </div>
    </div>
    
    <!-- 删除确认模态框 -->
    <div v-if="showDeleteModal" class="modal">
      <div class="modal-content delete-modal">
        <h2 v-if="recordToDelete?.type === 'single'">{{ t('admin.confirmDeleteShareRecord', { name: recordToDelete?.data?.originalName }) }}</h2>
        <h2 v-else-if="recordToDelete?.type === 'multiple'">{{ t('admin.confirmDeleteMultipleShareRecords', { count: recordToDelete?.data?.length }) }}</h2>
        <p>{{ t('admin.confirmDeleteShareRecordContent') }}</p>
        <div class="modal-actions">
          <button @click="confirmDelete" class="delete-button action-button">{{ t('admin.delete') }}</button>
          <button @click="closeDeleteModal" class="cancel-button action-button">{{ t('admin.cancel') }}</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { usePageAuth } from '../../composables/useAuth'
import { 
  getShareRecordsApi,
  deleteShareRecordsApi
} from '../../services/file'
import { formatDate } from '../../utils/fileUtils'

// 导入 Toast 组件
import Toast from '../../components/Toast.vue'

const { t } = useI18n()
const { checkAuthOnMounted } = usePageAuth()

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 搜索和排序相关数据
const searchQuery = ref('')
const sortField = ref('createTime')
const sortOrder = ref('desc') // 'asc' 或 'desc'

// 分页相关数据
const pagination = ref({
  currentPage: 1,
  pageSize: 20,
  totalPages: 1,
  totalCount: 0
})

// 短链接数据
const shareRecords = ref([])

// 模态框显示状态
const showDeleteModal = ref(false)

// 要删除的记录信息
const recordToDelete = ref(null)

// 选中的短链接
const selectedShareRecords = ref([])

// 页面加载时检查认证状态
checkAuthOnMounted(false)

// 页面加载时获取数据
onMounted(() => {
  fetchShareRecords()
})

// 获取短链接列表
const fetchShareRecords = async () => {
  try {
    const response = await getShareRecordsApi({
      searchQuery: searchQuery.value,
      page: pagination.value.currentPage,
      pageSize: pagination.value.pageSize
    })
    
    if (response.data && response.data.data) {
      shareRecords.value = response.data.data.items || []
      pagination.value.totalCount = response.data.data.totalCount || 0
      pagination.value.currentPage = response.data.data.page || 1
      pagination.value.pageSize = response.data.data.pageSize || 20
      pagination.value.totalPages = response.data.data.totalPages || 1
    } else {
      shareRecords.value = []
      pagination.value.totalCount = 0
      pagination.value.currentPage = 1
      pagination.value.pageSize = 20
      pagination.value.totalPages = 1
    }
  } catch (error) {
    console.error('获取短链接列表失败:', error)
    showToast(t('admin.fetchShareRecordsFailed'), 'error')
  }
}

// 切换选择
const toggleSelection = (id, selected) => {
  const index = selectedShareRecords.value.indexOf(id)
  if (selected && index === -1) {
    // 如果要选中且未选中，则添加
    selectedShareRecords.value.push(id)
  } else if (!selected && index > -1) {
    // 如果要取消选中且已选中，则移除
    selectedShareRecords.value.splice(index, 1)
  }
}

// 全选/取消全选
const toggleSelectAll = () => {
  // 如果当前已全选，则清空选择
  if (selectedShareRecords.value.length === filteredShareRecords.value.length && filteredShareRecords.value.length > 0) {
    selectedShareRecords.value = []
  } else {
    // 否则选择所有记录
    selectedShareRecords.value = [
      ...filteredShareRecords.value.map(record => record.id)
    ]
  }
}

// 计算属性 - 过滤和排序后的短链接
const filteredShareRecords = computed(() => {
  if (!shareRecords.value) return [];
  
  // 先过滤
  let result = shareRecords.value;
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter(record => 
      record.shortCode.toLowerCase().includes(query) ||
      (record.originalName && record.originalName.toLowerCase().includes(query)) ||
      (record.createdBy && record.createdBy.toLowerCase().includes(query))
    );
  }
  
  // 再排序
  return result.sort((a, b) => {
    let aValue, bValue;
    
    switch (sortField.value) {
      case 'shortCode':
        aValue = a.shortCode.toLowerCase();
        bValue = b.shortCode.toLowerCase();
        break;
      case 'originalName':
        aValue = (a.originalName || '').toLowerCase();
        bValue = (b.originalName || '').toLowerCase();
        break;
      case 'createdBy':
        aValue = (a.createdBy || '').toLowerCase();
        bValue = (b.createdBy || '').toLowerCase();
        break;
      case 'createTime':
        aValue = new Date(a.createTime);
        bValue = new Date(b.createTime);
        break;
      case 'expireTime':
        aValue = a.expireTime ? new Date(a.expireTime) : new Date(0);
        bValue = b.expireTime ? new Date(b.expireTime) : new Date(0);
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

// 处理搜索
const handleSearch = async () => {
  // 重置到第一页
  pagination.value.currentPage = 1;
  await fetchShareRecords();
};

// 跳转到指定页
const goToPage = async (page) => {
  if (page >= 1 && page <= pagination.value.totalPages) {
    pagination.value.currentPage = page;
    await fetchShareRecords();
  }
};

// 显示删除模态框
const showDeleteModalForRecord = (record) => {
  recordToDelete.value = {
    type: 'single',
    data: record
  };
  showDeleteModal.value = true;
};

// 批量删除
const deleteSelected = () => {
  if (selectedShareRecords.value.length > 0) {
    recordToDelete.value = {
      type: 'multiple',
      data: selectedShareRecords.value.map(id => 
        shareRecords.value.find(record => record.id === id)
      ).filter(record => record)
    };
    showDeleteModal.value = true;
  }
};

// 确认删除
const confirmDelete = async () => {
  try {
    if (recordToDelete.value && recordToDelete.value.type === 'single') {
      // 删除单个记录
      const record = recordToDelete.value.data;
      await deleteShareRecordsApi({ ids: [record.id] });
      await fetchShareRecords();
      showToast(t('admin.shareRecordDeleted'), 'success');
    } else if (recordToDelete.value && recordToDelete.value.type === 'multiple') {
      // 批量删除
      const ids = recordToDelete.value.data.map(record => record.id);
      await deleteShareRecordsApi({ ids });
      await fetchShareRecords();
      selectedShareRecords.value = [];
      showToast(t('admin.shareRecordsDeleted', { count: ids.length }), 'success');
    }
  } catch (error) {
    console.error('删除短链接失败:', error);
    showToast(t('admin.deleteShareRecordFailed'), 'error');
  }
  closeDeleteModal();
};

// 关闭删除模态框
const closeDeleteModal = () => {
  showDeleteModal.value = false;
  recordToDelete.value = null;
};

// 复制短链接
const copyShortLink = (record) => {
  const shareLink = `${window.location.origin}/share/${record.shortCode}`;
  navigator.clipboard.writeText(shareLink)
    .then(() => {
      showToast(t('admin.linkCopied'), 'success');
    })
    .catch(() => {
      // 降级方案
      const textArea = document.createElement('textarea');
      textArea.value = shareLink;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand('copy');
      document.body.removeChild(textArea);
      showToast(t('admin.linkCopied'), 'success');
    });
};

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message;
  toastType.value = type;
};
</script>

<style scoped>
.shortlink-management-container {
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

.toolbar-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  flex-wrap: wrap;
  gap: 15px;
}

.batch-actions {
  display: flex;
  align-items: center;
  gap: 15px;
}

.batch-buttons {
  display: flex;
  align-items: center;
  gap: 10px;
}

.select-all {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 14px;
  color: #555;
  cursor: pointer;
}

.batch-button {
  padding: 8px 15px;
  border: none;
  border-radius: 5px;
  font-size: 14px;
  cursor: pointer;
  background-color: #17a2b8;
  color: white;
}

.batch-button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.delete-button {
  background-color: #dc3545 !important;
}

.search-sort-container {
  display: flex;
  gap: 10px;
}

.search-box {
  display: flex;
  gap: 5px;
}

.search-input {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 14px;
}

.search-button {
  padding: 8px 15px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.content-list {
  background-color: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
}

.section-title {
  font-size: 20px;
  font-weight: bold;
  color: #333;
  margin-bottom: 20px;
}

.list-header {
  display: grid;
  grid-template-columns: 40px 1fr 1.5fr 100px 1fr 1fr 1fr 150px;
  gap: 10px;
  padding: 12px 15px;
  background-color: #f8f9fa;
  font-weight: bold;
  color: #555;
  border-bottom: 1px solid #eee;
}

.list-header-item {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.list-header-item:hover {
  color: #007bff;
}

.sort-indicator {
  margin-left: 5px;
  font-size: 12px;
}

.list-item {
  display: grid;
  grid-template-columns: 40px 1fr 1.5fr 100px 1fr 1fr 1fr 150px;
  gap: 10px;
  padding: 15px;
  border-bottom: 1px solid #eee;
  transition: background-color 0.2s;
}

.list-item:hover {
  background-color: #f8f9fa;
}

.list-item.selected {
  background-color: #e3f2fd;
}

.list-item-checkbox {
  display: flex;
  align-items: center;
  justify-content: center;
}

.list-item-shortcode .shortcode-text {
  font-family: monospace;
  font-weight: bold;
  color: #007bff;
}

.list-item-original .original-name {
  font-weight: 500;
  color: #333;
  word-break: break-word;
}

.type-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: bold;
  color: white;
  text-align: center;
}

.type-badge.file {
  background-color: #17a2b8;
}

.type-badge.folder {
  background-color: #28a745;
}

.list-item-createdby .createdby-name {
  color: #555;
}

.list-item-createtime .createtime-text,
.list-item-expiretime .expiretime-text {
  color: #666;
  font-size: 14px;
}

.list-item-actions {
  display: flex;
  gap: 5px;
  justify-content: flex-end;
}

.action-button {
  padding: 6px 10px;
  border: none;
  border-radius: 4px;
  font-size: 13px;
  cursor: pointer;
  white-space: nowrap;
}

.copy-button {
  background-color: #28a745;
  color: white;
}

.delete-button {
  background-color: #dc3545;
  color: white;
}

.action-button:hover {
  opacity: 0.8;
}

.empty-state {
  text-align: center;
  padding: 40px 20px;
  color: #666;
}

/* 分页控件样式 */
.pagination-container {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 15px;
  margin-top: 20px;
  padding: 20px 0;
}

.pagination-button {
  padding: 8px 16px;
  border: 1px solid #ddd;
  background-color: #fff;
  color: #333;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.pagination-button:hover:not(:disabled) {
  background-color: #007bff;
  color: white;
  border-color: #007bff;
}

.pagination-button:disabled {
  background-color: #f5f5f5;
  color: #999;
  cursor: not-allowed;
}

.pagination-info {
  font-size: 14px;
  color: #666;
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
  padding: 30px;
  border-radius: 10px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  max-width: 500px;
  width: 90%;
}

.delete-modal h2 {
  margin-top: 0;
  margin-bottom: 20px;
  color: #333;
  text-align: center;
}

.delete-modal p {
  margin-bottom: 30px;
  text-align: center;
  color: #666;
}

.modal-actions {
  display: flex;
  justify-content: center;
  gap: 15px;
}

.modal-actions .action-button {
  padding: 10px 20px;
  font-size: 16px;
  min-width: 100px;
}

.delete-button {
  background-color: #dc3545;
  color: white;
}

.cancel-button {
  background-color: #6c757d;
  color: white;
}

/* 响应式设计 */
@media (max-width: 1200px) {
  .list-header {
    grid-template-columns: 40px 1fr 1fr 100px 1fr 1fr 1fr 120px;
  }
  
  .list-item {
    grid-template-columns: 40px 1fr 1fr 100px 1fr 1fr 1fr 120px;
  }
}

@media (max-width: 992px) {
  .toolbar-container {
    flex-direction: column;
    align-items: stretch;
  }
  
  .batch-actions {
    justify-content: space-between;
  }
  
  .search-sort-container {
    justify-content: flex-end;
  }
  
  .list-header {
    grid-template-columns: 40px 1fr 1fr 100px 1fr 1fr 1fr 100px;
    font-size: 14px;
  }
  
  .list-item {
    grid-template-columns: 40px 1fr 1fr 100px 1fr 1fr 1fr 100px;
    font-size: 14px;
  }
  
  .action-button {
    padding: 5px 8px;
    font-size: 12px;
  }
}

@media (max-width: 768px) {
  .shortlink-management-container {
    padding: 15px;
  }
  
  .title {
    font-size: 22px;
  }
  
  .toolbar-container {
    gap: 10px;
  }
  
  .search-input {
    padding: 6px 10px;
    font-size: 13px;
  }
  
  .search-button {
    padding: 6px 12px;
    font-size: 13px;
  }
  
  .content-list {
    padding: 15px;
  }
  
  .section-title {
    font-size: 18px;
  }
  
  .list-header {
    grid-template-columns: 30px 1fr 1fr 80px 1fr 1fr 1fr 80px;
    font-size: 13px;
    padding: 10px 12px;
  }
  
  .list-item {
    grid-template-columns: 30px 1fr 1fr 80px 1fr 1fr 1fr 80px;
    font-size: 13px;
    padding: 12px;
  }
  
  .list-item-original .original-name,
  .list-item-createdby .createdby-name {
    font-size: 12px;
  }
  
  .list-item-createtime .createtime-text,
  .list-item-expiretime .expiretime-text {
    font-size: 12px;
  }
  
  .type-badge {
    padding: 3px 6px;
    font-size: 11px;
  }
  
  .pagination-container {
    flex-direction: column;
    gap: 10px;
  }
  
  .pagination-info {
    order: -1;
  }
}

@media (max-width: 576px) {
  .shortlink-management-container {
    padding: 10px;
  }
  
  .title {
    font-size: 20px;
  }
  
  .toolbar-container {
    flex-direction: column;
    align-items: stretch;
    gap: 10px;
  }
  
  .batch-actions {
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }
  
  .batch-buttons {
    width: 100%;
    justify-content: space-between;
  }
  
  .batch-button {
    padding: 6px 10px;
    font-size: 12px;
  }
  
  .search-sort-container {
    width: 100%;
  }
  
  .search-box {
    width: 100%;
  }
  
  .search-input {
    flex: 1;
    padding: 8px;
    font-size: 14px;
  }
  
  .search-button {
    padding: 8px 12px;
    font-size: 14px;
  }
  
  .content-list {
    padding: 10px;
  }
  
  .section-title {
    font-size: 16px;
  }
  
  .list-header {
    display: none;
  }
  
  .list-item {
    display: flex;
    flex-direction: column;
    gap: 8px;
    padding: 12px;
    border: 1px solid #eee;
    border-radius: 8px;
    margin-bottom: 10px;
  }
  
  .list-item.selected {
    border-color: #007bff;
  }
  
  .list-item > div {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  
  .list-item > div::before {
    content: attr(data-label);
    font-weight: bold;
    color: #555;
    margin-right: 10px;
    white-space: nowrap;
  }
  
  .list-item-checkbox {
    position: absolute;
    top: 10px;
    right: 10px;
  }
  
  .list-item-actions {
    justify-content: flex-start;
    margin-top: 5px;
  }
  
  .modal-content {
    padding: 20px;
    margin: 20px;
  }
  
  .delete-modal h2 {
    font-size: 18px;
  }
  
  .delete-modal p {
    font-size: 14px;
  }
  
  .modal-actions {
    flex-direction: column;
    gap: 10px;
  }
  
  .modal-actions .action-button {
    width: 100%;
    padding: 12px;
  }
}
</style>