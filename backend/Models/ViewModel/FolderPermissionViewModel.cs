namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 文件夹权限DTO
    /// </summary>
    public class FolderPermissionViewModel
    {
        public int Id { get; set; }
        public int FolderId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public bool CanRead { get; set; } = true;
        public bool CanUpload { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public bool CanCreateFolder { get; set; } = false;
        public DateTime GrantTime { get; set; }
        public int GrantedBy { get; set; }
        public string GrantedByName { get; set; } = string.Empty;
    }
}