/**
 * 简单的SHA256哈希函数（在实际应用中，密码哈希应在后端进行）
 * 这里仅用于演示目的
 */
export const sha256 = async (message) => {
  // 在浏览器环境中使用Web Crypto API
  const msgBuffer = new TextEncoder().encode(message)
  const hashBuffer = await crypto.subtle.digest('SHA-256', msgBuffer)
  const hashArray = Array.from(new Uint8Array(hashBuffer))
  return hashArray.map(b => b.toString(16).padStart(2, '0')).join('')
}

/**
 * 在实际应用中，密码加密应该在后端进行，前端只需确保通过HTTPS传输
 * 这里提供一个简单的Base64编码函数用于演示
 */
export const base64Encode = (str) => {
  return btoa(unescape(encodeURIComponent(str)))
}

export const base64Decode = (str) => {
  return decodeURIComponent(escape(atob(str)))
}