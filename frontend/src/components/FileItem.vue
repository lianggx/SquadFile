<template>
  <div class="file-item" :class="{ selected: selected }" @click="handleClick">
    <div class="file-selection">
      <input 
        type="checkbox" 
        :checked="selected" 
        @click.stop="$emit('update:selected', !selected)"
      />
    <FileIcon 
      :extension="file.extension" 
      :size="iconSize"
      class="file-icon"
    />        
    </div>
    
    <div class="file-info">
      <div class="file-name" 
        @click.stop="handlePreview">{{ file.originalName }}</div>
      <div class="file-meta">
        <span class="file-size">{{ formatFileSize(file.size) }}</span>
        <!-- <span class="file-type">{{ getFileTypeDisplayName(file.extension) }}</span> -->
        <span class="file-user">{{ file.userName }}</span>
        <span class="created-time">{{ formatDate(file.uploadTime) }}</span>
      </div>
      <div v-if="file.description" class="file-description">
        {{ file.description }}
      </div>
    </div>
    
    <!-- <FileActions 
      :file="file" 
      @preview="handlePreview" 
      @download="handleDownload" 
    /> -->
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import FileIcon from './FileIcon.vue'
import FileActions from './FileActions.vue'
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
} from '../utils/fileUtils'

// 定义 props
const props = defineProps({
  file: {
    type: Object,
    required: true
  },
  showSelection: {
    type: Boolean,
    default: false
  },
  selected: {
    type: Boolean,
    default: false
  },
  iconSize: {
    type: String,
    default: 'sm'
  }
})

// 定义 emits
const emit = defineEmits(['click', 'preview', 'download', 'update:selected'])

const { t } = useI18n()

// 处理点击事件
const handleClick = () => {
  emit('click', props.file)
}

// 处理预览
const handlePreview = (file) => {
  emit('preview', file)
}

// 处理下载
const handleDownload = (fileId) => {
  emit('download', fileId)
}
</script>

<style scoped>
.file-icon{
  float: right;
}
.file-item {
  align-items: flex-start;
  background-color: white;
  border-radius: 8px;
  padding: 15px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  cursor: pointer;
  gap: 15px;
}

.file-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.15);
}

.file-item.selected {
  border: 2px solid #667eea;
  background-color: #f0f4ff;
}

.file-selection {
  flex-shrink: 0;
  margin-top: 3px;
}

.file-selection input[type="checkbox"] {
  width: 18px;
  height: 18px;
  /* transform: scale(1.3); */
}

.file-info {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
}

.file-name {
  font-weight: bold;
  color: #333;
  word-break: break-word;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  margin: 10px 0px 10px 0px;
}

.file-meta {
  gap: 15px;
  font-size: 14px;
  color: #666;
  margin-bottom: 10px;
  flex-wrap: nowrap;
  white-space: nowrap;
}

.file-user {
  font-size: 14px;
  color: #666;
  margin: 0 10px;
}

.file-type {
  background-color: #e9ecef;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  white-space: nowrap;
}

.file-description {
  font-size: 14px;
  color: #888;
  margin-top: 5px;
  word-break: break-word;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .file-item {
    align-items: flex-start;
    padding: 15px;
    gap: 10px;
  }
  
  .file-header {
    gap: 8px;
  }
  
  .file-selection {
    margin-top: 3px;
  }
  
  .file-info {
    flex: 1;
    min-width: 0;
  }
  
  .file-name {
    font-size: 16px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
  
  .file-meta {
    flex-direction: row;
    gap: 10px;
    font-size: 12px;
    flex-wrap: nowrap;
    white-space: nowrap;
  }
  
  .file-user {
    font-size: 12px;
    margin: 0 10px;
  }
  
  .file-actions {
    flex-shrink: 0;
    margin-top: 10px;
    width: 100%;
    justify-content: flex-end;
  }
}

@media (max-width: 480px) {
  .file-item {
    padding: 12px;
    gap: 8px;
  }
  
  .file-header {
    gap: 6px;
  }
  
  .file-name {
    font-size: 14px;
  }
  
  .file-meta {
    font-size: 11px;
    gap: 8px;
  }
  
  .file-user {
    font-size: 11px;
    margin: 0 10px;
  }
  
  .file-selection input[type="checkbox"] {
    transform: scale(1.2);
  }
  
  .file-actions {
    margin-top: 8px;
  }
}
</style>