namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 登录响应结果
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorKey { get; set; }
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public SysUserDto User { get; set; } = new();
    }

    /// <summary>
    /// 用户信息DTO
    /// </summary>
    public class SysUserDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// 头像URL
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 是否首次登录
        /// </summary>
        public bool IsFirstLogin { get; set; }
    }
}