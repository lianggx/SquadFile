namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 重置密码请求参数
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }
}