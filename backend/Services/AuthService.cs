using Microsoft.Extensions.Options;
using SquadFile.Data;
using SquadFile.Models;
using SquadFile.Models.ViewModel;
using SquadFile.Security;

namespace SquadFile.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly CaptchaService _captchaService;

        public AuthService(ApplicationDbContext context, IOptions<JwtHelper> options, CaptchaService captchaService)
        {
            _context = context;
            _jwtHelper = options.Value;
            _captchaService = captchaService;
        }

        public async Task<LoginResponse> AuthenticateAsync(LoginRequest request, string clientIp)
        {
            var response = new LoginResponse();
            // 验证验证码为必填项
            if (string.IsNullOrEmpty(request.Captcha))
            {
                response.ErrorKey = "CaptchaRequired";
                return response;
            }

            // 验证验证码ID为必填项
            if (string.IsNullOrEmpty(request.CaptchaId))
            {
                response.ErrorKey = "CaptchaIdRequired";
                return response;
            }

            // 验证验证码
            if (!_captchaService.ValidateCaptcha(request.CaptchaId, request.Captcha))
            {
                response.ErrorKey = "CaptchaInvalid";
                return response;
            }

            var user = _context.SysUsers.FirstOrDefault(u => u.Username == request.Username && u.DeletedTime == null);

            if (user == null)
            {
                response.ErrorKey = "InvalidCredentials";
                return response;
            }

            // Check if account is locked
            if (user.Status == UserStatus.Locked && user.LockTime.HasValue && user.LockTime.Value.AddMinutes(10) > DateTime.Now)
            {
                response.ErrorKey = "AccountLocked";
                return response;
            }

            // Verify password
            if (!PasswordHasherSHA256.VerifyPassword(request.Password, user.PasswordHash))
            {
                // Increment failed login count
                user.LoginFailCount++;
                if (user.LoginFailCount >= 3)
                {
                    user.Status = UserStatus.Locked;
                    user.LockTime = DateTime.Now;
                    await _context.SaveChangesAsync();
                    response.ErrorKey = "AccountLocked";
                    return response;
                }
                await _context.SaveChangesAsync();
                response.ErrorKey = "InvalidCredentials";
                return response;
            }

            // Reset failed login count on successful login
            user.LoginFailCount = 0;
            user.LockTime = null;
            user.Status = UserStatus.Active;
            user.LastLoginTime = DateTime.Now;
            user.LastLoginIp = clientIp;
            await _context.SaveChangesAsync();

            // Generate tokens
            var accessToken = _jwtHelper.GenerateToken(user.Id, user.Username, user.Role.ToString());

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = Guid.NewGuid().ToString(),
                ExpiresIn = 3600,
                User = new SysUserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Department = user.Department,
                    Role = user.Role.ToString(),
                    Avatar = user.Avatar,
                    IsFirstLogin = user.IsFirstLogin
                }
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.SysUsers.FindAsync(userId);
            if (user == null || user.DeletedTime.HasValue)
                return false;

            // Verify old password
            if (!PasswordHasherSHA256.VerifyPassword(oldPassword, user.PasswordHash))
                return false;

            // Update password
            user.PasswordHash = PasswordHasherSHA256.HashPassword(newPassword);
            user.IsFirstLogin = false;
            user.UpdatedTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}