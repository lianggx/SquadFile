namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 语言设置DTO
    /// </summary>
    public class LanguageSettingsViewModel
    {
        /// <summary>
        /// 默认语言
        /// </summary>
        public string DefaultLanguage { get; set; } = "zh-Hans";

        /// <summary>
        /// 支持的语言列表
        /// </summary>
        public List<SupportedLanguageDto> SupportedLanguages { get; set; } = new();
    }

    /// <summary>
    /// 支持的语言DTO
    /// </summary>
    public class SupportedLanguageDto
    {
        /// <summary>
        /// 语言代码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 语言名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}