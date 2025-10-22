using Microsoft.EntityFrameworkCore;
using SquadFile.Data;
using SquadFile.Models;
using SquadFile.Models.ViewModel;
using SquadFile.Security;

namespace SquadFile.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SysUser?> CreateUserAsync(CreateUserRequest request)
        {
            // Check if username already exists
            if (await _context.SysUsers.AnyAsync(u => u.Username == request.Username && u.DeletedTime == null))
                return null;

            var user = new SysUser
            {
                Username = request.Username,
                Email = request.Email,
                DisplayName = request.DisplayName,
                Department = request.Department,
                Role = request.Role,
                PasswordHash = PasswordHasherSHA256.HashPassword(request.Password),
                IsFirstLogin = true,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now
            };

            _context.SysUsers.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> ResetUserPasswordAsync(int userId, string newPassword)
        {
            var user = await _context.SysUsers.FindAsync(userId);
            if (user == null || user.DeletedTime.HasValue)
                return false;

            user.PasswordHash = PasswordHasherSHA256.HashPassword(newPassword);
            user.IsFirstLogin = true;
            user.UpdatedTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.SysUsers.FindAsync(userId);
            if (user == null || user.DeletedTime.HasValue)
                return false;

            // 验证旧密码
            if (!PasswordHasherSHA256.VerifyPassword(oldPassword, user.PasswordHash))
                return false;

            // 更新密码
            user.PasswordHash = PasswordHasherSHA256.HashPassword(newPassword);
            user.UpdatedTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _context.SysUsers.FindAsync(userId);
            if (user == null || user.DeletedTime.HasValue)
                return false;

            // 更新用户资料
            user.Email = request.Email ?? user.Email;
            user.DisplayName = request.DisplayName ?? user.DisplayName;
            user.Department = request.Department ?? user.Department;
            user.UpdatedTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<SysUser?> GetUserByIdAsync(int userId)
        {
            return await _context.SysUsers.FindAsync(userId);
        }

        public async Task<List<SysUser>> GetAllUsersAsync()
        {
            return await _context.SysUsers
                .Where(u => u.DeletedTime == null)
                .OrderByDescending(u => u.CreatedTime)
                .ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            var user = await _context.SysUsers.FindAsync(userId);
            if (user == null || user.DeletedTime.HasValue)
                return false;

            // 更新用户信息
            user.Username = request.Username ?? user.Username;
            user.Email = request.Email ?? user.Email;
            user.DisplayName = request.DisplayName ?? user.DisplayName;
            user.Department = request.Department ?? user.Department;
            user.Role = request.Role ?? user.Role;
            user.Status = request.Status;
            user.UpdatedTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.SysUsers.FindAsync(userId);
            if (user == null || user.DeletedTime.HasValue)
                return false;

            // 软删除用户
            user.DeletedTime = DateTime.Now;
            user.UpdatedTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}