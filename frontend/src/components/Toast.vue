<template>
  <div v-if="isVisible" class="toast-container" :class="type">
    <div class="toast-message">
      {{ message }}
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  message: {
    type: String,
    default: ''
  },
  type: {
    type: String,
    default: 'info' // info, success, warning, error
  },
  duration: {
    type: Number,
    default: 3000 // 默认3秒后自动关闭
  }
})

const isVisible = ref(false)

// 监听消息变化，显示提示
watch(() => props.message, (newMessage) => {
  if (newMessage) {
    isVisible.value = true
    // 自动关闭
    setTimeout(() => {
      isVisible.value = false
    }, props.duration)
  }
})
</script>

<style scoped>
.toast-container {
  position: fixed;
  top: 80px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 9999;
  min-width: 300px;
  max-width: 600px;
  padding: 15px 20px;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  font-size: 16px;
  font-weight: 500;
  text-align: center;
  animation: slideDown 0.3s ease-out;
  backdrop-filter: blur(10px);
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translate(-50%, -20px);
  }
  to {
    opacity: 1;
    transform: translate(-50%, 0);
  }
}

.toast-message {
  margin: 0;
  padding: 0;
}

/* 不同类型的颜色 */
.info {
  background-color: rgba(255, 255, 255, 0.95);
  color: #333;
  border: 1px solid #ddd;
}

.success {
  background-color: rgba(40, 167, 69, 0.95);
  color: white;
  border: 1px solid rgba(40, 167, 69, 0.8);
}

.warning {
  background-color: rgba(255, 193, 7, 0.95);
  color: #212529;
  border: 1px solid rgba(255, 193, 7, 0.8);
}

.error {
  background-color: rgba(220, 53, 69, 0.95);
  color: white;
  border: 1px solid rgba(220, 53, 69, 0.8);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .toast-container {
    min-width: 250px;
    max-width: 90%;
    padding: 12px 16px;
    font-size: 14px;
  }
}

@media (max-width: 480px) {
  .toast-container {
    top: 70px;
    min-width: 200px;
    padding: 10px 14px;
    font-size: 13px;
  }
}
</style>