import { ref } from 'vue'

// 下载状态管理
const downloadingFiles = ref(new Set())

/**
 * 公共下载函数
 * @param {string} url - 下载链接
 * @param {string} filename - 文件名
 * @param {string} fileId - 文件ID（用于状态管理）
 * @returns {Promise<boolean>} 下载是否成功
 */
export async function downloadFile(url, filename, fileId = null) {
  // 如果正在下载该文件，则不处理重复点击
  if (fileId && downloadingFiles.value.has(fileId)) {
    return false
  }
  
  try {
    if (fileId) {
      downloadingFiles.value.add(fileId)
    }
    
    // 使用 fetch + blob URL 方式创建下载链接，可以更好地控制下载过程
    const response = await fetch(url)
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    
    const blob = await response.blob()
    const blobUrl = window.URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.style.display = 'none'
    a.href = blobUrl
    a.download = filename || 'download'
    document.body.appendChild(a)
    a.click()
    window.URL.revokeObjectURL(blobUrl)
    document.body.removeChild(a)
    
    // 下载完成后重置状态
    if (fileId) {
      downloadingFiles.value.delete(fileId)
    }
    
    return true
  } catch (error) {
    console.error('下载文件失败:', error)
    
    // 如果 fetch 失败，回退到原来的下载方式
    try {
      const link = document.createElement('a')
      link.href = url
      link.download = filename || 'download'
      link.style.display = 'none'
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      
      // 下载完成后重置状态
      if (fileId) {
        downloadingFiles.value.delete(fileId)
      }
      
      return true
    } catch (fallbackError) {
      console.error('回退下载方式也失败了:', fallbackError)
      
      // 短暂延迟后重置状态
      if (fileId) {
        setTimeout(() => {
          downloadingFiles.value.delete(fileId)
        }, 1000)
      }
      
      return false
    }
  }
}

/**
 * 检查文件是否正在下载
 * @param {string} fileId - 文件ID
 * @returns {boolean} 是否正在下载
 */
export function isFileDownloading(fileId) {
  return downloadingFiles.value.has(fileId)
}