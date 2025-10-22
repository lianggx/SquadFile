namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 验证密码请求DTO
    /// </summary>
    public class ValidatePasswordRequest
    {
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}