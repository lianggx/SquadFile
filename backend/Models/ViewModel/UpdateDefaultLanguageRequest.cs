using System.ComponentModel.DataAnnotations;

namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 更新默认语言请求DTO
    /// </summary>
    public class UpdateDefaultLanguageRequest
    {
        /// <summary>
        /// 默认语言代码
        /// </summary>
        [Required]
        public string DefaultLanguage { get; set; } = string.Empty;
    }
}