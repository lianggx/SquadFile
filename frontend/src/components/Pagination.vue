<template>
  <div class="pagination">
    <button 
      @click="prevPage" 
      :disabled="currentPage === 1"
      class="pagination-button"
    >
      {{ $t('admin.previous') }}
    </button>
    
    <span class="pagination-info">
      {{ $t('admin.pageInfo', { current: currentPage, total: totalPages }) }}
    </span>
    
    <button 
      @click="nextPage" 
      :disabled="currentPage === totalPages"
      class="pagination-button"
    >
      {{ $t('admin.next') }}
    </button>
  </div>
</template>

<script setup>
defineProps({
  currentPage: {
    type: Number,
    required: true
  },
  totalPages: {
    type: Number,
    required: true
  }
})

const emit = defineEmits(['pageChanged'])

const prevPage = () => {
  if (currentPage > 1) {
    emit('pageChanged', currentPage - 1)
  }
}

const nextPage = () => {
  if (currentPage < totalPages) {
    emit('pageChanged', currentPage + 1)
  }
}
</script>

<style scoped>
.pagination {
  display: flex;
  align-items: center;
  gap: 15px;
}

.pagination-button {
  padding: 8px 12px;
  border: 1px solid #ddd;
  background-color: #fff;
  color: #333;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.pagination-button:hover:not(:disabled) {
  background-color: #f5f7fa;
  border-color: #667eea;
  color: #667eea;
}

.pagination-button:disabled {
  background-color: #f5f7fa;
  color: #999;
  cursor: not-allowed;
}

.pagination-info {
  font-size: 14px;
  color: #666;
  white-space: nowrap;
}
</style>