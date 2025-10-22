namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 更新用户请求参数
    /// </summary>
    public class UpdateUserRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? Username { get; set; }
        
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
        public string? Role { get; set; }
        
        /// <summary>
        /// 用户状态
        /// </summary>
        public UserStatus Status { get; set; }
    }
}