namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 文件夹DTO
    /// </summary>
    public class FileFolderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string? Description { get; set; }
        public int FileCount { get; set; } // 文件数量
        public bool IsPublic { get; set; } // 是否公开
        public string UserName { get; set; } = string.Empty; // 添加创建者名称
    }
}