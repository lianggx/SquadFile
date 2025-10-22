using System.ComponentModel.DataAnnotations.Schema;

namespace SquadFile.Models
{
    /// <summary>
    /// 文件夹实体
    /// </summary>
    [Table("file_folder")]
    public class FileFolder
    {
        public int Id { get; set; }
        
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 父文件夹ID（用于支持嵌套文件夹）
        /// </summary>
        public int? ParentId { get; set; }
        
        /// <summary>
        /// 创建者用户ID
        /// </summary>
        public int CreatedBy { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
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
        /// 文件夹描述
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// 是否公开（允许所有登录用户访问）
        /// </summary>
        public bool IsPublic { get; set; } = false;
    }
}