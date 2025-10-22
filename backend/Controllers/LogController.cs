using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SquadFile.Services;
using SquadFile.Models.ViewModel;

namespace SquadFile.Controllers
{
    /// <summary>
    /// 日志控制器，处理系统日志相关操作
    /// </summary>
    [ApiController]
    [Route("squadfile-api/[controller]")]
    [Authorize("admin")]
    public class LogController : BaseController
    {
        private readonly LogService _logService;

        public LogController(LogService logService, LocalizationService? localizationService) : base(localizationService)
        {
            _logService = logService;
        }

        /// <summary>
        /// 获取登录日志列表
        /// </summary>
        /// <param name="request">获取登录日志请求</param>
        /// <returns>登录日志列表</returns>
        [HttpGet("login")]
        public async Task<IActionResult> GetLoginLogs([FromQuery] GetLogsRequest request)
        {
            var (logs, totalCount) = await _logService.GetLoginLogsAsync(request.Page, request.PageSize, request.SearchQuery);

            var result = logs.Select(log => new
            {
                id = log.Id,
                userId = log.UserId,
                username = log.Username,
                loginTime = log.LoginTime,
                ipAddress = log.IpAddress,
                userAgent = log.UserAgent,
                loginResult = log.LoginResult,
                failureReason = log.FailureReason
            }).ToList();

            return Success(new
            {
                items = result,
                totalCount = totalCount,
                page = request.Page,
                pageSize = request.PageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            });
        }

        /// <summary>
        /// 获取下载日志列表
        /// </summary>
        /// <param name="request">获取下载日志请求</param>
        /// <returns>下载日志列表</returns>
        [HttpGet("download")]
        public async Task<IActionResult> GetDownloadLogs([FromQuery] GetLogsRequest request)
        {
            var (logs, totalCount) = await _logService.GetDownloadLogsAsync(request.Page, request.PageSize, request.SearchQuery);

            var result = logs.Select(log => new
            {
                id = log.Id,
                userId = log.UserId,
                username = log.Username,
                downloadTime = log.DownloadTime,
                fileId = log.FileId,
                originalFileName = log.OriginalFileName,
                filePath = log.FilePath,
                fileSize = log.FileSize,
                ipAddress = log.IpAddress,
                deviceType = log.DeviceType,
                userAgent = log.UserAgent
            }).ToList();

            return Success(new
            {
                items = result,
                totalCount = totalCount,
                page = request.Page,
                pageSize = request.PageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            });
        }
    }

    /// <summary>
    /// 获取日志请求
    /// </summary>
    public class GetLogsRequest
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string? SearchQuery { get; set; }

        /// <summary>
        /// 页码（从1开始）
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}