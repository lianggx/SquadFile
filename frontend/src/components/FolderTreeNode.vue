<template>
  <div class="folder-tree-node" :style="{ marginLeft: level * 20 + 'px' }">
    <div 
      class="folder-card"
      @click="$emit('folder-click', folder.id)"
    >
      <div class="folder-icon">📁</div>
      <div class="folder-name">{{ folder.name }}</div>
      <div class="folder-info">
        <span class="file-count">{{ $t('home.fileCount', { count: folder.fileCount }) }}</span>
        <span class="updated-time">{{ formatTime(folder.updatedTime) }}</span>
      </div>
    </div>
    
    <!-- 递归渲染子文件夹 -->
    <div v-if="folder.children && folder.children.length > 0" class="folder-children">
      <FolderTreeNode
        v-for="child in folder.children"
        :key="child.id"
        :folder="child"
        :level="level + 1"
        @folder-click="$emit('folder-click', $event)"
      />
    </div>
  </div>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
  folder: {
    type: Object,
    required: true
  },
  level: {
    type: Number,
    default: 0
  }
})

defineEmits(['folder-click'])

// 格式化时间
const formatTime = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleDateString()
}
</script>

<style scoped>
.folder-tree-node {
  margin-bottom: 10px;
}

.folder-card {
  background: white;
  border-radius: 15px;
  padding: 20px;
  text-align: center;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.folder-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.folder-icon {
  font-size: 48px;
  margin-bottom: 15px;
}

.folder-name {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin-bottom: 10px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  width: 100%;
}

.folder-info {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
  color: #666;
  width: 100%;
}

.folder-children {
  margin-top: 10px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .folder-card {
    padding: 15px;
  }
  
  .folder-icon {
    font-size: 36px;
    margin-bottom: 10px;
  }
  
  .folder-name {
    font-size: 16px;
  }
  
  .folder-info {
    font-size: 12px;
  }
}

@media (max-width: 480px) {
  .folder-card {
    padding: 12px;
  }
  
  .folder-icon {
    font-size: 32px;
  }
  
  .folder-name {
    font-size: 14px;
  }
  
  .folder-info {
    flex-direction: column;
    gap: 3px;
  }
}
</style>