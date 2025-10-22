namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 创建文件夹请求DTO
    /// </summary>
    public class CreateFolderRequest
    {
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 父文件夹ID（可选）
        /// </summary>
        public int? ParentId { get; set; }
        
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