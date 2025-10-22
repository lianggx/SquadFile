namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 刷新令牌请求参数
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
    }
}