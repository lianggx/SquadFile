import request from './request'

/**
 * 获取所有用户
 * @returns {Promise} 用户列表响应
 */
export const getAllUsersApi = () => {
  return request.get('/user')
}

/**
 * 创建用户
 * @param {Object} userData - 用户数据
 * @returns {Promise} 创建用户响应
 */
export const createUserApi = (userData) => {
  return request.post('/user', userData)
}

/**
 * 更新用户
 * @param {number} userId - 用户ID
 * @param {Object} userData - 用户数据
 * @returns {Promise} 更新用户响应
 */
export const updateUserApi = (userId, userData) => {
  return request.put(`/user/${userId}`, userData)
}

/**
 * 删除用户
 * @param {number} userId - 用户ID
 * @returns {Promise} 删除用户响应
 */
export const deleteUserApi = (userId) => {
  return request.delete(`/user/${userId}`)
}

/**
 * 重置用户密码
 * @param {number} userId - 用户ID
 * @param {string} newPassword - 新密码
 * @returns {Promise} 重置密码响应
 */
export const resetUserPasswordApi = (userId, newPassword) => {
  return request.post(`/user/${userId}/reset-password`, { newPassword })
}