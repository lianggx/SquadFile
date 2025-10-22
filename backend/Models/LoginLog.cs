using System.ComponentModel.DataAnnotations.Schema;

namespace SquadFile.Models
{
    /// <summary>
    /// 登录日志实体
    /// </summary>
    [Table("login_log")]
    public class LoginLog
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
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 登录IP地址
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// 用户代理信息
        /// </summary>
        public string UserAgent { get; set; } = string.Empty;
        
        /// <summary>
        /// 登录结果（成功/失败）
        /// </summary>
        public bool LoginResult { get; set; }
        
        /// <summary>
        /// 失败原因（如果登录失败）
        /// </summary>
        public string? FailureReason { get; set; }
    }
}