using Microsoft.AspNetCore.Builder;
using SquadFile.Middleware;

namespace SquadFile.Extensions
{
    /// <summary>
    /// 中间件扩展方法
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// 使用全局异常处理中间件
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <returns>应用构建器</returns>
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }

        /// <summary>
        /// 使用请求日志中间件
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <returns>应用构建器</returns>
        public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}