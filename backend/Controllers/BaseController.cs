using Microsoft.AspNetCore.Mvc;
using SquadFile.Services;
using System.Security.Claims;

namespace SquadFile.Controllers
{
    /// <summary>
    /// 基础控制器，提供通用的API响应方法
    /// </summary>
    [ApiController]
    [Route("squadfile-api/[controller]")]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 本地化服务
        /// </summary>
        protected readonly LocalizationService? _localizationService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="localizationService">本地化服务</param>
        public BaseController(LocalizationService? localizationService)
        {
            _localizationService = localizationService;
        }

        /// <summary>
        ///  获取客户端请求的IP地址
        /// </summary>
        /// <returns></returns>
        protected string GetClientIP()
        {
            string ipAddress = string.Empty;
            if (string.IsNullOrEmpty(Request.Headers["X-Real-IP"]))
            {
                ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            else
            {
                ipAddress = Request.Headers["X-Real-IP"].ToString();
            }

            ipAddress = ipAddress?.Replace("::ffff:", "");

            return ipAddress;
        }

        /// <summary>
        /// 从JWT token中获取当前用户ID
        /// </summary>
        /// <returns>用户ID</returns>
        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return -1;
        }

        protected bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="key">资源键</param>
        /// <returns>本地化字符串</returns>
        protected string GetLocalizedText(string key)
        {
            return _localizationService?.GetLocalizedString(key) ?? key;
        }

        /// <summary>
        /// 返回成功的响应
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <param name="messageKey">消息键</param>
        /// <returns>成功响应</returns>
        protected IActionResult Success(object? data = null, string messageKey = "OperationSuccessful")
        {
            return Ok(new
            {
                code = 200,
                message = GetLocalizedText(messageKey),
                data = data
            });
        }

        /// <summary>
        /// 返回失败的响应
        /// </summary>
        /// <param name="messageKey">错误消息键</param>
        /// <param name="code">错误代码</param>
        /// <returns>失败响应</returns>
        protected IActionResult Fail(string messageKey = "OperationFailed", int code = 400)
        {
            return BadRequest(new
            {
                code = code,
                message = GetLocalizedText(messageKey)
            });
        }

        /// <summary>
        /// 返回未授权的响应
        /// </summary>
        /// <param name="messageKey">错误消息键</param>
        /// <returns>未授权响应</returns>
        protected IActionResult UnauthorizedResult(string messageKey = "UnauthorizedAccess")
        {
            return Unauthorized(new
            {
                code = 401,
                message = GetLocalizedText(messageKey)
            });
        }

        /// <summary>
        /// 返回未找到资源的响应
        /// </summary>
        /// <param name="messageKey">错误消息键</param>
        /// <returns>未找到响应</returns>
        protected IActionResult NotFoundResult(string messageKey = "ResourceNotFound")
        {
            return NotFound(new
            {
                code = 404,
                message = GetLocalizedText(messageKey)
            });
        }

        /// <summary>
        /// 返回服务器内部错误响应
        /// </summary>
        /// <param name="messageKey">错误消息键</param>
        /// <returns>服务器错误响应</returns>
        protected IActionResult InternalError(string messageKey = "InternalServerError")
        {
            return StatusCode(500, new
            {
                code = 500,
                message = GetLocalizedText(messageKey)
            });
        }
    }
}