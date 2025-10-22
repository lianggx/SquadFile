using System.ComponentModel.DataAnnotations;

namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 更新用户语言偏好请求DTO
    /// </summary>
    public class UpdateUserLanguageRequest
    {
        /// <summary>
        /// 用户语言偏好
        /// </summary>
        [Required]
        public string PreferredLanguage { get; set; } = string.Empty;
    }
}