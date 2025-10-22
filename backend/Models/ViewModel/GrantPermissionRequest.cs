namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 授权请求DTO
    /// </summary>
    public class GrantPermissionRequest
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
    }
}