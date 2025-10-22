<template>
    <img 
      v-if="svgIcon" 
      :src="svgIcon" 
      :alt="extension" 
      class="file-svg-icon" 
      :class="iconClass"
    />
</template>

<script setup>
import { computed } from 'vue'

// 导入 SVG 图标
import ImageSvg from '../assets/iconfont/svg/IMAGE.svg'
import VideoSvg from '../assets/iconfont/svg/OTHER.svg'
import AudioSvg from '../assets/iconfont/svg/OTHER.svg'
import DocumentSvg from '../assets/iconfont/svg/OTHER.svg'
import ArchiveSvg from '../assets/iconfont/svg/ZIP.svg'
import CodeSvg from '../assets/iconfont/svg/OTHER.svg'
import FileSvg from '../assets/iconfont/svg/OTHER.svg'
import PdfSvg from '../assets/iconfont/svg/PDF.svg'
import WordSvg from '../assets/iconfont/svg/WORD.svg'
import ExcelSvg from '../assets/iconfont/svg/EXCEL.svg'
import PptSvg from '../assets/iconfont/svg/PPT.svg'
import JpgSvg from '../assets/iconfont/svg/JPG.svg'
import PngSvg from '../assets/iconfont/svg/PNG.svg'
import GifSvg from '../assets/iconfont/svg/GIF.svg'
import Mp4Svg from '../assets/iconfont/svg/MP4.svg'
import AviSvg from '../assets/iconfont/svg/AVI.svg'
import MovSvg from '../assets/iconfont/svg/MOV.svg'
import Mp3Svg from '../assets/iconfont/svg/WAV.svg'
import ZipSvg from '../assets/iconfont/svg/ZIP.svg'

// 定义 props
const props = defineProps({
  extension: {
    type: String,
    default: ''
  },
  size: {
    type: String,
    default: 'md' // sm, md, lg
  }
})

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

const isArchiveFile = (extension) => {
  const archiveExtensions = ['.zip', '.rar', '.7z', '.tar', '.gz', '.bz2', '.xz', '.lz', '.lzma', '.tlz', '.tgz', '.tbz2', '.txz', '.z', '.Z', '.arj', '.cab', '.lzh', '.lha', '.ace', '.arc', '.sfx', '.sit', '.sitx', '.sea', '.hqx', '.sqx', '.shar', '.swm', '.xar', '.pkg', '.rpm', '.deb', '.dmg', '.iso']
  return archiveExtensions.includes(extension.toLowerCase())
}

const isCodeFile = (extension) => {
  const codeExtensions = ['.js', '.ts', '.html', '.css', '.php', '.py', '.java', '.c', '.cpp', '.cs', '.go', '.rb', '.swift', '.kt', '.kts', '.rs', '.sh', '.bash', '.pl', '.pm', '.t', '.r', '.sql', '.lua', '.perl', '.ps1', '.psm1', '.psd1', '.vbs', '.bat', '.cmd', '.clj', '.cljs', '.cljc', '.edn', '.erl', '.hrl', '.ex', '.exs', '.elm', '.groovy', '.gvy', '.gy', '.gsh', '.hs', '.lhs', '.coffee', '.litcoffee', '.iced', '.jl', '.dart', '.fs', '.fsi', '.fsx', '.fsscript', '.hx', '.hxml', '.ml', '.mli', '.mll', '.mly', '.nb', '.wl', '.wls', '.scm', '.ss', '.sc', '.scala', '.sbt', '.scd', '.vala', '.vapi', '.v', '.sv', '.svh', '.vh', '.vhd', '.vhdl']
  return codeExtensions.includes(extension.toLowerCase())
}

// 获取 SVG 图标
const getSvgIcon = (extension) => {
  const normalizedExtension = extension.startsWith('.') ? extension.substring(1).toUpperCase() : extension.toUpperCase()
  
  switch (normalizedExtension) {
    case 'JPG':
    case 'JPEG':
      return JpgSvg
    case 'PNG':
      return PngSvg
    case 'GIF':
      return GifSvg
    case 'PDF':
      return PdfSvg
    case 'DOC':
    case 'DOCX':
      return WordSvg
    case 'XLS':
    case 'XLSX':
      return ExcelSvg
    case 'PPT':
    case 'PPTX':
      return PptSvg
    case 'MP4':
      return Mp4Svg
    case 'AVI':
      return AviSvg
    case 'MOV':
      return MovSvg
    case 'MP3':
    case 'WAV':
      return Mp3Svg
    case 'ZIP':
    case 'RAR':
    case '7Z':
      return ZipSvg
    default:
      if (isImageFile(extension)) {
        return ImageSvg
      } else if (isVideoFile(extension)) {
        return VideoSvg
      } else if (isAudioFile(extension)) {
        return AudioSvg
      } else if (isDocumentFile(extension)) {
        return DocumentSvg
      } else if (isArchiveFile(extension)) {
        return ArchiveSvg
      } else if (isCodeFile(extension)) {
        return CodeSvg
      } else {
        return FileSvg
      }
  }
}

// 计算属性
const svgIcon = computed(() => {
  if (props.extension) {
    return getSvgIcon(props.extension)
  }
  return FileSvg
})

const iconText = computed(() => {
  if (isImageFile(props.extension)) return '🖼️'
  if (isVideoFile(props.extension)) return '🎬'
  if (isAudioFile(props.extension)) return '🎵'
  if (isDocumentFile(props.extension)) return '📄'
  if (isArchiveFile(props.extension)) return '📦'
  if (isCodeFile(props.extension)) return '💻'
  return '📁'
})

const iconClass = computed(() => {
  return [
    `file-icon-${props.size}`,
    {
      'file-icon-image': isImageFile(props.extension),
      'file-icon-video': isVideoFile(props.extension),
      'file-icon-audio': isAudioFile(props.extension),
      'file-icon-document': isDocumentFile(props.extension),
      'file-icon-archive': isArchiveFile(props.extension),
      'file-icon-code': isCodeFile(props.extension),
      'file-icon-default': !props.extension || (!isImageFile(props.extension) && !isVideoFile(props.extension) && 
                              !isAudioFile(props.extension) && !isDocumentFile(props.extension) && 
                              !isArchiveFile(props.extension) && !isCodeFile(props.extension))
    }
  ]
})

</script>

<style scoped>

.file-svg-icon {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

.file-icon-sm {
  width: 24px;
  height: 24px;
}

.file-icon-md {
  width: 48px;
  height: 48px;
}

.file-icon-lg {
  width: 64px;
  height: 64px;
}

.file-icon-image {
  color: #4caf50;
}

.file-icon-video {
  color: #f44336;
}

.file-icon-audio {
  color: #ff9800;
}

.file-icon-document {
  color: #2196f3;
}

.file-icon-archive {
  color: #9c27b0;
}

.file-icon-code {
  color: #607d8b;
}

.file-icon-default {
  color: #757575;
}

.file-icon-text {
  font-size: 24px;
}
</style>
