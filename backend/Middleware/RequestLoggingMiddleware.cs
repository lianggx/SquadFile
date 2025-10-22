using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SquadFile.Middleware
{
    /// <summary>
    /// 请求日志中间件
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
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
            // 记录请求信息
            var request = await FormatRequest(context.Request);
            _logger.LogInformation("Incoming Request: {Method} {Url} {QueryString} {Request}", 
                context.Request.Method, 
                context.Request.Path, 
                context.Request.QueryString, 
                request);

            // 替换响应体流以捕获响应
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                // 继续处理请求
                await _next(context);

                // 记录响应信息
                var response = await FormatResponse(context.Response);
                _logger.LogInformation("Outgoing Response: {StatusCode} {Response}", 
                    context.Response.StatusCode, 
                    response);

                // 将响应写回原始流
                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        /// <summary>
        /// 格式化请求信息
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <returns>格式化后的请求信息</returns>
        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = string.Empty;

            // 确保可以多次读取请求体
            request.EnableBuffering();

            // 读取请求体
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            body = await reader.ReadToEndAsync();
            
            // 重置请求体流位置
            request.Body.Position = 0;

            return body.Length > 0 ? body : string.Empty;
        }

        /// <summary>
        /// 格式化响应信息
        /// </summary>
        /// <param name="response">HTTP响应</param>
        /// <returns>格式化后的响应信息</returns>
        private async Task<string> FormatResponse(HttpResponse response)
        {
            // 重置响应体流位置
            response.Body.Position = 0;

            // 读取响应体
            using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            // 重置响应体流位置
            response.Body.Position = 0;

            return body.Length > 0 ? body : string.Empty;
        }
    }
}