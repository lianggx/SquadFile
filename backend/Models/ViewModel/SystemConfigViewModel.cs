using System.ComponentModel.DataAnnotations;

namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 系统配置DTO
    /// </summary>
    public class SystemConfigViewModel
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        public string? SiteName { get; set; }
        
        /// <summary>
        /// 登录页LOGO路径
        /// </summary>
        public string? LoginLogoPath { get; set; }
        
        /// <summary>
        /// 首页LOGO路径
        /// </summary>
        public string? HomeLogoPath { get; set; }
        
        /// <summary>
        /// 最大文件大小（MB）
        /// </summary>
        public int MaxFileSize { get; set; }
        
        /// <summary>
        /// 存储限制（MB）
        /// </summary>
        public int StorageLimit { get; set; }
        
        /// <summary>
        /// 文件存储路径
        /// </summary>
        public string FileStoragePath { get; set; } = "storage";
        
        /// <summary>
        /// 默认语言
        /// </summary>
        public string DefaultLanguage { get; set; } = "zh-Hans";
    }
}