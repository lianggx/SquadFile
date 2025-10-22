import request from './request'

/**
 * 用户登录
 * @param {Object} credentials - 登录凭证 { username, password, captcha, captchaId }
 * @returns {Promise} 登录响应
 */
export const loginApi = (credentials) => {
  return request.post('/auth/login', credentials)
}

/**
 * 刷新token
 * @param {string} refreshToken - 刷新令牌
 * @returns {Promise} 刷新响应
 */
export const refreshTokenApi = (refreshToken) => {
  return request.post('/auth/refresh', { refreshToken })
}

/**
 * 用户登出
 * @returns {Promise} 登出响应
 */
export const logoutApi = () => {
  return request.post('/auth/logout')
}

/**
 * 修改密码
 * @param {Object} passwords - 密码信息 { oldPassword, newPassword, confirmPassword }
 * @returns {Promise} 修改密码响应
 */
export const changePasswordApi = (passwords) => {
  return request.put('/auth/password', passwords)
}

/**
 * 更新用户资料
 * @param {Object} profile - 用户资料 { email, displayName, department }
 * @returns {Promise} 更新资料响应
 */
export const updateProfileApi = (profile) => {
  return request.put('/user/profile', profile)
}

/**
 * 获取用户资料
 * @returns {Promise} 用户资料响应
 */
export const getProfileApi = () => {
  return request.get('/user/profile')
}

/**
 * 修改当前用户密码
 * @param {Object} passwords - 密码信息 { oldPassword, newPassword, confirmPassword }
 * @returns {Promise} 修改密码响应
 */
export const changeCurrentUserPasswordApi = (passwords) => {
  return request.put('/user/change-password', passwords)
}