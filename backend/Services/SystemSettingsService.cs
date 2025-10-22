using SquadFile.Data;
using SquadFile.Models;
using SquadFile.Models.ViewModel;

namespace SquadFile.Services
{
    public class SystemSettingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public SystemSettingsService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LanguageSettingsViewModel> GetLanguageSettingsAsync()
        {
            var settings = GetSystemConfig();

            return new LanguageSettingsViewModel
            {
                DefaultLanguage = settings.DefaultLanguage,
                SupportedLanguages = new List<SupportedLanguageDto>
                {
                    new SupportedLanguageDto { Code = "zh-Hans", Name = "中文" },
                    new SupportedLanguageDto { Code = "en-US", Name = "English" }
                }
            };
        }

        public SystemSettings GetSystemConfig()
        {
            var settings = _context.SystemSettings.FirstOrDefault();
            if (settings == null)
            {
                // 如果没有系统设置，创建默认设置
                settings = new SystemSettings
                {
                    DefaultLanguage = "zh-Hans",
                    SiteName = "小队快传",
                    LoginLogoPath = "/static/images/logo.png",
                    HomeLogoPath = "/static/images/logo.png",
                    MaxFileSize = 100,
                    StorageLimit = 10240,
                    FileStoragePath = _configuration["Uploads"]
                };
                _context.SystemSettings.Add(settings);
                _context.SaveChanges();
            }

            return settings;
        }

        public async Task<SystemConfigViewModel> GetSystemConfigAsync()
        {
            var settings = GetSystemConfig();

            return new SystemConfigViewModel
            {
                SiteName = settings.SiteName,
                LoginLogoPath = settings.LoginLogoPath,
                HomeLogoPath = settings.HomeLogoPath,
                MaxFileSize = settings.MaxFileSize,
                StorageLimit = settings.StorageLimit,
                FileStoragePath = settings.FileStoragePath,
                DefaultLanguage = settings.DefaultLanguage
            };
        }

        public async Task<bool> UpdateSystemConfigAsync(UpdateSystemConfigRequest request)
        {
            var settings = GetSystemConfig();

            // 更新设置
            settings.SiteName = request.SiteName ?? settings.SiteName;
            settings.LoginLogoPath = request.LoginLogoPath ?? settings.LoginLogoPath;
            settings.HomeLogoPath = request.HomeLogoPath ?? settings.HomeLogoPath;
            settings.MaxFileSize = request.MaxFileSize;
            settings.StorageLimit = request.StorageLimit;

            // 固定使用 uploads 作为文件存储路径
            settings.FileStoragePath = _configuration["Uploads"];
            settings.UpdatedTime = DateTime.Now;

            // 确保存储目录存在
            if (!string.IsNullOrEmpty(settings.FileStoragePath))
            {
                // 获取应用程序根目录
                var appRoot = Directory.GetCurrentDirectory();
                // 构建完整的存储路径
                var fullPath = Path.Combine(appRoot, settings.FileStoragePath);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
            }

            _context.SystemSettings.Update(settings);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateDefaultLanguageAsync(UpdateDefaultLanguageRequest request)
        {
            var settings = GetSystemConfig();
            settings.DefaultLanguage = request.DefaultLanguage;
            settings.UpdatedTime = DateTime.Now;

            _context.SystemSettings.Update(settings);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserLanguagePreferenceAsync(int userId, UpdateUserLanguageRequest request)
        {
            var user = await _context.SysUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }

            user.PreferredLanguage = request.PreferredLanguage;
            user.UpdatedTime = DateTime.Now;

            _context.SysUsers.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 获取管理统计数据
        /// </summary>
        /// <returns>管理统计数据</returns>
        public async Task<AdminStatsViewModel> GetAdminStatsAsync()
        {
            var totalUsers = await _context.SysUsers.CountAsync();
            var totalFolders = await _context.FileFolders.CountAsync(f => !f.IsDeleted);
            var totalFiles = await _context.FileRecords.CountAsync(f => !f.IsDeleted);
            var totalDownloads = await _context.DownloadLogs.CountAsync();
            var totalStorageUsed = await _context.FileRecords.Where(f => !f.IsDeleted).SumAsync(f => (long)f.Size);

            // 获取最新上传的10个文件
            var latestFiles = await _context.FileRecords
                .Where(f => !f.IsDeleted)
                .OrderByDescending(f => f.UploadTime)
                .Take(10)
                .Select(f => new LatestFileDto
                {
                    Id = f.Id,
                    Name = f.OriginalName,
                    Size = f.Size,
                    UploadTime = f.UploadTime,
                    UploadedByUserId = f.UploadedBy
                })
                .ToListAsync();

            // 获取上传者用户名
            var userIds = latestFiles.Select(f => f.UploadedByUserId).Distinct().ToList();
            var userNames = await _context.SysUsers
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Username);

            foreach (var file in latestFiles)
            {
                if (userNames.ContainsKey(file.UploadedByUserId))
                {
                    file.UploadedBy = userNames[file.UploadedByUserId];
                }
            }

            // 获取文件类型分布
            var fileTypeDistribution = await _context.FileRecords
                .Where(f => !f.IsDeleted)
                .GroupBy(f => f.Extension.ToLower())
                .Select(g => new FileTypeDistributionDto
                {
                    Extension = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // 获取分享短链接统计
            var totalShareLinks = await _context.ShareRecords.CountAsync(s => !s.IsDeleted);
            var activeShareLinks = await _context.ShareRecords.CountAsync(s => !s.IsDeleted && (!s.ExpireTime.HasValue || s.ExpireTime > DateTime.Now));

            // 获取下载统计（文件被下载次数）
            var downloadStats = await _context.DownloadLogs
                .GroupBy(dl => new { dl.FileId, dl.OriginalFileName })
                .Select(g => new DownloadStatDto
                {
                    FileId = g.Key.FileId,
                    FileName = g.Key.OriginalFileName,
                    DownloadCount = g.Count()
                })
                .OrderByDescending(ds => ds.DownloadCount)
                .Take(10)
                .ToListAsync();

            // 将字节转换为MB
            var storageUsedInMB = totalStorageUsed / (1024 * 1024);

            return new AdminStatsViewModel
            {
                TotalUsers = totalUsers,
                TotalFolders = totalFolders,
                TotalFiles = totalFiles,
                TotalDownloads = totalDownloads,
                StorageUsed = totalStorageUsed,
                StorageUsedDisplay = $"{storageUsedInMB} MB",
                LatestFiles = latestFiles,
                FileTypeDistribution = fileTypeDistribution,
                TotalShareLinks = totalShareLinks,
                ActiveShareLinks = activeShareLinks,
                DownloadStats = downloadStats
            };
        }

        /// <summary>
        /// 更新Logo路径
        /// </summary>
        /// <param name="type">Logo类型 (login 或 home)</param>
        /// <param name="logoPath">Logo路径</param>
        /// <returns>更新结果</returns>
        public async Task<bool> UpdateLogoPathAsync(string type, string logoPath)
        {
            var settings = GetSystemConfig();

            // 根据类型更新对应的Logo路径
            if (type == "login")
            {
                settings.LoginLogoPath = logoPath;
            }
            else if (type == "home")
            {
                settings.HomeLogoPath = logoPath;
            }
            else
            {
                return false; // 无效的Logo类型
            }

            settings.UpdatedTime = DateTime.Now;

            _context.SystemSettings.Update(settings);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}