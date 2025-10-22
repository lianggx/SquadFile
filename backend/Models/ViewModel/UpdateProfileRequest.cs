namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 更新用户资料请求参数
    /// </summary>
    public class UpdateProfileRequest
    {
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
    }
}