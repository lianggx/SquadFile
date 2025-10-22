namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 上传文件请求DTO
    /// </summary>
    public class UploadFileRequest
    {
        /// <summary>
        /// 文件夹ID
        /// </summary>
        public int FolderId { get; set; }
        
        /// <summary>
        /// 文件名
        /// </summary>
        public string OriginalName { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }
        
        /// <summary>
        /// 文件类型
        /// </summary>
        public string Extension { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件描述
        /// </summary>
        public string? Description { get; set; }
    }
}