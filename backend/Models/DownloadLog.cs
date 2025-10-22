using System.ComponentModel.DataAnnotations.Schema;

namespace SquadFile.Models
{
    /// <summary>
    /// 下载日志实体
    /// </summary>
    [Table("download_log")]
    public class DownloadLog
    {
        public int Id { get; set; }
        
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 下载时间
        /// </summary>
        public DateTime DownloadTime { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 文件ID
        /// </summary>
        public int FileId { get; set; }
        
        /// <summary>
        /// 文件原始名称
        /// </summary>
        public string OriginalFileName { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long FileSize { get; set; }
        
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; } = string.Empty;
        
        /// <summary>
        /// 用户代理信息
        /// </summary>
        public string UserAgent { get; set; } = string.Empty;
    }
}