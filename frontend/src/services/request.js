import axios from 'axios'

// 创建axios实例
const request = axios.create({
  baseURL: '/squadfile-api', // 添加baseURL前缀，与后端控制器路由前缀匹配
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 请求拦截器
request.interceptors.request.use(
  (config) => {
    // 从localStorage获取token
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    
    // 从localStorage获取当前语言设置
    const currentLanguage = localStorage.getItem('preferredLanguage') || 'zh-Hans'
    config.headers['Accept-Language'] = currentLanguage
    
    return config
  },
  (error) => {
    console.error('请求错误:', error)
    return Promise.reject(error)
  }
)

// 响应拦截器
request.interceptors.response.use(
  (response) => {
    // 直接返回完整的响应对象，让调用方处理数据
    return response
  },
  (error) => {
    console.error('响应错误:', error)
    // 处理错误响应
    if (error.response) {
      console.error('错误响应状态:', error.response.status)
      console.error('错误响应数据:', error.response.data)
      switch (error.response.status) {
        case 401:
          // 未授权，清除token并跳转到登录页
          // 但需要检查请求URL，如果是分享密码验证则不跳转
          const requestUrl = error.config?.url || '';
          if (!requestUrl.includes('/fileshare/validate/')) {
            localStorage.removeItem('token')
            // 在实际应用中，你可能需要跳转到登录页面
            window.location.href = '/login' // 添加自动跳转到登录页
          }
          break
        case 403:
          // 禁止访问
          console.error('访问被禁止')
          break
        case 500:
          // 服务器错误
          console.error('服务器错误')
          break
        default:
          console.error('未知错误')
      }
    }
    return Promise.reject(error)
  }
)

export default request