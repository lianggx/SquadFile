<script setup>
// Vue3应用入口文件
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import Header from './src/components/Header.vue'

const route = useRoute()

// 计算属性：是否显示顶部导航栏（除了登录页外都显示）
const showHeader = computed(() => {
  // 除了登录页外的所有页面都显示顶部导航栏
  return route.name !== 'Login'
})
</script>

<template>
  <div id="app">
    <!-- 除了登录页外的所有页面都显示顶部导航栏 -->
    <Header v-if="showHeader" />
    
    <!-- 使用router-view渲染页面 -->
    <router-view />
  </div>
</template>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
  min-height: 100vh;
}

/* 添加流式布局样式 */
* {
  box-sizing: border-box;
}

body {
  margin: 0;
  padding: 0;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .container {
    padding: 0 15px;
  }
}

@media (max-width: 480px) {
  .container {
    padding: 0 10px;
  }
}
</style>