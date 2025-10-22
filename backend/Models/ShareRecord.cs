using System.ComponentModel.DataAnnotations.Schema;

namespace SquadFile.Models
{
    /// <summary>
    /// 分享记录实体
    /// </summary>
    [Table("share_record")]
    public class ShareRecord
    {
        public int Id { get; set; }
        
        /// <summary>
        /// 被分享的项目ID（文件ID或文件夹ID）
        /// </summary>
        public int ItemId { get; set; }
        
        /// <summary>
        /// 项目类型（文件或文件夹）
        /// </summary>
        public ShareItemType ItemType { get; set; }
        
        /// <summary>
        /// 分享短链接代码
        /// </summary>
        public string ShortCode { get; set; } = string.Empty;
        
        /// <summary>
        /// 分享密码（可选）
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 过期时间（可选）
        /// </summary>
        public DateTime? ExpireTime { get; set; }
        
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedTime { get; set; }
    }
    
    /// <summary>
    /// 分享项目类型枚举
    /// </summary>
    public enum ShareItemType
    {
        File,
        Folder
    }
}