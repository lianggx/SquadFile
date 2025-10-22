using System.ComponentModel.DataAnnotations.Schema;

namespace SquadFile.Models
{
    /// <summary>
    /// 文件记录实体
    /// </summary>
    [Table("file_record")]
    public class FileRecord
    {
        public int Id { get; set; }
        
        /// <summary>
        /// 文件原始名称
        /// </summary>
        public string OriginalName { get; set; } = string.Empty;
        
        /// <summary>
        /// 存储文件名（唯一标识）
        /// </summary>
        public string StorageName { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long Size { get; set; }
        
        /// <summary>
        /// 文件类型/扩展名
        /// </summary>
        public string Extension { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件夹ID
        /// </summary>
        public int FolderId { get; set; }
        
        /// <summary>
        /// 上传者用户ID
        /// </summary>
        public int UploadedBy { get; set; }
        
        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedTime { get; set; }
        
        /// <summary>
        /// 文件描述
        /// </summary>
        public string? Description { get; set; }
    }
}