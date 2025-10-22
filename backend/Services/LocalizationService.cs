using Microsoft.Extensions.Localization;
using SquadFile.Resources;
using System.Diagnostics;
using System.Globalization;

namespace SquadFile.Services
{
    /// <summary>
    /// 本地化服务
    /// </summary>
    public class LocalizationService
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public LocalizationService(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="key">资源键</param>
        /// <returns>本地化字符串</returns>
        public string GetLocalizedString(string key)
        {
            var localizedString = _localizer[key];
            return localizedString;
        }
    }
}