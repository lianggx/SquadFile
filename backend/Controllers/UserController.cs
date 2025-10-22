using Microsoft.AspNetCore.Mvc;
using SquadFile.Models.ViewModel;
using SquadFile.Services;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SquadFile.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// </summary>
    [ApiController]
    [Route("squadfile-api/[controller]")]
    [Authorize("admin")]
    public class UserController : BaseController
    {
        private readonly UserService _userService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userService">用户服务</param>
        /// <param name="localizationService">本地化服务</param>
        public UserController(UserService userService, LocalizationService localizationService) : base(localizationService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        /// <response code="200">获取成功</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var userDtos = users.Select(u => new SysUserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Department = u.Department,
                Role = u.Role,
                Avatar = u.Avatar,
                IsFirstLogin = u.IsFirstLogin
            }).ToList();

            return Success(userDtos, "UsersRetrievedSuccessfully");
        }

        /// <summary>
        /// 创建新用户
        /// </summary>
        /// <param name="request">创建用户请求参数</param>
        /// <returns>创建结果</returns>
        /// <response code="200">用户创建成功</response>
        /// <response code="400">用户名已存在</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var user = await _userService.CreateUserAsync(request);

            if (user == null)
            {
                return Fail("UsernameAlreadyExists");
            }

            return Success(null, "UserCreationSuccessful");
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="request">更新用户请求参数</param>
        /// <returns>更新结果</returns>
        /// <response code="200">用户更新成功</response>
        /// <response code="404">用户不存在</response>
        [HttpPut("{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequest request)
        {
            var result = await _userService.UpdateUserAsync(userId, request);

            if (!result)
            {
                return NotFoundResult("UserNotFound");
            }

            return Success(null, "UserUpdateSuccessful");
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>删除结果</returns>
        /// <response code="200">用户删除成功</response>
        /// <response code="404">用户不存在</response>
        [HttpDelete("{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _userService.DeleteUserAsync(userId);

            if (!result)
            {
                return NotFoundResult("UserNotFound");
            }

            return Success(null, "UserDeletionSuccessful");
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="request">重置密码请求参数</param>
        /// <returns>密码重置结果</returns>
        /// <response code="200">密码重置成功</response>
        /// <response code="404">用户不存在</response>
        [HttpPost("{userId}/reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResetPassword(int userId, [FromBody] ResetPasswordRequest request)
        {
            var result = await _userService.ResetUserPasswordAsync(userId, request.NewPassword);

            if (!result)
            {
                return NotFoundResult("UserNotFound");
            }

            return Success(null, "PasswordResetSuccessful");
        }

        /// <summary>
        /// 修改当前用户密码
        /// </summary>
        /// <param name="request">修改密码请求参数</param>
        /// <returns>密码修改结果</returns>
        /// <response code="200">密码修改成功</response>
        /// <response code="400">旧密码错误</response>
        /// <response code="404">用户不存在</response>
        [HttpPut("change-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // 从JWT token中获取当前用户ID
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _userService.ChangeUserPasswordAsync(userId, request.OldPassword, request.NewPassword);

            if (!result)
            {
                // 检查用户是否存在
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFoundResult("UserNotFound");
                }
                return Fail("OldPasswordIncorrect");
            }

            return Success(null, "PasswordChangeSuccessful");
        }

        /// <summary>
        /// 更新当前用户资料
        /// </summary>
        /// <param name="request">更新资料请求参数</param>
        /// <returns>资料更新结果</returns>
        /// <response code="200">资料更新成功</response>
        /// <response code="404">用户不存在</response>
        [HttpPut("profile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            // 从JWT token中获取当前用户ID
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _userService.UpdateUserProfileAsync(userId, request);

            if (!result)
            {
                return NotFoundResult("UserNotFound");
            }

            // 更新成功后，返回更新后的用户信息
            var user = await _userService.GetUserByIdAsync(userId);
            return Success(user, "ProfileUpdateSuccessful");
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        /// <response code="200">获取成功</response>
        /// <response code="404">用户不存在</response>
        [HttpGet("profile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProfile()
        {
            // 从JWT token中获取当前用户ID
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFoundResult("UserNotFound");
            }

            return Success(user, "ProfileRetrievedSuccessfully");
        }
    }
}