using System.ComponentModel.DataAnnotations;

namespace SquadFile.Models
{
    /// <summary>
    /// 临时下载Token模型
    /// </summary>
    public class TemporaryDownloadToken
    {
        /// <summary>
        /// Token ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Token值
        /// </summary>
        [Required]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 文件ID
        /// </summary>
        [Required]
        public int FileId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Required]
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// 是否已使用
        /// </summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// 使用时间
        /// </summary>
        public DateTime? UsedTime { get; set; }
    }
}