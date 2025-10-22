using System.ComponentModel.DataAnnotations.Schema;

namespace SquadFile.Models
{
    public enum UserRole
    {
        Normal,
        Admin
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Locked
    }

    [Table("sys_user")]
    public class SysUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Avatar { get; set; }
        public string? Department { get; set; }
        public string Role { get; set; } = nameof(UserRole.Normal);
        public UserStatus Status { get; set; } = UserStatus.Active;
        public bool IsFirstLogin { get; set; } = true;
        public DateTime? PasswordExpiry { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string? LastLoginIp { get; set; }
        public int LoginFailCount { get; set; } = 0;
        public DateTime? LockTime { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
        public DateTime? DeletedTime { get; set; }
        // 语言偏好设置
        public string? PreferredLanguage { get; set; }
    }
}