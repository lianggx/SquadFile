using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadFile.Models
{
    /// <summary>
    /// 系统设置实体
    /// </summary>
    [Table("system_settings")]
    public class SystemSettings
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 默认语言
        /// </summary>
        public string DefaultLanguage { get; set; } = "zh-Hans";

        /// <summary>
        /// 站点名称
        /// </summary>
        public string? SiteName { get; set; }

        /// <summary>
        /// 登录页面LOGO路径
        /// </summary>
        public string? LoginLogoPath { get; set; }

        /// <summary>
        /// 首页LOGO路径
        /// </summary>
        public string? HomeLogoPath { get; set; }

        /// <summary>
        /// 最大文件大小（MB）
        /// </summary>
        public int MaxFileSize { get; set; } = 100;

        /// <summary>
        /// 存储限制（MB）
        /// </summary>
        public int StorageLimit { get; set; } = 10240;

        /// <summary>
        /// 文件存储路径
        /// </summary>
        public string FileStoragePath { get; set; } = "uploads";

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}