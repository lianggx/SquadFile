using Microsoft.Extensions.Options;
using SquadFile.Models.ViewModel;
using SquadFile.Security;
using SquadFile.Services;

namespace SquadFile.Controllers
{
    /// <summary>
    /// 认证控制器，处理用户登录、登出、密码修改等操作
    /// </summary>
    [Authorize]
    public class AuthController : BaseController
    {
        private readonly AuthService _authService;
        private readonly LogService _logService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authService">认证服务</param>
        /// <param name="localizationService">本地化服务</param>
        /// <param name="logService">日志服务</param>
        public AuthController(AuthService authService, LocalizationService localizationService, LogService logService) : base(localizationService)
        {
            _authService = authService;
            _logService = logService;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request">登录请求参数</param>
        /// <returns>登录结果和访问令牌</returns>
        /// <response code="200">登录成功</response>
        /// <response code="401">用户名或密码错误</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var clientIP = GetClientIP();
            var userAgent = Request.Headers["User-Agent"].ToString();

            var response = await _authService.AuthenticateAsync(request, clientIP);
            if (response.ErrorKey != null)
            {
                // 记录登录失败日志
                await _logService.LogLoginAsync(0, request.Username ?? "Unknown", clientIP, userAgent, false, response.ErrorKey);

                return Fail(response.ErrorKey);
            }
            else
            {
                // 记录登录成功日志
                await _logService.LogLoginAsync(response.User.Id, response.User.Username, clientIP, userAgent, true);

                return Success(response);
            }
        }

        /// <summary>
        /// 刷新访问令牌
        /// </summary>
        /// <param name="request">刷新令牌请求参数</param>
        /// <returns>新的访问令牌和刷新令牌</returns>
        /// <response code="200">令牌刷新成功</response>
        /// <response code="400">请求参数无效</response>
        [HttpPost("refresh")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            // 验证刷新令牌
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return Fail("RefreshTokenRequired", 400);
            }

            // 在实际实现中，您需要验证刷新令牌的有效性
            // 这可能包括检查令牌是否存在于数据库中，是否已过期等
            // 为了简化，我们这里假设令牌有效

            // 生成新的访问令牌和刷新令牌
            // 注意：在实际应用中，您需要从存储中获取用户信息
            var newAccessToken = "new_access_token"; // 占位符，实际应生成JWT
            var newRefreshToken = Guid.NewGuid().ToString();

            var response = new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken,
                expiresIn = 3600 // 1小时
            };

            return Success(response, "RefreshTokenSuccessful");
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <returns>登出结果</returns>
        /// <response code="200">登出成功</response>
        [HttpPost("logout")]
        [ProducesResponseType(200)]
        public IActionResult Logout()
        {
            // 在实际实现中，您可能需要：
            // 1. 将令牌加入黑名单，防止重复使用
            // 2. 清除用户的会话信息（如果有的话）
            // 3. 记录登出日志

            // 由于项目不需要用户会话管理，我们只需返回成功响应
            return Success(null, "LogoutSuccessful");
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="request">修改密码请求参数</param>
        /// <returns>密码修改结果</returns>
        /// <response code="200">密码修改成功</response>
        /// <response code="400">请求参数无效</response>
        [HttpPut("password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // 验证请求数据
            if (string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                return Fail("OldAndPasswordRequired", 400);
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return Fail("PasswordsDoNotMatch", 400);
            }

            // 获取用户ID（在实际应用中从JWT令牌中获取）
            // 这里我们使用一个占位符ID
            var userId = 1; // 占位符，实际应从JWT令牌中解析

            // 调用服务更改密码
            var result = await _authService.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword);

            if (!result)
            {
                return Fail("PasswordChangeFailed");
            }

            return Success(null, "PasswordChangeSuccessful");
        }
    }
}