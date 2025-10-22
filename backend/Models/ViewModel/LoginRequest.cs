namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 登录请求参数
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// 验证码
        /// </summary>
        public string Captcha { get; set; } = string.Empty;
        
        /// <summary>
        /// 验证码ID
        /// </summary>
        public string CaptchaId { get; set; } = string.Empty;
    }

    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public string? DeviceType { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string? DeviceName { get; set; }
        
        /// <summary>
        /// 用户代理
        /// </summary>
        public string? UserAgent { get; set; }
        
        /// <summary>
        /// IP地址
        /// </summary>
        public string? IpAddress { get; set; }
    }
}