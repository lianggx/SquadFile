using Microsoft.AspNetCore.Mvc;
using SquadFile.Controllers;
using SquadFile.Models.ViewModel;
using SquadFile.Services;

namespace SquadFile.Controllers
{
    /// <summary>
    /// 系统控制器，处理系统设置相关操作
    /// </summary>

    [Authorize("admin")]
    public class SystemController : BaseController
    {
        private readonly SystemSettingsService _systemSettingsService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="systemSettingsService">系统设置服务</param>
        /// <param name="localizationService">本地化服务</param>
        public SystemController(SystemSettingsService systemSettingsService, LocalizationService localizationService) : base(localizationService)
        {
            _systemSettingsService = systemSettingsService;
        }

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <returns>系统配置信息</returns>
        /// <response code="200">获取成功</response>
        [HttpGet("config")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SystemConfigViewModel), 200)]
        public async Task<IActionResult> GetSystemConfig()
        {
            var config = await _systemSettingsService.GetSystemConfigAsync();
            return Success(config, "OperationSuccessful");
        }

        /// <summary>
        /// 获取系统语言设置
        /// </summary>
        /// <returns>语言设置信息</returns>
        /// <response code="200">获取成功</response>
        [HttpGet("language-settings")]
        [ProducesResponseType(typeof(LanguageSettingsViewModel), 200)]
        public async Task<IActionResult> GetLanguageSettings()
        {
            var settings = await _systemSettingsService.GetLanguageSettingsAsync();
            return Success(settings, "LanguageSettingsRetrieved");
        }

        /// <summary>
        /// 更新系统默认语言设置
        /// </summary>
        /// <param name="request">更新默认语言请求参数</param>
        /// <returns>更新结果</returns>
        /// <response code="200">更新成功</response>
        /// <response code="400">请求参数无效</response>
        [HttpPut("language-settings")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateDefaultLanguage([FromBody] UpdateDefaultLanguageRequest request)
        {
            if (string.IsNullOrEmpty(request.DefaultLanguage))
            {
                return Fail("DefaultLanguageRequired", 400);
            }

            var result = await _systemSettingsService.UpdateDefaultLanguageAsync(request);

            if (!result)
            {
                return Fail("LanguageSettingsUpdateFailed");
            }

            return Success(null, "LanguageSettingsUpdateSuccessful");
        }

        /// <summary>
        /// 更新用户语言偏好
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="request">更新用户语言偏好请求参数</param>
        /// <returns>更新结果</returns>
        /// <response code="200">更新成功</response>
        /// <response code="400">请求参数无效</response>
        /// <response code="404">用户不存在</response>
        [HttpPut("users/{userId}/language")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserLanguage(int userId, [FromBody] UpdateUserLanguageRequest request)
        {
            if (string.IsNullOrEmpty(request.PreferredLanguage))
            {
                return Fail("PreferredLanguageRequired", 400);
            }

            var result = await _systemSettingsService.UpdateUserLanguagePreferenceAsync(userId, request);

            if (!result)
            {
                return NotFoundResult("UserNotFound");
            }

            return Success(null, "PreferredLanguageUpdateSuccessful");
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="request">更新系统配置请求参数</param>
        /// <returns>更新结果</returns>
        /// <response code="200">更新成功</response>
        /// <response code="400">请求参数无效</response>
        [HttpPut("config")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateSystemConfig([FromBody] UpdateSystemConfigRequest request)
        {
            var result = await _systemSettingsService.UpdateSystemConfigAsync(request);

            if (!result)
            {
                return Fail("SystemConfigUpdateFailed");
            }

            return Success(null, "SystemConfigUpdateSuccessful");
        }

        /// <summary>
        /// 获取管理统计数据
        /// </summary>
        /// <returns>管理统计数据</returns>
        /// <response code="200">获取成功</response>
        [HttpGet("admin-stats")]
        [ProducesResponseType(typeof(AdminStatsViewModel), 200)]
        public async Task<IActionResult> GetAdminStats()
        {
            var stats = await _systemSettingsService.GetAdminStatsAsync();
            return Success(stats, "OperationSuccessful");
        }

        /// <summary>
        /// 上传Logo文件
        /// </summary>
        /// <param name="type">Logo类型 (login 或 home)</param>
        /// <returns>上传结果</returns>
        /// <response code="200">上传成功</response>
        /// <response code="400">请求参数无效</response>
        [HttpPost("logo")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadLogo([FromForm] string type)
        {
            // 验证Logo类型
            if (string.IsNullOrEmpty(type) || (type != "login" && type != "home"))
            {
                return Fail("InvalidLogoType", 400);
            }

            // 检查是否有上传文件
            var file = Request.Form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
            {
                return Fail("NoFileUploaded", 400);
            }

            // 验证文件类型（只允许图片）
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return Fail("InvalidFileType", 400);
            }

            // 验证文件大小（限制为5MB）
            if (file.Length > 5 * 1024 * 1024)
            {
                return Fail("FileSizeTooLarge", 400);
            }

            // 生成文件名
            var fileName = $"{type}-logo-{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // 确保上传目录存在
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 保存文件
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 更新系统配置
            var logoPath = $"/files/{fileName}";
            var result = await _systemSettingsService.UpdateLogoPathAsync(type, logoPath);

            if (!result)
            {
                // 如果更新数据库失败，删除已上传的文件
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return Fail("UpdateLogoPathFailed");
            }

            return Success(new { LogoPath = logoPath }, "LogoUploadSuccessful");
        }
    }
}