using SquadFile.Data;
using SquadFile.Models;
using Microsoft.EntityFrameworkCore;

namespace SquadFile.Services
{
    public class LogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 记录登录日志
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="username">用户名</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="userAgent">用户代理</param>
        /// <param name="loginResult">登录结果</param>
        /// <param name="failureReason">失败原因（如果登录失败）</param>
        public async Task LogLoginAsync(int userId, string username, string ipAddress, string userAgent, bool loginResult, string? failureReason = null)
        {
            var loginLog = new LoginLog
            {
                UserId = userId,
                Username = username,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LoginResult = loginResult,
                FailureReason = failureReason
            };

            _context.LoginLogs.Add(loginLog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 记录文件下载日志
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="username">用户名</param>
        /// <param name="fileId">文件ID</param>
        /// <param name="originalFileName">文件原始名称</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="userAgent">用户代理</param>
        public async Task LogDownloadAsync(int userId, string username, int fileId, string originalFileName, string filePath, long fileSize, string ipAddress, string userAgent)
        {
            // 确定设备类型
            string deviceType = "Unknown";
            if (!string.IsNullOrEmpty(userAgent))
            {
                if (userAgent.Contains("Windows"))
                    deviceType = "Windows";
                else if (userAgent.Contains("Mac"))
                    deviceType = "Mac";
                else if (userAgent.Contains("Linux"))
                    deviceType = "Linux";
                else if (userAgent.Contains("Android"))
                    deviceType = "Android";
                else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
                    deviceType = "iOS";
            }

            var downloadLog = new DownloadLog
            {
                UserId = userId,
                Username = username,
                FileId = fileId,
                OriginalFileName = originalFileName,
                FilePath = filePath,
                FileSize = fileSize,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                DeviceType = deviceType
            };

            _context.DownloadLogs.Add(downloadLog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 获取登录日志列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="searchQuery">搜索关键词</param>
        /// <returns>登录日志列表</returns>
        public async Task<(List<LoginLog> logs, int totalCount)> GetLoginLogsAsync(int page, int pageSize, string? searchQuery = null)
        {
            var query = _context.LoginLogs.AsQueryable();

            // 如果有搜索关键词，则添加搜索条件
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(log => 
                    log.Username.Contains(searchQuery) || 
                    log.IpAddress.Contains(searchQuery) ||
                    log.FailureReason.Contains(searchQuery));
            }

            // 获取总记录数
            var totalCount = await query.CountAsync();

            // 分页处理
            var logs = await query
                .OrderByDescending(log => log.LoginTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (logs, totalCount);
        }

        /// <summary>
        /// 获取下载日志列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="searchQuery">搜索关键词</param>
        /// <returns>下载日志列表</returns>
        public async Task<(List<DownloadLog> logs, int totalCount)> GetDownloadLogsAsync(int page, int pageSize, string? searchQuery = null)
        {
            var query = _context.DownloadLogs.AsQueryable();

            // 如果有搜索关键词，则添加搜索条件
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(log => 
                    log.Username.Contains(searchQuery) || 
                    log.OriginalFileName.Contains(searchQuery) ||
                    log.IpAddress.Contains(searchQuery) ||
                    log.DeviceType.Contains(searchQuery));
            }

            // 获取总记录数
            var totalCount = await query.CountAsync();

            // 分页处理
            var logs = await query
                .OrderByDescending(log => log.DownloadTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (logs, totalCount);
        }
    }
}