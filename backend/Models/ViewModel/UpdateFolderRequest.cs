namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 更新文件夹请求DTO
    /// </summary>
    public class UpdateFolderRequest
    {
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件夹描述
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// 是否公开（允许所有登录用户访问）
        /// </summary>
        public bool IsPublic { get; set; }
    }
}