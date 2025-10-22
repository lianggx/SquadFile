// 文件图标映射工具函数

// 根据文件扩展名获取对应的SVG图标
export const getSvgIcon = (extension, iconComponents) => {
  // 确保扩展名以点开头并转换为大写进行比较
  const normalizedExtension = extension.startsWith('.') ? extension.substring(1).toUpperCase() : extension.toUpperCase();
  
  // 根据扩展名返回对应的导入的SVG组件
  switch (normalizedExtension) {
    case 'JPG':
    case 'JPEG':
      return iconComponents.JpgSvg;
    case 'PNG':
      return iconComponents.PngSvg;
    case 'GIF':
      return iconComponents.GifSvg;
    case 'SVG':
      return iconComponents.SvgSvg;
    case 'PDF':
      return iconComponents.PdfSvg;
    case 'DOC':
    case 'DOCX':
      return iconComponents.WordSvg;
    case 'XLS':
    case 'XLSX':
      return iconComponents.ExcelSvg;
    case 'PPT':
    case 'PPTX':
      return iconComponents.PptSvg;
    case 'MP4':
      return iconComponents.Mp4Svg;
    case 'AVI':
      return iconComponents.AviSvg;
    case 'MOV':
      return iconComponents.MovSvg;
    case 'MP3':
    case 'WAV':
      return iconComponents.Mp3Svg;
    case 'ZIP':
    case 'RAR':
      return iconComponents.ZipSvg;
    default:
      // 对于没有特定图标的文件类型，返回默认的文档图标
      if (isImageFile(extension)) {
        return iconComponents.ImageSvg;
      } else if (isVideoFile(extension)) {
        return iconComponents.VideoSvg;
      } else if (isAudioFile(extension)) {
        return iconComponents.AudioSvg;
      } else if (isDocumentFile(extension)) {
        return iconComponents.DocumentSvg;
      } else if (isArchiveFile(extension)) {
        return iconComponents.ArchiveSvg;
      } else if (isCodeFile(extension)) {
        return iconComponents.CodeSvg;
      } else {
        return iconComponents.FileSvg;
      }
  }
};

// 获取文件扩展名
export const getFileExtension = (fileName) => {
  return '.' + fileName.split('.').pop().toLowerCase();
};

// 判断是否为图片文件
const isImageFile = (extension) => {
  const imageExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp', '.svg', '.ico', '.tiff', '.tif', '.raw', '.cr2', '.nef', '.arw', '.dng'];
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`;
  return imageExtensions.includes(normalizedExtension);
};

// 判断是否为视频文件
const isVideoFile = (extension) => {
  const videoExtensions = ['.mp4', '.mov', '.avi', '.mkv', '.wmv', '.flv', '.webm', '.m4v', '.3gp', '.3g2', '.mpeg', '.mpg', '.m2v', '.m4v', '.vob', '.ogv', '.ogg', '.drc', '.gifv', '.qt', '.yuv'];
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`;
  return videoExtensions.includes(normalizedExtension);
};

// 判断是否为音频文件
const isAudioFile = (extension) => {
  const audioExtensions = ['.mp3', '.wav', '.aac', '.flac', '.m4a', '.wma', '.ogg', '.oga', '.opus', '.aiff', '.aif', '.aifc', '.au', '.snd', '.mid', '.midi', '.mp2', '.m3u', '.pls'];
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`;
  return audioExtensions.includes(normalizedExtension);
};

// 判断是否为文档文件
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
  ];
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`;
  return documentExtensions.includes(normalizedExtension);
};

// 判断是否为压缩文件
const isArchiveFile = (extension) => {
  const archiveExtensions = ['.zip', '.rar', '.7z', '.tar', '.gz', '.bz2', '.xz', '.lz', '.lzma', '.tlz', '.tgz', '.tbz2', '.txz', '.z', '.Z', '.arj', '.cab', '.lzh', '.lha', '.ace', '.arc', '.sfx', '.sit', '.sitx', '.sea', '.hqx', '.sqx', '.shar', '.swm', '.xar', '.pkg', '.rpm', '.deb', '.dmg', '.iso'];
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`;
  return archiveExtensions.includes(normalizedExtension);
};

// 判断是否为代码文件
const isCodeFile = (extension) => {
  const codeExtensions = ['.js', '.ts', '.html', '.css', '.php', '.py', '.java', '.c', '.cpp', '.cs', '.go', '.rb', '.swift', '.kt', '.kts', '.rs', '.sh', '.bash', '.pl', '.pm', '.t', '.r', '.sql', '.lua', '.perl', '.ps1', '.psm1', '.psd1', '.vbs', '.bat', '.cmd', '.clj', '.cljs', '.cljc', '.edn', '.erl', '.hrl', '.ex', '.exs', '.elm', '.groovy', '.gvy', '.gy', '.gsh', '.hs', '.lhs', '.coffee', '.litcoffee', '.iced', '.jl', '.dart', '.fs', '.fsi', '.fsx', '.fsscript', '.hx', '.hxml', '.ml', '.mli', '.mll', '.mly', '.nb', '.wl', '.wls', '.scm', '.ss', '.sc', '.scala', '.sbt', '.scd', '.vala', '.vapi', '.v', '.sv', '.svh', '.vh', '.vhd', '.vhdl'];
  const normalizedExtension = extension.startsWith('.') ? extension.toLowerCase() : `.${extension.toLowerCase()}`;
  return codeExtensions.includes(normalizedExtension);
};