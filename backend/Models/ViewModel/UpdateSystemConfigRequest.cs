using System.ComponentModel.DataAnnotations;

namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 更新系统配置请求参数
    /// </summary>
    public class UpdateSystemConfigRequest
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
    }
}