using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SquadFile.Middleware
{
    /// <summary>
    /// 全局异常处理中间件
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// 中间件处理方法
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>异步任务</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// 处理异常并返回统一格式的错误响应
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <param name="exception">异常对象</param>
        /// <returns>异步任务</returns>
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            // 根据异常类型设置状态码
            response.StatusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            // 创建错误响应模型
            var errorResponse = new
            {
                code = response.StatusCode,
                message = GetErrorMessage(exception),
                timestamp = DateTime.Now,
                path = context.Request.Path
            };

            // 序列化并返回错误响应
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(jsonResponse);
        }

        /// <summary>
        /// 获取错误消息
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <returns>错误消息</returns>
        private static string GetErrorMessage(Exception exception)
        {
            string message = exception.Message;
#if DEBUG
            message += $"\r\n{exception.StackTrace}";
#endif
            return exception switch
            {
                UnauthorizedAccessException => "UnauthorizedAccess",
                KeyNotFoundException => exception.Message,
                _ => message
            };
        }
    }
}