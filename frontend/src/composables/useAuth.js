import { onMounted, onActivated } from 'vue'
import { useAuthStore } from '../stores/auth'
import { useRouter } from 'vue-router'

/**
 * 页面认证检查组合式函数
 * @param {boolean} redirectOnUnauth - 未认证时是否重定向到登录页
 * @returns {Object} 包含认证检查方法的对象
 */
export function usePageAuth(redirectOnUnauth = true) {
  const authStore = useAuthStore()
  const router = useRouter()

  /**
   * 检查用户认证状态
   * @param {boolean} useSwitchTab - 是否使用switchTab跳转（适用于tab页面）
   * @returns {boolean} 是否已认证
   */
  const checkAuth = (useSwitchTab = false) => {
    const isAuthenticated = authStore.checkAuth()
    
    if (!isAuthenticated && redirectOnUnauth) {
      // 未登录且需要重定向，跳转到登录页面
      if (useSwitchTab) {
        router.push('/login')
      } else {
        router.push('/login')
      }
      return false
    }
    
    return isAuthenticated
  }

  /**
   * 在页面加载时检查认证状态
   * @param {boolean} useSwitchTab - 是否使用switchTab跳转（适用于tab页面）
   */
  const checkAuthOnMounted = (useSwitchTab = false) => {
    onMounted(() => {
      // 添加延迟确保认证状态检查正确执行
      setTimeout(() => {
        checkAuth(useSwitchTab)
      }, 100)
    })
  }

  /**
   * 在页面激活时检查认证状态（适用于tab页面）
   * @param {boolean} useSwitchTab - 是否使用switchTab跳转（适用于tab页面）
   */
  const checkAuthOnShow = (useSwitchTab = false) => {
    if (typeof onActivated !== 'undefined') {
      onActivated(() => {
        // 添加延迟确保认证状态检查正确执行
        setTimeout(() => {
          checkAuth(useSwitchTab)
        }, 100)
      })
    } else {
      // 如果 onActivated 不可用，使用 onMounted 作为备选方案
      onMounted(() => {
        // 添加延迟确保认证状态检查正确执行
        setTimeout(() => {
          checkAuth(useSwitchTab)
        }, 100)
      })
    }
  }

  return {
    checkAuth,
    checkAuthOnMounted,
    checkAuthOnShow
  }
}