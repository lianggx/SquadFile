using Microsoft.EntityFrameworkCore;
using SquadFile.Data;
using SquadFile.Models;
using SquadFile.Models.ViewModel;
using System.Security.Cryptography;
using System.Text;

namespace SquadFile.Services
{
    /// <summary>
    /// 分享服务
    /// </summary>
    public class ShareService
    {
        private readonly ApplicationDbContext _context;
        private readonly FileManagementService _fileManagementService;

        public ShareService(ApplicationDbContext context, FileManagementService fileManagementService)
        {
            _context = context;
            _fileManagementService = fileManagementService;
        }

        /// <summary>
        /// 创建文件分享
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="request">分享请求</param>
        /// <param name="userId">用户ID</param>
        /// <returns>分享记录</returns>
        public async Task<ShareRecord> ShareFileAsync(int fileId, ShareFileRequest request, int userId)
        {
            // 检查文件是否存在
            var file = await _context.FileRecords.FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted);
            if (file == null)
                throw new ArgumentException("文件不存在");

            // 检查用户是否有权限分享该文件
            if (!await _fileManagementService.HasFolderPermissionAsync(file.FolderId, userId, true))
            {
                throw new UnauthorizedAccessException("没有分享该文件的权限");
            }

            // 生成唯一的短链接代码
            var shortCode = await GenerateUniqueShortCodeAsync();

            // 创建分享记录
            var shareRecord = new ShareRecord
            {
                ItemId = fileId,
                ItemType = ShareItemType.File,
                ShortCode = shortCode,
                Password = string.IsNullOrEmpty(request.Password) ? null : HashPassword(request.Password),
                ExpireTime = request.ExpireTime,
                CreatedTime = DateTime.Now,
                CreatedBy = userId  // 添加创建人字段
            };

            _context.ShareRecords.Add(shareRecord);
            await _context.SaveChangesAsync();

            return shareRecord;
        }

        /// <summary>
        /// 创建文件夹分享
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="request">分享请求</param>
        /// <param name="userId">用户ID</param>
        /// <returns>分享记录</returns>
        public async Task<ShareRecord> ShareFolderAsync(int folderId, ShareFileRequest request, int userId)
        {
            // 检查文件夹是否存在
            var folder = await _context.FileFolders.FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted);
            if (folder == null)
                throw new ArgumentException("文件夹不存在");

            // 检查用户是否有权限分享该文件夹
            if (!await _fileManagementService.HasFolderPermissionAsync(folderId, userId, true))
            {
                throw new UnauthorizedAccessException("没有分享该文件夹的权限");
            }

            // 生成唯一的短链接代码
            var shortCode = await GenerateUniqueShortCodeAsync();

            // 创建分享记录
            var shareRecord = new ShareRecord
            {
                ItemId = folderId,
                ItemType = ShareItemType.Folder,
                ShortCode = shortCode,
                Password = string.IsNullOrEmpty(request.Password) ? null : HashPassword(request.Password),
                ExpireTime = request.ExpireTime,
                CreatedTime = DateTime.Now,
                CreatedBy = userId  // 添加创建人字段
            };

            _context.ShareRecords.Add(shareRecord);
            await _context.SaveChangesAsync();

            return shareRecord;
        }

        /// <summary>
        /// 获取分享记录
        /// </summary>
        /// <param name="shortCode">短链接代码</param>
        /// <returns>分享记录</returns>
        public async Task<ShareRecord?> GetShareRecordByShortCodeAsync(string shortCode)
        {
            return await _context.ShareRecords
                .FirstOrDefaultAsync(s => s.ShortCode == shortCode && !s.IsDeleted);
        }

        /// <summary>
        /// 获取分享的文件信息
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns>文件信息</returns>
        public async Task<FileRecord?> GetSharedFileAsync(int fileId)
        {
            return await _context.FileRecords
                .FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted);
        }

        /// <summary>
        /// 获取分享的文件夹信息
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <returns>文件夹信息</returns>
        public async Task<FileFolder?> GetSharedFolderAsync(int folderId)
        {
            return await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted);
        }

        /// <summary>
        /// 验证分享密码
        /// </summary>
        /// <param name="shareRecord">分享记录</param>
        /// <param name="password">密码</param>
        /// <returns>是否正确</returns>
        public bool ValidateSharePassword(ShareRecord shareRecord, string password)
        {
            if (string.IsNullOrEmpty(shareRecord.Password))
                return true;

            // 检查是否过期
            if (shareRecord.ExpireTime.HasValue && shareRecord.ExpireTime.Value < DateTime.Now)
                return false;

            return HashPassword(password) == shareRecord.Password;
        }

        /// <summary>
        /// 生成唯一的短链接代码
        /// </summary>
        /// <returns>短链接代码</returns>
        private async Task<string> GenerateUniqueShortCodeAsync()
        {
            string shortCode;
            bool isUnique = false;
            
            do
            {
                // 生成随机短链接代码
                var bytes = new byte[6];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }
                
                shortCode = Convert.ToBase64String(bytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Substring(0, 8);

                // 检查是否唯一
                var existing = await _context.ShareRecords
                    .AnyAsync(s => s.ShortCode == shortCode);
                
                isUnique = !existing;
            } while (!isUnique);

            return shortCode;
        }

        /// <summary>
        /// 哈希密码
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>哈希后的密码</returns>
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}