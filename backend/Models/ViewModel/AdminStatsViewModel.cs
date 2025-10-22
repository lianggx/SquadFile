using System.ComponentModel.DataAnnotations;

namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 管理统计数据DTO
    /// </summary>
    public class AdminStatsViewModel
    {
        /// <summary>
        /// 总用户数
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// 总文件数
        /// </summary>
        public int TotalFiles { get; set; }

        /// <summary>
        /// 总文件夹数
        /// </summary>
        public int TotalFolders { get; set; }

        /// <summary>
        /// 存储使用量（字节）
        /// </summary>
        public long StorageUsed { get; set; }

        /// <summary>
        /// 存储使用量显示文本
        /// </summary>
        public string StorageUsedDisplay { get; set; } = "0 MB";
        
        /// <summary>
        /// 总下载数
        /// </summary>
        public int TotalDownloads { get; set; }
        
        /// <summary>
        /// 最新上传的文件列表
        /// </summary>
        public List<LatestFileDto> LatestFiles { get; set; } = new List<LatestFileDto>();
        
        /// <summary>
        /// 文件类型分布
        /// </summary>
        public List<FileTypeDistributionDto> FileTypeDistribution { get; set; } = new List<FileTypeDistributionDto>();
        
        /// <summary>
        /// 总分享链接数
        /// </summary>
        public int TotalShareLinks { get; set; }
        
        /// <summary>
        /// 活跃分享链接数
        /// </summary>
        public int ActiveShareLinks { get; set; }
        
        /// <summary>
        /// 下载统计
        /// </summary>
        public List<DownloadStatDto> DownloadStats { get; set; } = new List<DownloadStatDto>();
    }
    
    /// <summary>
    /// 最新文件DTO
    /// </summary>
    public class LatestFileDto
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long Size { get; set; }
        
        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }
        
        /// <summary>
        /// 上传者用户ID
        /// </summary>
        public int UploadedByUserId { get; set; }
        
        /// <summary>
        /// 上传者用户名
        /// </summary>
        public string UploadedBy { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// 文件类型分布DTO
    /// </summary>
    public class FileTypeDistributionDto
    {
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string Extension { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件数量
        /// </summary>
        public int Count { get; set; }
    }
    
    /// <summary>
    /// 下载统计DTO
    /// </summary>
    public class DownloadStatDto
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public int FileId { get; set; }
        
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        
        /// <summary>
        /// 下载次数
        /// </summary>
        public int DownloadCount { get; set; }
    }
}