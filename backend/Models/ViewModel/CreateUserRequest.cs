namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 创建用户请求参数
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string? Email { get; set; }
        
        /// <summary>
        /// 显示名称
        /// </summary>
        public string? DisplayName { get; set; }
        
        /// <summary>
        /// 所属部门
        /// </summary>
        public string? Department { get; set; }
        
        /// <summary>
        /// 用户角色
        /// </summary>
        public string Role { get; set; } = "Member";
        
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}