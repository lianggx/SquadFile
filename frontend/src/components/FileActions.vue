<template>
  <div class="file-actions">
    <button 
      v-if="canPreview"
      @click.stop="handlePreview"
      class="action-button preview-button"
    >
      {{ t('admin.preview') }}
    </button>
    <button 
      @click.stop="handleDownload"
      class="action-button download-button"
    >
      {{ t('admin.download') }}
    </button>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { generateFileDownloadUrlApi, downloadFileApi } from '../services/file'

// 定义 props
const props = defineProps({
  file: {
    type: Object,
    required: true
  }
})

// 定义 emits
const emit = defineEmits(['preview', 'download'])

const { t } = useI18n()

// 判断文件类型
const isImageFile = (extension) => {
  const imageExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp', '.svg', '.ico', '.tiff', '.tif', '.raw', '.cr2', '.nef', '.arw', '.dng']
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return imageExtensions.includes(normalizedExtension)
}

const isVideoFile = (extension) => {
  const videoExtensions = ['.mp4', '.mov', '.avi', '.mkv', '.wmv', '.flv', '.webm', '.m4v', '.3gp', '.3g2', '.mpeg', '.mpg', '.m2v', '.m4v', '.vob', '.ogv', '.ogg', '.drc', '.gifv', '.qt', '.yuv']
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return videoExtensions.includes(normalizedExtension)
}

const isAudioFile = (extension) => {
  const audioExtensions = ['.mp3', '.wav', '.aac', '.flac', '.m4a', '.wma', '.ogg', '.oga', '.opus', '.aiff', '.aif', '.aifc', '.au', '.snd', '.mid', '.midi', '.mp2', '.m3u', '.pls']
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return audioExtensions.includes(normalizedExtension)
}

const isDocumentFile = (extension) => {
  const documentExtensions = [
    '.doc', '.docx', '.xls', '.xlsx', '.ppt', '.pptx', 
    '.pdf', '.txt', '.md', '.csv', '.xml', '.html', '.htm', 
    '.js', '.css', '.json', '.rtf', '.odt', '.ods', '.odp', 
    '.pages', '.numbers', '.key', '.tex', '.epub', '.mobi',
    '.djvu', '.xps', '.oxps', '.ps', '.eps', '.ai', '.indd',
    '.docm', '.dot', '.dotx', '.dotm', '.xlsb', '.xlsm', 
    '.xlt', '.xltx', '.xltm', '.pptm', '.pot', '.potx', 
    '.potm', '.ppsx', '.ppsm', '.sldx', '.sldm',
    '.fb2', '.azw', '.azw3', '.prc', '.lit', '.pdb', '.chm',
    '.hlp', '.msg', '.eml', '.vcf', '.ics', '.vcs'
  ]
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return documentExtensions.includes(normalizedExtension)
}

const isCodeFile = (extension) => {
  const codeExtensions = ['.js', '.ts', '.html', '.css', '.php', '.py', '.java', '.c', '.cpp', '.cs', '.go', '.rb', '.swift', '.kt', '.kts', '.rs', '.sh', '.bash', '.pl', '.pm', '.t', '.r', '.sql', '.lua', '.perl', '.ps1', '.psm1', '.psd1', '.vbs', '.bat', '.cmd', '.clj', '.cljs', '.cljc', '.edn', '.erl', '.hrl', '.ex', '.exs', '.elm', '.groovy', '.gvy', '.gy', '.gsh', '.hs', '.lhs', '.coffee', '.litcoffee', '.iced', '.jl', '.dart', '.fs', '.fsi', '.fsx', '.fsscript', '.hx', '.hxml', '.ml', '.mli', '.mll', '.mly', '.nb', '.wl', '.wls', '.scm', '.ss', '.sc', '.scala', '.sbt', '.scd', '.vala', '.vapi', '.v', '.sv', '.svh', '.vh', '.vhd', '.vhdl']
  return codeExtensions.includes(extension.toLowerCase())
}

// 判断是否为文本文件
const isTextFile = (extension) => {
  const textExtensions = ['.txt', '.md']
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`
  return textExtensions.includes(normalizedExtension)
}

// 获取视频MIME类型
const getVideoMimeType = (extension) => {
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
const getAudioMimeType = (extension) => {
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

// 计算属性
const canPreview = computed(() => {
  const ext = props.file.extension
  return isImageFile(ext) || isVideoFile(ext) || isAudioFile(ext) || 
         isCodeFile(ext) || 
         (isDocumentFile(ext) && (ext.toLowerCase() === '.txt' || ext.toLowerCase() === '.md'))
})

// 处理预览
const handlePreview = async () => {
  try {
    // 根据文件类型使用不同的预览方式
    if (isImageFile(props.file.extension) || 
        isVideoFile(props.file.extension) || 
        isAudioFile(props.file.extension) || 
        isCodeFile(props.file.extension) || 
        (isDocumentFile(props.file.extension) && (props.file.extension.toLowerCase() === '.txt' || props.file.extension.toLowerCase() === '.md'))) {
      // 发出预览事件，让父组件处理模态框显示
      emit('preview', props.file)
    }
  } catch (error) {
    console.error('文件预览失败:', error)
  }
}

// 处理下载
const handleDownload = async () => {
  try {
    const response = await downloadFileApi(props.file.id)
    window.location.href = response.data.data.downloadUrl
    emit('download', props.file.id)
  } catch (error) {
    console.error('文件下载失败:', error)
  }
}
</script>

<style scoped>
.file-actions {
  display: flex;
  gap: 5px;
  margin-top: 10px;
  justify-content: center;
}

.action-button {
  padding: 5px 10px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  white-space: nowrap;
}

.preview-button {
  background-color: #6f42c1;
  color: white;
}

.download-button {
  background-color: #17a2b8;
  color: white;
}

.action-button:hover {
  opacity: 0.8;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .file-actions {
    flex-direction: column;
    gap: 5px;
  }
  
  .action-button {
    width: 100%;
  }
}
</style>