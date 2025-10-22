namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 分享文件请求DTO
    /// </summary>
    public class ShareFileRequest
    {
        /// <summary>
        /// 分享密码
        /// </summary>
        public string? Password { get; set; }
        
        /// <summary>
        /// 过期时间（可选）
        /// </summary>
        public DateTime? ExpireTime { get; set; }
    }
}