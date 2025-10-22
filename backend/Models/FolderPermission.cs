using System.ComponentModel.DataAnnotations.Schema;

namespace SquadFile.Models
{
    /// <summary>
    /// 文件夹权限实体
    /// </summary>
    [Table("folder_permission")]
    public class FolderPermission
    {
        public int Id { get; set; }
        
        /// <summary>
        /// 文件夹ID
        /// </summary>
        public int FolderId { get; set; }
        
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 是否有读取权限
        /// </summary>
        public bool CanRead { get; set; } = true;
        
        /// <summary>
        /// 是否有上传权限
        /// </summary>
        public bool CanUpload { get; set; } = false;
        
        /// <summary>
        /// 是否有删除权限
        /// </summary>
        public bool CanDelete { get; set; } = false;
        
        /// <summary>
        /// 是否有创建文件夹权限
        /// </summary>
        public bool CanCreateFolder { get; set; } = false;
        
        /// <summary>
        /// 授权时间
        /// </summary>
        public DateTime GrantTime { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 授权者用户ID
        /// </summary>
        public int GrantedBy { get; set; }
        
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedTime { get; set; }
    }
}