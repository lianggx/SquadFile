namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 撤销权限请求DTO
    /// </summary>
    public class RevokePermissionRequest
    {
        /// <summary>
        /// 文件夹ID
        /// </summary>
        public int FolderId { get; set; }
        
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
    }
}