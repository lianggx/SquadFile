namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 授权创建文件夹权限请求DTO
    /// </summary>
    public class GrantCreateFolderPermissionRequest
    {
        /// <summary>
        /// 文件夹ID
        /// </summary>
        public int FolderId { get; set; }
        
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 是否有创建文件夹权限
        /// </summary>
        public bool CanCreateFolder { get; set; } = false;
    }
}