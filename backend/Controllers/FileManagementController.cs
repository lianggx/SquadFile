using Microsoft.AspNetCore.Mvc;
using SquadFile.Services;
using SquadFile.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SquadFile.Data;
using SquadFile.Models;
using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt;
using SquadFile.Security;

namespace SquadFile.Controllers
{
    /// <summary>
    /// 文件管理控制器
    /// </summary>
    [ApiController]
    [Route("squadfile-api/[controller]")]
    [Authorize]
    public class FileManagementController : BaseController
    {
        private readonly FileManagementService _fileManagementService;
        private readonly TemporaryDownloadTokenService _temporaryDownloadTokenService;
        private readonly LogService _logService;

        public FileManagementController(FileManagementService fileManagementService, LocalizationService? localizationService, TemporaryDownloadTokenService temporaryDownloadTokenService, LogService logService) : base(localizationService)
        {
            _fileManagementService = fileManagementService;
            _temporaryDownloadTokenService = temporaryDownloadTokenService;
            _logService = logService;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="request">创建文件夹请求</param>
        /// <returns>创建的文件夹</returns>
        [HttpPost("folders")]
        public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var folder = await _fileManagementService.CreateFolderAsync(request, userId);
            return Success(folder, "FolderCreatedSuccessfully");
        }

        /// <summary>
        /// 获取用户有权限的文件夹列表
        /// </summary>
        /// <returns>文件夹列表</returns>
        [HttpGet("folders")]
        public async Task<IActionResult> GetUserFolders()
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var folders = await _fileManagementService.GetUserFoldersAsync(userId);
            return Success(folders, "FoldersRetrievedSuccessfully");
        }

        /// <summary>
        /// 获取文件夹中的文件列表
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <returns>文件列表</returns>
        [HttpGet("folders/{folderId}/files")]
        public async Task<IActionResult> GetFilesInFolder(int folderId)
        {
            var userId = GetCurrentUserId();

            if (userId <= 0)
            {
                return Success(); // 返回空列表
            }

            if (!IsAdmin())
            {
                var hasPermission = await _fileManagementService.HasFolderPermissionAsync(folderId, userId);
                if (!hasPermission)
                {
                    var folder = await _fileManagementService.GetFolderByIdAsync(folderId);
                    if (folder == null || !folder.IsPublic)
                    {
                        return Success(); // 返回空列表
                    }
                }
            }

            var files = await _fileManagementService.GetFilesInFolderAsync(folderId);
            return Success(files, "FilesRetrievedSuccessfully");
        }

        /// <summary>
        /// 授权用户访问文件夹
        /// </summary>
        /// <param name="request">授权请求</param>
        /// <returns>授权结果</returns>
        [HttpPost("permissions")]
        public async Task<IActionResult> GrantFolderPermission([FromBody] GrantPermissionRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _fileManagementService.GrantFolderPermissionAsync(request, userId);
            if (result)
            {
                return Success(null, "PermissionGrantedSuccessfully");
            }
            else
            {
                return Fail("PermissionGrantFailed");
            }
        }

        /// <summary>
        /// 批量授权用户访问文件夹
        /// </summary>
        /// <param name="requests">授权请求列表</param>
        /// <returns>授权结果</returns>
        [HttpPost("permissions/batch")]
        public async Task<IActionResult> GrantFolderPermissionsBatch([FromBody] List<GrantPermissionRequest> requests)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _fileManagementService.GrantFolderPermissionsBatchAsync(requests, userId);
            if (result)
            {
                return Success(null, "PermissionsGrantedSuccessfully");
            }
            else
            {
                return Fail("PermissionsGrantFailed");
            }
        }

        /// <summary>
        /// 撤销用户对文件夹的访问权限
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>撤销结果</returns>
        [HttpDelete("permissions/folders/{folderId}/users/{userId}")]
        public async Task<IActionResult> RevokeFolderPermission(int folderId, int userId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _fileManagementService.RevokeFolderPermissionAsync(folderId, userId);
            if (result)
            {
                return Success(null, "PermissionRevokedSuccessfully");
            }
            else
            {
                return Fail("PermissionRevokeFailed");
            }
        }

        /// <summary>
        /// 批量撤销用户对文件夹的访问权限
        /// </summary>
        /// <param name="requests">撤销权限请求列表</param>
        /// <returns>撤销结果</returns>
        [HttpDelete("permissions/batch")]
        public async Task<IActionResult> RevokeFolderPermissionsBatch([FromBody] List<RevokePermissionRequest> requests)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _fileManagementService.RevokeFolderPermissionsBatchAsync(requests, userId);
            if (result)
            {
                return Success(null, "PermissionsRevokedSuccessfully");
            }
            else
            {
                return Fail("PermissionsRevokeFailed");
            }
        }

        /// <summary>
        /// 准备文件上传
        /// </summary>
        /// <param name="request">上传请求</param>
        /// <returns>文件上传信息</returns>
        [HttpPost("upload/prepare")]
        public async Task<IActionResult> PrepareFileUpload([FromBody] UploadFileRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var fileRecord = await _fileManagementService.PrepareFileUploadAsync(request, userId);
            return Success(new
            {
                FileId = fileRecord.Id,
                StorageName = fileRecord.StorageName
            }, "UploadPreparedSuccessfully");
        }

        /// <summary>
        /// 上传文件块
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="chunkIndex">块索引</param>
        /// <param name="totalChunks">总块数</param>
        /// <param name="file">文件块</param>
        /// <returns>上传结果</returns>
        [HttpPost("upload/chunk/{fileId}")]
        public async Task<IActionResult> UploadFileChunk(int fileId, [FromForm] int chunkIndex, [FromForm] int totalChunks, [FromForm] IFormFile file)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            // 读取文件块数据
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var chunkBytes = memoryStream.ToArray();

            // 保存块到临时文件
            var tempDir = Path.Combine(Path.GetTempPath(), "SquadFileUploads");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            var chunkPath = Path.Combine(tempDir, $"{fileId}_chunk_{chunkIndex}");
            await System.IO.File.WriteAllBytesAsync(chunkPath, chunkBytes);

            // 如果是最后一个块，合并所有块
            if (chunkIndex == totalChunks - 1)
            {
                using var finalStream = new MemoryStream();
                for (int i = 0; i < totalChunks; i++)
                {
                    var tempChunkPath = Path.Combine(tempDir, $"{fileId}_chunk_{i}");
                    if (System.IO.File.Exists(tempChunkPath))
                    {
                        var chunkData = await System.IO.File.ReadAllBytesAsync(tempChunkPath);
                        await finalStream.WriteAsync(chunkData);
                        // 删除临时块文件
                        System.IO.File.Delete(tempChunkPath);
                    }
                }

                // 完成文件上传
                var result = await _fileManagementService.CompleteFileUploadAsync(fileId, finalStream.ToArray());
                if (result)
                {
                    return Success(null, "FileUploadCompleted");
                }
                else
                {
                    return Fail("FileUploadFailed");
                }
            }

            return Success(null, "ChunkUploadedSuccessfully");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="token">认证token（可选，用于新窗口下载）</param>
        /// <returns>重定向到文件URL</returns>
        [HttpGet("download/{fileId}")]
        [AllowAnonymous] // 允许匿名访问，但会验证token或JWT
        public async Task<IActionResult> DownloadFile(int fileId, [FromQuery] string token = null)
        {
            int userId = GetCurrentUserId();
            string filePath = string.Empty;
            string originalFileName = string.Empty;

            // 验证文件ID是否匹配
            var file = await _fileManagementService.GetFileItem(fileId);
            if (file == null)
            {
                return NotFoundResult();
            }

            // 如果JWT无效且提供了URL token，则验证URL token
            if (userId > 0)
            {
                var result = await _fileManagementService.GetFilePathAndNameAsync(fileId, userId);
                filePath = result.filePath;
                originalFileName = result.originalFileName;
            }
            else
            {
                if (string.IsNullOrEmpty(token))
                {
                    return Fail("InvalidDownloadToken");
                }

                // 验证临时下载token
                var temporaryToken = await _temporaryDownloadTokenService.ValidateTokenAsync(token);
                if (temporaryToken == null)
                {
                    return Fail("DownloadTokenExpired");
                }

                // 既不是文件也不是文件夹，拒绝访问
                if (file.Id != fileId && file.FolderId != fileId)
                {
                    return Fail("InvalidDownloadToken");
                }

                var result = await _fileManagementService.GetFilePathAndNameAsync(fileId);
                filePath = result.filePath;
                originalFileName = result.originalFileName;
            }

            // 检查文件是否存在
            if (!System.IO.File.Exists(filePath))
            {
                return NotFoundResult("FileNotFound");
            }

            // 构建文件访问URL
            var fileStoragePath = await _fileManagementService.GetStoragePathAsync();
            var fileRelativePath = filePath.Substring(fileStoragePath.Length + 1).Split('\\').Aggregate((a, b) => a + "/" + Uri.EscapeDataString(b));
            var fileUrl = $"/files/{fileRelativePath}";

            // 记录下载日志
            var clientIP = GetClientIP();
            var userAgent = Request.Headers["User-Agent"].ToString();
            var userName = User.Identity.IsAuthenticated ? User.Identity.Name : GetLocalizedText("Anonymous");
            await _logService.LogDownloadAsync(userId, userName, file.Id, originalFileName, Uri.UnescapeDataString(fileRelativePath), file.Size, clientIP, userAgent);

            return Success(new { downloadUrl = fileUrl });
        }

        /// <summary>
        /// 获取下载链接
        /// </summary>
        /// <param name="fileId">文件ID</param>
        [HttpGet("getfile-url/{fileId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFileUrl(int fileId)
        {
            // 通过服务层获取文件路径和原始文件名
            var result = await _fileManagementService.GetFilePathAndNameAsync(fileId);
            var filePath = result.filePath;
            var originalFileName = result.originalFileName;

            // 检查文件是否存在
            if (!System.IO.File.Exists(filePath))
            {
                return NotFoundResult("FileNotFound");
            }

            // 构建文件访问URL
            var fileStoragePath = await _fileManagementService.GetStoragePathAsync();
            var fileRelativePath = filePath.Substring(fileStoragePath.Length + 1).Split('\\').Aggregate((a, b) => a + "/" + Uri.EscapeDataString(b));
            var fileUrl = $"/files/{fileRelativePath}";

            return Success(new { downloadUrl = fileUrl });
        }

        /// <summary>
        /// 生成临时下载链接（支持通过URL参数传递token）
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns>临时下载链接</returns>
        [HttpGet("download-url/{fileId}")]
        public async Task<IActionResult> GenerateDownloadUrl(int fileId)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            // 验证用户是否有权限下载该文件
            var result = await _fileManagementService.GetFilePathAndNameAsync(fileId, userId);
            var filePath = result.filePath;
            var originalFileName = result.originalFileName;

            // 生成临时token并存储到缓存
            var temporaryToken = await _temporaryDownloadTokenService.CreateTemporaryTokenAsync(fileId, userId, 30); // 30秒有效期

            // 构建下载URL
            var downloadUrl = $"/squadfile-api/filemanagement/download/{fileId}?token={temporaryToken}";

            return Success(new { downloadUrl = downloadUrl }, "DownloadUrlGeneratedSuccessfully");
        }

        /// <summary>
        /// 分享文件
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="request">分享请求</param>
        /// <returns>分享结果</returns>
        [HttpPost("share/{fileId}")]
        public async Task<IActionResult> ShareFile(int fileId, [FromBody] ShareFileRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _fileManagementService.ShareFileAsync(fileId, request, userId);
            if (result)
            {
                return Success(null, "FileSharedSuccessfully");
            }
            else
            {
                return Fail("FileShareFailed");
            }
        }

        /// <summary>
        /// 验证分享密码
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="password">密码</param>
        /// <returns>验证结果</returns>
        [HttpPost("share/{fileId}/validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateSharePassword(int fileId, [FromBody] string password)
        {
            var result = await _fileManagementService.ValidateSharePasswordAsync(fileId, password);
            return Success(new { IsValid = result }, "PasswordValidationCompleted");
        }

        /// <summary>
        /// 获取文件夹的权限列表
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <returns>权限列表</returns>
        [HttpGet("permissions/folders/{folderId}")]
        public async Task<IActionResult> GetFolderPermissions(int folderId)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            // 检查用户是否有权限查看该文件夹的权限（必须是文件夹创建者）
            var folders = await _fileManagementService.GetUserFoldersAsync(userId);
            if (!folders.Any(f => f.Id == folderId))
            {
                return UnauthorizedResult("NoPermissionToViewPermissions");
            }

            var permissions = await _fileManagementService.GetFolderPermissionsAsync(folderId);
            return Success(permissions, "PermissionsRetrievedSuccessfully");
        }

        /// <summary>
        /// 更新文件夹
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="request">更新文件夹请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("folders/{folderId}")]
        public async Task<IActionResult> UpdateFolder(int folderId, [FromBody] UpdateFolderRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _fileManagementService.UpdateFolderAsync(folderId, request, userId);
            if (result)
            {
                return Success(null, "FolderUpdatedSuccessfully");
            }
            else
            {
                return Fail("FolderUpdateFailed");
            }
        }

        /// <summary>
        /// 获取文件夹中的文件列表（用于分享页面）
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="token">认证token</param>
        /// <returns>文件列表</returns>
        [HttpGet("folders/{folderId}/files/share")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFilesInFolderForShare(int folderId, [FromQuery] string token = null)
        {
            // 如果提供了token，验证token
            if (!string.IsNullOrEmpty(token))
            {
                var temporaryToken = await _temporaryDownloadTokenService.ValidateTokenAsync(token);
                if (temporaryToken != null && temporaryToken.FileId == folderId)
                {
                    // Token有效，获取文件夹内容
                    var files = await _fileManagementService.GetFilesInFolderAsync(folderId);
                    return Success(files, "FilesRetrievedSuccessfully");
                }
            }

            // 检查文件夹是否是公开的
            var folder = await _fileManagementService.GetFolderByIdAsync(folderId);
            if (folder != null && folder.IsPublic)
            {
                // 公开文件夹，允许匿名访问
                var files = await _fileManagementService.GetFilesInFolderAsync(folderId); // 0表示匿名用户
                return Success(files, "FilesRetrievedSuccessfully");
            }

            return UnauthorizedResult("Unauthorized");
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("folders/{folderId}")]
        public async Task<IActionResult> DeleteFolder(int folderId)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _fileManagementService.DeleteFolderAsync(folderId, userId);
            if (result)
            {
                return Success(null, "FolderDeletedSuccessfully");
            }
            else
            {
                return Fail("FolderDeleteFailed");
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("files/{fileId}")]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _fileManagementService.DeleteFileAsync(fileId, userId);
            if (result)
            {
                return Success(null, "FileDeletedSuccessfully");
            }
            else
            {
                return Fail("FileDeleteFailed");
            }
        }

        /// <summary>
        /// 搜索文件和文件夹
        /// </summary>
        /// <param name="query">搜索关键词</param>
        /// <param name="folderId">文件夹ID（可选，默认为根目录）</param>
        /// <returns>搜索结果</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchItems([FromQuery] string query, [FromQuery] int folderId = 0)
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return UnauthorizedResult("Unauthorized");
            }

            var result = await _fileManagementService.SearchItemsAsync(query, folderId, userId);
            return Success(result, "SearchCompletedSuccessfully");
        }
    }
}