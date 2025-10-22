namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 文件记录DTO
    /// </summary>
    public class FileRecordViewModel
    {
        public int Id { get; set; }
        public string OriginalName { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Extension { get; set; } = string.Empty;
        public int FolderId { get; set; }
        public int UploadedBy { get; set; }
        public DateTime UploadTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string? Description { get; set; }
        public bool IsShared { get; set; }
        public DateTime? ShareCreateTime { get; set; }
        public DateTime? ShareExpireTime { get; set; }
        public string FormattedSize { get; set; } = string.Empty; // 格式化的文件大小
        public string UserName { get; set; }
    }
}