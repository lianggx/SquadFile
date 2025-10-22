namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 修改密码请求参数
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; } = string.Empty;
        
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
        
        /// <summary>
        /// 确认新密码
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}