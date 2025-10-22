<template>
  <div class="folder-tree-node" :style="{ marginLeft: level * 20 + 'px' }">
    <div class="folder-tree-item">
      <div class="folder-tree-header">
        <div class="folder-tree-checkbox">
          <input 
            type="checkbox" 
            :checked="folder.hasPermission || false" 
            @change="toggleFolderPermission"
          />
        </div>
        <div class="folder-tree-name" @click="toggleFolder">
          <span 
            v-if="folder.children && folder.children.length > 0" 
            class="folder-toggle"
          >
            {{ (folder.expanded !== undefined ? folder.expanded : false) ? '▼' : '▶' }}
          </span>
          <span 
            v-else 
            class="folder-toggle-placeholder"
          ></span>
          <span class="folder-name-text">{{ folder.name }}</span>
          <span 
            v-if="folder.isPublic" 
            class="public-badge"
          >
            {{ $t('admin.publicFolder') }}
          </span>
        </div>
      </div>
      <div 
        v-if="folder.hasPermission" 
        class="folder-permissions-detail"
      >
        <label class="permission-checkbox">
          <input 
            type="checkbox" 
            v-model="folder.canRead"
          />
          {{ $t('admin.canRead') }}
        </label>
        <label class="permission-checkbox">
          <input 
            type="checkbox" 
            v-model="folder.canUpload"
          />
          {{ $t('admin.canUpload') }}
        </label>
        <label class="permission-checkbox">
          <input 
            type="checkbox" 
            v-model="folder.canDelete"
          />
          {{ $t('admin.canDelete') }}
        </label>
        <label class="permission-checkbox">
          <input 
            type="checkbox" 
            v-model="folder.canCreateFolder"
          />
          {{ $t('admin.canCreateFolder') }}
        </label>
      </div>
    </div>
    <div 
      v-if="(folder.expanded !== undefined ? folder.expanded : false) && folder.children && folder.children.length > 0" 
      class="folder-tree-children"
    >
      <FolderTree
        v-for="child in folder.children"
        :key="child.id"
        :folder="child"
        :level="level + 1"
        @toggle-folder="$emit('toggle-folder', $event)"
        @toggle-permission="$emit('toggle-permission', $event)"
      />
    </div>
  </div>
</template>

<script setup>
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

const emit = defineEmits(['toggle-folder', 'toggle-permission'])

const toggleFolder = () => {
  // 发出事件，让父组件处理展开/折叠状态
  emit('toggle-folder', props.folder);
}

const toggleFolderPermission = () => {
  // 发出事件，让父组件处理权限状态
  emit('toggle-permission', props.folder);
}
</script>

<style scoped>
.folder-tree-node {
  margin-bottom: 5px;
}

.folder-tree-item {
  display: flex;
  flex-direction: column;
  padding: 8px;
  border: 1px solid #eee;
  border-radius: 4px;
  background-color: #f8f9fa;
}

.folder-tree-header {
  display: flex;
  align-items: center;
  gap: 10px;
}

.folder-tree-checkbox input {
  margin-right: 5px;
}

.folder-tree-name {
  display: flex;
  align-items: center;
  cursor: pointer;
  flex: 1;
  gap: 5px;
}

.folder-toggle {
  font-size: 12px;
  width: 15px;
  text-align: center;
}

.folder-toggle-placeholder {
  width: 15px;
}

.folder-name-text {
  font-weight: 500;
}

.public-badge {
  background-color: #28a745;
  color: white;
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 12px;
  margin-left: 5px;
}

.folder-permissions-detail {
  display: flex;
  gap: 15px;
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px solid #ddd;
}

.permission-checkbox {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 14px;
}

.folder-tree-children {
  margin-top: 5px;
}
</style>