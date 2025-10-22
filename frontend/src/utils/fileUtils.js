import { useI18n } from 'vue-i18n'

// 判断是否为图片文件
export const isImageFile = (extension) => {
  const imageExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp', '.svg', '.ico', '.tiff', '.tif', '.raw', '.cr2', '.nef', '.arw', '.dng']
  // 确保扩展名以点开头并转换为小写进行比较
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return imageExtensions.includes(normalizedExtension)
}

// 判断是否为视频文件
export const isVideoFile = (extension) => {
  const videoExtensions = ['.mp4', '.avi', '.mov', '.wmv', '.flv', '.mkv', '.webm', '.m4v', '.3gp', '.3g2', '.mpeg', '.mpg', '.m2v', '.m4v', '.vob', '.ogv', '.ogg', '.drc', '.gifv', '.qt', '.yuv']
  // 确保扩展名以点开头并转换为小写进行比较
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return videoExtensions.includes(normalizedExtension)
}

// 判断是否为音频文件
export const isAudioFile = (extension) => {
  const audioExtensions = ['.mp3', '.wav', '.ogg', '.flac', '.aac', '.m4a', '.wma', '.opus', '.aiff', '.aif', '.aifc', '.au', '.snd', '.mid', '.midi', '.mp2', '.m3u', '.pls']
  // 确保扩展名以点开头并转换为小写进行比较
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return audioExtensions.includes(normalizedExtension)
}

// 判断是否为文档文件
export const isDocumentFile = (extension) => {
  const documentExtensions = ['.pdf', '.doc', '.docx', '.xls', '.xlsx', '.ppt', '.pptx', '.txt', '.rtf', 
    '.odt', '.ods', '.odp', '.pages', '.numbers', '.key', '.tex', '.epub', '.mobi',
    '.djvu', '.xps', '.oxps', '.ps', '.eps', '.ai', '.indd',
    '.docm', '.dot', '.dotx', '.dotm', '.xlsb', '.xlsm', 
    '.xlt', '.xltx', '.xltm', '.pptm', '.pot', '.potx', 
    '.potm', '.ppsx', '.ppsm', '.sldx', '.sldm',
    // 添加更多支持的文档格式
    '.fb2', '.azw', '.azw3', '.prc', '.lit', '.pdb', '.chm',
    '.hlp', '.msg', '.eml', '.vcf', '.ics', '.vcs']
  // 确保扩展名以点开头并转换为小写进行比较
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return documentExtensions.includes(normalizedExtension)
}

// 判断是否为压缩文件
export const isArchiveFile = (extension) => {
  const archiveExtensions = ['.zip', '.rar', '.7z', '.tar', '.gz', '.bz2', '.xz', '.lz', '.lzma', '.tlz', '.tgz', '.tbz2', '.txz', '.z', '.Z', '.arj', '.cab', '.lzh', '.lha', '.ace', '.arc', '.sfx', '.sit', '.sitx', '.sea', '.hqx', '.sqx', '.shar', '.swm', '.xar', '.pkg', '.rpm', '.deb', '.dmg', '.iso']
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return archiveExtensions.includes(normalizedExtension)
}

// 判断是否为代码文件
export const isCodeFile = (extension) => {
  const codeExtensions = ['.js', '.ts', '.html', '.css', '.java', '.cpp', '.c', '.cs', '.py', '.php', '.rb', '.go', '.sql', '.lua', '.perl', '.ps1', '.psm1', '.psd1', '.vbs', '.bat', '.cmd', '.clj', '.cljs', '.cljc', '.edn', '.erl', '.hrl', '.ex', '.exs', '.elm', '.groovy', '.gvy', '.gy', '.gsh', '.hs', '.lhs', '.coffee', '.litcoffee', '.iced', '.jl', '.dart', '.fs', '.fsi', '.fsx', '.fsscript', '.hx', '.hxml', '.ml', '.mli', '.mll', '.mly', '.nb', '.wl', '.wls', '.scm', '.ss', '.sc', '.scala', '.sbt', '.scd', '.vala', '.vapi', '.v', '.sv', '.svh', '.vh', '.vhd', '.vhdl']
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return codeExtensions.includes(normalizedExtension)
}

// 判断是否为文本文件
export const isTextFile = (extension) => {
  const textExtensions = ['.txt', '.md']
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return textExtensions.includes(normalizedExtension)
}

// 获取视频MIME类型
export const getVideoMimeType = (extension) => {
  const mimeTypes = {
    '.mp4': 'video/mp4',
    '.webm': 'video/webm',
    '.ogg': 'video/ogg',
    '.mov': 'video/quicktime',
    '.avi': 'video/x-msvideo',
    '.mkv': 'video/x-matroska'
  }
  return mimeTypes[extension.toLowerCase()] || 'video/mp4'
}

// 获取音频MIME类型
export const getAudioMimeType = (extension) => {
  const mimeTypes = {
    '.mp3': 'audio/mpeg',
    '.wav': 'audio/wav',
    '.aac': 'audio/aac',
    '.flac': 'audio/flac',
    '.m4a': 'audio/mp4',
    '.wma': 'audio/x-ms-wma',
    '.ogg': 'audio/ogg',
    '.oga': 'audio/ogg',
    '.opus': 'audio/opus',
    '.aiff': 'audio/aiff',
    '.aif': 'audio/aiff',
    '.aifc': 'audio/aiff'
  }
  return mimeTypes[extension.toLowerCase()] || 'audio/mpeg'
}

// 格式化文件大小
export const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// 格式化日期
export const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  // 检查日期是否有效
  if (isNaN(date.getTime())) return '';
  return date.toLocaleDateString();
};

// 格式化时间（仅日期部分）
export const formatTime = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  // 检查日期是否有效
  if (isNaN(date.getTime())) return '';
  return date.toLocaleDateString();
};

// 获取文件扩展名
export const getFileExtension = (fileName) => {
  return '.' + fileName.split('.').pop().toLowerCase()
}

// 获取文件类型显示名称
export const getFileTypeDisplayName = (extension) => {
  // 直接在函数内部使用 useI18n
  const { t } = useI18n();
  
  if (isImageFile(extension)) return t('admin.fileTypes.image');
  if (isVideoFile(extension)) return t('admin.fileTypes.video');
  if (isAudioFile(extension)) return t('admin.fileTypes.audio');
  if (isArchiveFile(extension)) return t('admin.fileTypes.archive');
  if (isCodeFile(extension)) return t('admin.fileTypes.code');
  if (isDocumentFile(extension)) {
    // 更具体的文档类型
    const ext = extension.toLowerCase();
    if (ext === '.pdf') return t('admin.fileTypes.pdf');
    if (ext === '.doc' || ext === '.docx' || ext === '.docm' || ext === '.dot' || ext === '.dotx' || ext === '.dotm') 
      return t('admin.fileTypes.word');
    if (ext === '.xls' || ext === '.xlsx' || ext === '.xlsb' || ext === '.xlsm' || ext === '.xlt' || ext === '.xltx' || ext === '.xltm') 
      return t('admin.fileTypes.excel');
    if (ext === '.ppt' || ext === '.pptx' || ext === '.pptm' || ext === '.pot' || ext === '.potx' || ext === '.potm' || ext === '.ppsx' || ext === '.ppsm' || ext === '.sldx' || ext === '.sldm') 
      return t('admin.fileTypes.powerpoint');
    if (ext === '.txt' || ext === '.md' || ext === '.rtf') 
      return t('admin.fileTypes.text');
    if (ext === '.epub' || ext === '.mobi' || ext === '.azw' || ext === '.azw3' || ext === '.fb2' || ext === '.prc' || ext === '.lit' || ext === '.pdb') 
      return t('admin.fileTypes.ebook');
    if (ext === '.msg' || ext === '.eml') 
      return t('admin.fileTypes.email');
    if (ext === '.ics' || ext === '.vcs') 
      return t('admin.fileTypes.calendar');
    return t('admin.fileTypes.document');
  }
  return t('admin.fileTypes.document');
};
