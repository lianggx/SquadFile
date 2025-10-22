import request from './request'

/**
 * 获取系统配置
 * @returns {Promise} 系统配置响应
 */
export const getSystemConfigApi = () => {
  return request.get('/system/config')
}

/**
 * 更新系统配置
 * @param {Object} config - 系统配置 { siteName, maxFileSize, storageLimit, defaultLanguage, allowRegistration }
 * @returns {Promise} 更新配置响应
 */
export const updateSystemConfigApi = (config) => {
  return request.put('/system/config', config)
}

/**
 * 上传Logo文件
 * @param {FormData} formData - 包含Logo文件的表单数据
 * @returns {Promise} 上传响应
 */
export const uploadLogoApi = (formData) => {
  return request({
    url: '/system/logo',
    method: 'post',
    data: formData,
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

/**
 * 获取管理统计数据
 * @returns {Promise} 管理统计数据响应
 */
export const getAdminStatsApi = () => {
  return request.get('/system/admin-stats')
}

/**
 * 获取验证码图片
 * @returns {Promise} 验证码图片响应
 */
export const getCaptchaImageApi = () => {
  return request.get('/captcha/image')
}

/**
 * 获取登录日志
 * @param {Object} params - 查询参数 { page, pageSize, searchQuery }
 * @returns {Promise} 登录日志响应
 */
export const getLoginLogsApi = (params) => {
  return request.get('/log/login', { params })
}

/**
 * 获取下载日志
 * @param {Object} params - 查询参数 { page, pageSize, searchQuery }
 * @returns {Promise} 下载日志响应
 */
export const getDownloadLogsApi = (params) => {
  return request.get('/log/download', { params })
}
