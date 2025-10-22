using Microsoft.AspNetCore.Mvc;
using SquadFile.Services;
using SquadFile.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SquadFile.Data;
using SquadFile.Models;

namespace SquadFile.Controllers
{
    /// <summary>
    /// 文件分享控制器
    /// </summary>
    [ApiController]
    [Route("squadfile-api/[controller]")]
    public class FileShareController : BaseController
    {
        private readonly FileManagementService _fileManagementService;
        private readonly ShareService _shareService;
        private readonly ApplicationDbContext _context;
        private readonly TemporaryDownloadTokenService _temporaryDownloadTokenService;
        private readonly LogService _logService;

        public FileShareController(
            FileManagementService fileManagementService,
            ShareService shareService,
            ApplicationDbContext context,
            LocalizationService? localizationService,
            TemporaryDownloadTokenService temporaryDownloadTokenService,
            LogService logService) : base(localizationService)
        {
            _fileManagementService = fileManagementService;
            _shareService = shareService;
            _context = context;
            _temporaryDownloadTokenService = temporaryDownloadTokenService;
            _logService = logService;
        }

        /// <summary>
        /// 通过短链接获取文件或文件夹内容
        /// </summary>
        /// <param name="shortCode">短链接代码</param>
        /// <returns>文件内容</returns>
        [HttpGet("{shortCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetItemByShortCode(string shortCode)
        {
            // 通过短链接代码找到对应的分享记录
            var shareRecord = await _shareService.GetShareRecordByShortCodeAsync(shortCode);

            if (shareRecord == null)
            {
                return NotFoundResult("InvalidShortCode");
            }

            // 检查是否过期
            if (shareRecord.ExpireTime.HasValue)
            {
                var dt1 = DateTime.Parse(shareRecord.ExpireTime.Value.ToShortDateString());
                var dt2 = DateTime.Parse(DateTime.Now.ToShortDateString());
                if (dt1 < dt2)
                {
                    return NotFoundResult("ShareExpired");
                }
            }

            // 根据项目类型处理不同的逻辑
            if (shareRecord.ItemType == ShareItemType.File)
            {
                // 获取文件信息
                var fileRecord = await _context.FileRecords.FirstOrDefaultAsync(f => f.Id == shareRecord.ItemId && !f.IsDeleted);
                if (fileRecord == null)
                {
                    return NotFoundResult("FileNotFound");
                }

                // 通过服务层获取文件路径和原始文件名
                var (filePath, originalFileName) = await _fileManagementService.GetFilePathAndNameAsync(fileRecord.Id);

                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                {
                    return NotFoundResult("FileNotFound");
                }

                // 记录下载日志
                var clientIP = GetClientIP();
                var userAgent = Request.Headers["User-Agent"].ToString();
                var currentUserId = GetCurrentUserId();
                var currentUser = await _context.SysUsers.FirstOrDefaultAsync(u => u.Id == currentUserId);
                var username = currentUser?.Username ?? "Anonymous";
                
                await _logService.LogDownloadAsync(
                    currentUserId, 
                    username, 
                    fileRecord.Id, 
                    fileRecord.OriginalName, 
                    filePath, 
                    fileRecord.Size, 
                    clientIP, 
                    userAgent);

                // 构建相对于路径
                var fileStoragePath = await _fileManagementService.GetStoragePathAsync();
                var fileRelativePath = filePath.Substring(fileStoragePath.Length + 1).Split('\\').Aggregate((a, b) => a + "/" + Uri.EscapeDataString(b));
                // 使用新的URL前缀返回重定向到静态文件
                return Redirect($"/files/{fileRelativePath}");
            }
            else if (shareRecord.ItemType == ShareItemType.Folder)
            {
                // 获取文件夹信息
                var folderRecord = await _context.FileFolders.FirstOrDefaultAsync(f => f.Id == shareRecord.ItemId && !f.IsDeleted);
                if (folderRecord == null)
                {
                    return NotFoundResult("FolderNotFound");
                }

                // 对于文件夹分享，返回文件夹信息而不是直接下载
                // 这里可以根据需要返回文件夹的元数据
                return Success(new
                {
                    Id = folderRecord.Id,
                    Name = folderRecord.Name,
                    Description = folderRecord.Description,
                    CreatedTime = folderRecord.CreatedTime
                });
            }

            return NotFoundResult("InvalidItemType");
        }

        /// <summary>
        /// 生成文件短链接
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns>短链接</returns>
        [HttpGet("generate/file/{fileId}")]
        [Authorize]
        public async Task<IActionResult> GenerateFileShortLink(int fileId)
        {
            // 创建一个空的分享请求来生成链接
            var request = new ShareFileRequest();
            var shareRecord = await _shareService.ShareFileAsync(fileId, request, GetCurrentUserId());

            var baseUrl = $"{Request.Scheme}://{Request.Host}/squadfile-api/fileshare";
            var shortLink = $"{baseUrl}/{shareRecord.ShortCode}";

            return Success(new { ShortLink = shortLink, ShortCode = shareRecord.ShortCode }, "ShortLinkGeneratedSuccessfully");
        }

        /// <summary>
        /// 生成文件夹短链接
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <returns>短链接</returns>
        [HttpGet("generate/folder/{folderId}")]
        [Authorize]
        public async Task<IActionResult> GenerateFolderShortLink(int folderId)
        {
            // 创建一个空的分享请求来生成链接
            var request = new ShareFileRequest();
            var shareRecord = await _shareService.ShareFolderAsync(folderId, request, GetCurrentUserId());

            var baseUrl = $"{Request.Scheme}://{Request.Host}/squadfile-api/fileshare";
            var shortLink = $"{baseUrl}/{shareRecord.ShortCode}";

            return Success(new { ShortLink = shortLink, ShortCode = shareRecord.ShortCode }, "ShortLinkGeneratedSuccessfully");
        }

        /// <summary>
        /// 分享文件
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="request">分享请求</param>
        /// <returns>分享结果</returns>
        [HttpPost("file/{fileId}")]
        [Authorize]
        public async Task<IActionResult> ShareFile(int fileId, [FromBody] ShareFileRequest request)
        {
            var shareRecord = await _shareService.ShareFileAsync(fileId, request, GetCurrentUserId());

            var baseUrl = $"{Request.Scheme}://{Request.Host}/squadfile-api/fileshare";
            var shortLink = $"{baseUrl}/{shareRecord.ShortCode}";

            return Success(new { ShortLink = shortLink, ShortCode = shareRecord.ShortCode }, "FileSharedSuccessfully");
        }

        /// <summary>
        /// 分享文件夹
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="request">分享请求</param>
        /// <returns>分享结果</returns>
        [HttpPost("folder/{folderId}")]
        [Authorize]
        public async Task<IActionResult> ShareFolder(int folderId, [FromBody] ShareFileRequest request)
        {
            var shareRecord = await _shareService.ShareFolderAsync(folderId, request, GetCurrentUserId());

            var baseUrl = $"{Request.Scheme}://{Request.Host}/squadfile-api/fileshare";
            var shortLink = $"{baseUrl}/{shareRecord.ShortCode}";

            return Success(new { ShortLink = shortLink, ShortCode = shareRecord.ShortCode }, "FolderSharedSuccessfully");
        }

        /// <summary>
        /// 验证分享密码
        /// </summary>
        /// <param name="shortCode">短链接代码</param>
        /// <param name="request">密码验证请求</param>
        /// <returns>验证结果</returns>
        [HttpPost("validate/{shortCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateSharePassword(string shortCode, [FromBody] ValidatePasswordRequest request)
        {
            var shareRecord = await _shareService.GetShareRecordByShortCodeAsync(shortCode);

            if (shareRecord == null)
            {
                return NotFoundResult("InvalidShortCode");
            }

            var isValid = _shareService.ValidateSharePassword(shareRecord, request.Password);

            if (isValid)
            {
                var token = await _temporaryDownloadTokenService.CreateTemporaryTokenAsync(shareRecord.ItemId, 0, 30);
                return Success(new { IsValid = true, token, fileId = shareRecord.ItemId }, "PasswordValid");
            }
            else
            {
                return UnauthorizedResult("InvalidPassword");
            }
        }

        /// <summary>
        /// 获取分享项目信息
        /// </summary>
        /// <param name="shortCode">短链接代码</param>
        /// <returns>分享项目信息</returns>
        [HttpGet("info/{shortCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShareItemInfo(string shortCode)
        {
            // 通过短链接代码找到对应的分享记录
            var shareRecord = await _shareService.GetShareRecordByShortCodeAsync(shortCode);

            if (shareRecord == null)
            {
                return NotFoundResult("InvalidShortCode");
            }

            // 检查是否过期
            if (shareRecord.ExpireTime.HasValue && shareRecord.ExpireTime.Value < DateTime.Now)
            {
                return NotFoundResult("ShareExpired");
            }

            // 根据项目类型处理不同的逻辑
            if (shareRecord.ItemType == ShareItemType.File)
            {
                // 获取文件信息
                var fileRecord = await _shareService.GetSharedFileAsync(shareRecord.ItemId);
                if (fileRecord == null)
                {
                    return NotFoundResult("FileNotFound");
                }

                // 返回文件信息
                return Success(new
                {
                    Id = fileRecord.Id,
                    Name = fileRecord.OriginalName,
                    Size = fileRecord.Size,
                    CreatedTime = fileRecord.UploadTime,
                    Extension = System.IO.Path.GetExtension(fileRecord.OriginalName),
                    HasPassword = !string.IsNullOrEmpty(shareRecord.Password),
                    ShortCode = shareRecord.ShortCode
                });
            }
            else if (shareRecord.ItemType == ShareItemType.Folder)
            {
                // 获取文件夹信息
                var folderRecord = await _shareService.GetSharedFolderAsync(shareRecord.ItemId);
                if (folderRecord == null)
                {
                    return NotFoundResult("FolderNotFound");
                }

                // 返回文件夹信息
                return Success(new
                {
                    Id = folderRecord.Id,
                    Name = folderRecord.Name,
                    Description = folderRecord.Description,
                    CreatedTime = folderRecord.CreatedTime,
                    HasPassword = !string.IsNullOrEmpty(shareRecord.Password),
                    ShortCode = shareRecord.ShortCode
                });
            }

            return NotFoundResult("InvalidItemType");
        }

        /// <summary>
        /// 获取短链接记录列表
        /// </summary>
        /// <returns>短链接记录列表</returns>
        [HttpGet("records")]
        [Authorize("admin")]
        public async Task<IActionResult> GetShareRecords([FromQuery] GetShareRecordsRequest request)
        {
            // 获取所有未删除的分享记录，支持搜索
            var query = _context.ShareRecords
                .Where(sr => !sr.IsDeleted);

            // 如果有搜索关键词，则添加搜索条件
            if (!string.IsNullOrEmpty(request.SearchQuery))
            {
                query = query.Where(sr =>
                    sr.ShortCode.Contains(request.SearchQuery) ||
                    (sr.ItemType == ShareItemType.File &&
                     _context.FileRecords.Any(fr => fr.Id == sr.ItemId && fr.OriginalName.Contains(request.SearchQuery))) ||
                    (sr.ItemType == ShareItemType.Folder &&
                     _context.FileFolders.Any(ff => ff.Id == sr.ItemId && ff.Name.Contains(request.SearchQuery)))
                );
            }

            // 获取总记录数
            var totalCount = await query.CountAsync();

            // 分页处理
            var shareRecords = await query
                .OrderByDescending(sr => sr.CreatedTime)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // 获取所有相关的文件记录
            var fileRecordIds = shareRecords
                .Where(sr => sr.ItemType == ShareItemType.File)
                .Select(sr => sr.ItemId)
                .Distinct()
                .ToList();

            var fileRecords = await _context.FileRecords
                .Where(fr => fileRecordIds.Contains(fr.Id) && !fr.IsDeleted)
                .ToDictionaryAsync(fr => fr.Id, fr => fr.OriginalName);

            // 获取所有相关的文件夹记录
            var folderRecordIds = shareRecords
                .Where(sr => sr.ItemType == ShareItemType.Folder)
                .Select(sr => sr.ItemId)
                .Distinct()
                .ToList();

            var folderRecords = await _context.FileFolders
                .Where(fr => folderRecordIds.Contains(fr.Id) && !fr.IsDeleted)
                .ToDictionaryAsync(fr => fr.Id, fr => fr.Name);

            // 获取所有用户信息，用于显示创建人名称
            var users = await _context.SysUsers
                .ToDictionaryAsync(u => u.Id, u => u.Username);

            // 构建返回数据
            var result = shareRecords.Select(record => new
            {
                id = record.Id,
                shortCode = record.ShortCode,
                itemId = record.ItemId,
                itemType = record.ItemType.ToString(),
                createTime = record.CreatedTime,
                createdBy = users.ContainsKey(record.CreatedBy) ? users[record.CreatedBy] : GetLocalizedText("UnknownUser"),
                expireTime = record.ExpireTime,
                originalName = record.ItemType == ShareItemType.File
                    ? (fileRecords.ContainsKey(record.ItemId) ? fileRecords[record.ItemId] : GetLocalizedText("UnknownFile"))
                    : (folderRecords.ContainsKey(record.ItemId) ? folderRecords[record.ItemId] : GetLocalizedText("UnknownFolder"))
            }).ToList();

            return Success(new
            {
                items = result,
                totalCount = totalCount,
                page = request.Page,
                pageSize = request.PageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            });
        }

        /// <summary>
        /// 批量删除短链接记录
        /// </summary>
        /// <param name="request">删除请求</param>
        /// <returns>删除结果</returns>
        [HttpDelete("records/batch")]
        [Authorize("admin")]
        public async Task<IActionResult> DeleteShareRecords([FromBody] DeleteShareRecordsRequest request)
        {
            if (request.Ids == null || !request.Ids.Any())
            {
                return Fail("PleaseSelectRecordsToDelete");
            }

            // 获取要删除的记录
            var records = await _context.ShareRecords
                .Where(sr => request.Ids.Contains(sr.Id) && !sr.IsDeleted)
                .ToListAsync();

            if (!records.Any())
            {
                return Fail("NoShareRecordsFound");
            }

            // 标记为已删除
            foreach (var record in records)
            {
                record.IsDeleted = true;
                record.DeletedTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Success(GetLocalizedText("ShareRecordsDeleted").Replace("{count}", records.Count.ToString()));
        }
    }

    /// <summary>
    /// 删除短链接记录请求
    /// </summary>
    public class DeleteShareRecordsRequest
    {
        /// <summary>
        /// 要删除的记录ID列表
        /// </summary>
        public List<int> Ids { get; set; } = new List<int>();
    }

    /// <summary>
    /// 获取短链接记录列表请求
    /// </summary>
    public class GetShareRecordsRequest
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string? SearchQuery { get; set; }

        /// <summary>
        /// 页码（从1开始）
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}