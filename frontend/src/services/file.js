import request from './request'

// 创建文件夹
export const createFolderApi = (data) => {
  return request({
    url: '/filemanagement/folders',
    method: 'post',
    data
  })
}

// 获取用户文件夹列表
export const getUserFoldersApi = () => {
  return request({
    url: '/filemanagement/folders',
    method: 'get'
  })
}

// 获取文件夹中的文件列表
export const getFilesInFolderApi = (folderId) => {
  return request({
    url: `/filemanagement/folders/${folderId}/files`,
    method: 'get'
  })
}

// 授权文件夹权限
export const grantFolderPermissionApi = (data) => {
  return request({
    url: '/filemanagement/permissions',
    method: 'post',
    data
  })
}

// 撤销文件夹权限
export const revokeFolderPermissionApi = (folderId, userId) => {
  return request({
    url: `/filemanagement/permissions/folders/${folderId}/users/${userId}`,
    method: 'delete'
  })
}

// 获取文件夹权限列表
export const getFolderPermissionsApi = (folderId) => {
  return request({
    url: `/filemanagement/permissions/folders/${folderId}`,
    method: 'get'
  })
}

// 删除文件夹
export const deleteFolderApi = (folderId) => {
  return request({
    url: `/filemanagement/folders/${folderId}`,
    method: 'delete'
  })
}

// 更新文件夹
export const updateFolderApi = (folderId, data) => {
  return request({
    url: `/filemanagement/folders/${folderId}`,
    method: 'put',
    data
  })
}

// 准备文件上传
export const prepareFileUploadApi = (data) => {
  return request({
    url: '/filemanagement/upload/prepare',
    method: 'post',
    data
  })
}

// 上传文件块
export const uploadFileChunkApi = (fileId, chunkIndex, totalChunks, file, onUploadProgress) => {
  const formData = new FormData()
  formData.append('chunkIndex', chunkIndex)
  formData.append('totalChunks', totalChunks)
  formData.append('file', file)
  
  return request({
    url: `/filemanagement/upload/chunk/${fileId}`,
    method: 'post',
    data: formData,
    headers: {
      'Content-Type': 'multipart/form-data'
    },
    onUploadProgress: onUploadProgress
  })
}

// 下载文件
export const downloadFileApi = (fileId,token) => {
  return request({
    url: `/filemanagement/download/${fileId}?token=${token??null}`,
    method: 'get'
  })
}

// 删除文件
export const deleteFileApi = (fileId) => {
  return request({
    url: `/filemanagement/files/${fileId}`,
    method: 'delete'
  })
}

// 分享文件
export const shareFileApi = (fileId, data) => {
  return request({
    url: `/fileshare/file/${fileId}`,
    method: 'post',
    data
  })
}

// 分享文件夹
export const shareFolderApi = (folderId, data) => {
  return request({
    url: `/fileshare/folder/${folderId}`,
    method: 'post',
    data
  })
}

// 验证分享密码
export const validateSharePasswordApi = (shortCode, password) => {
  return request({
    url: `/fileshare/validate/${shortCode}`,
    method: 'post',
    data: { password }
  })
}

// 生成文件分享短链接
export const generateFileShareLinkApi = (fileId) => {
  return request({
    url: `/fileshare/generate/file/${fileId}`,
    method: 'get'
  })
}

// 生成文件夹分享短链接
export const generateFolderShareLinkApi = (folderId) => {
  return request({
    url: `/fileshare/generate/folder/${folderId}`,
    method: 'get'
  })
}

// 生成文件下载URL
export const generateFileDownloadUrlApi = (fileId) => {
  return request({
    url: `/filemanagement/download-url/${fileId}`,
    method: 'get'
  });
}

// 获取分享项目信息
export const getShareItemInfoApi = (shortCode) => {
  return request({
    url: `/fileshare/info/${shortCode}`,
    method: 'get'
  });
}

// 搜索文件和文件夹
export const searchItemsApi = (query, folderId = 0) => {
  return request({
    url: `/filemanagement/search?query=${encodeURIComponent(query)}&folderId=${folderId}`,
    method: 'get'
  });
}

// 获取分享文件夹内容
export const getShareFolderContentApi = (folderId, token) => {
  return request({
    url: `/filemanagement/folders/${folderId}/files/share?token=${token}`,
    method: 'get'
  });
}

// 批量授权文件夹权限
export const grantFolderPermissionsBatchApi = (data) => {
  return request({
    url: '/filemanagement/permissions/batch',
    method: 'post',
    data
  });
}

// 批量撤销文件夹权限
export const revokeFolderPermissionsBatchApi = (data) => {
  return request({
    url: '/filemanagement/permissions/batch',
    method: 'delete',
    data
  });
}

// 获取短链接列表
export const getShareRecordsApi = (params = {}) => {
  const { searchQuery, page, pageSize } = params;
  let url = '/fileshare/records';
  
  // 构建查询参数
  const queryParams = [];
  if (searchQuery) queryParams.push(`searchQuery=${encodeURIComponent(searchQuery)}`);
  if (page) queryParams.push(`page=${page}`);
  if (pageSize) queryParams.push(`pageSize=${pageSize}`);
  
  if (queryParams.length > 0) {
    url += `?${queryParams.join('&')}`;
  }
  
  return request({
    url,
    method: 'get'
  });
}

// 批量删除短链接
export const deleteShareRecordsApi = (data) => {
  return request({
    url: '/fileshare/records/batch',
    method: 'delete',
    data
  });
}
