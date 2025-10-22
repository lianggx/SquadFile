import { defineStore } from 'pinia'
import { loginApi, changeCurrentUserPasswordApi, updateProfileApi, getProfileApi } from '../services/auth'
import { useI18n } from 'vue-i18n'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: localStorage.getItem('token') || null,
    isAuthenticated: false
  }),

  actions: {
    async login(credentials) {
      try {
        const response = await loginApi(credentials)
        
        // 检查响应结构 - 注意响应数据在response.data.data中
        if (response && response.data && response.data.data) {
          const { data } = response.data // 注意这里的变化
          
          this.token = data.accessToken
          this.user = data.user
          this.isAuthenticated = true
          
          // 保存token和用户信息到localStorage
          localStorage.setItem('token', data.accessToken)
          localStorage.setItem('user', JSON.stringify(data.user))
          
          return response
        } else {
          throw new Error('login.loginFailed')
        }
      } catch (error) {
        console.error('登录失败:', error)
        // 检查错误类型并抛出相应的错误消息
        if (error.response) {
          const status = error.response.status
          const responseData = error.response.data
          
          if (status === 400) {
            // 客户端错误
            if (responseData && responseData.message) {
              throw new Error(responseData.message)
            } else {
              throw new Error('login.loginFailed')
            }
          } else if (status === 401) {
            // 认证错误
            throw new Error('login.loginFailed')
          } else {
            // 其他错误
            throw new Error('login.loginFailed')
          }
        } else {
          // 网络错误或其他未知错误
          throw new Error('login.loginFailed')
        }
      }
    },

    logout() {
      this.user = null
      this.token = null
      this.isAuthenticated = false
      
      // 清除localStorage中的token和用户信息
      localStorage.removeItem('token')
      localStorage.removeItem('user')
    },

    async refreshToken() {
      // 在实际应用中，这里会调用刷新token的API
      // 暂时留空
    },

    /**
     * 修改当前用户密码
     * @param {Object} passwords - 密码信息 { oldPassword, newPassword, confirmPassword }
     * @returns {Promise} 修改结果
     */
    async changePassword(passwords) {
      try {
        const response = await changeCurrentUserPasswordApi(passwords)
        return response
      } catch (error) {
        console.error('修改密码失败:', error)
        throw error
      }
    },

    /**
     * 更新当前用户资料
     * @param {Object} profile - 用户资料 { email, displayName, department }
     * @returns {Promise} 更新结果
     */
    async updateProfile(profile) {
      try {
        const response = await updateProfileApi(profile)
        // 注意：响应数据在response.data.data中
        const { data } = response.data
        
        // 更新store中的用户信息
        this.user = data
        // 更新localStorage中的用户信息
        localStorage.setItem('user', JSON.stringify(data))
        
        return response
      } catch (error) {
        console.error('更新资料失败:', error)
        throw error
      }
    },

    /**
     * 获取当前用户资料
     * @returns {Promise} 用户资料
     */
    async fetchProfile() {
      try {
        const response = await getProfileApi()
        // 注意：响应数据在response.data.data中
        const { data } = response.data
        
        // 更新store中的用户信息
        this.user = data
        // 更新localStorage中的用户信息
        localStorage.setItem('user', JSON.stringify(data))
        
        return response
      } catch (error) {
        console.error('获取用户资料失败:', error)
        throw error
      }
    },

    /**
     * 检查用户是否已认证
     * @returns {boolean} 是否已认证
     */
    checkAuth() {      
      // 检查localStorage中是否存在token
      const token = localStorage.getItem('token')
      
      // 如果有token，更新store状态
      if (token) {
        this.token = token
        this.isAuthenticated = true
        
        // 从localStorage中恢复用户信息
        try {
          const userStr = localStorage.getItem('user')
          if (userStr) {
            this.user = JSON.parse(userStr)
          }
        } catch (e) {
          console.error('解析用户信息失败:', e)
          // 如果解析失败，清除localStorage中的用户信息
          localStorage.removeItem('user')
        }
        
        return true
      } else {
        // 如果没有token，确保store状态是未认证的
        this.token = null
        this.isAuthenticated = false
        this.user = null
        return false
      }
    }
  }
})