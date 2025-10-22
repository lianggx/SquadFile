using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SquadFile.Data;
using SquadFile.Models;
using SquadFile.Models.ViewModel;

namespace SquadFile.Services
{
    public class FileManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public FileManagementService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// 获取当前存储路径
        /// </summary>
        /// <returns>存储路径</returns>
        public async Task<string> GetStoragePathAsync()
        {
            var uploads = _configuration["Uploads"];
            // 确保存储目录存在
            if (!string.IsNullOrEmpty(uploads))
            {
                // 获取应用程序根目录
                var appRoot = Directory.GetCurrentDirectory();
                // 构建完整的存储路径
                var fullPath = Path.Combine(appRoot, uploads);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
            }

            // 返回相对于路径
            var appRootPath = Directory.GetCurrentDirectory();
            return Path.Combine(appRootPath, uploads);
        }

        /// <summary>
        /// 获取用户有权限的文件夹列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="folderId"></param>
        /// <returns>文件夹列表</returns>
        public async Task<bool> HasFolderPermissionAsync(int userId, int folderId)
        {
            // 获取用户有权限访问的文件夹ID列表
            var count = await _context.FolderPermissions
                .Where(p => p.UserId == userId && !p.IsDeleted && p.FolderId == folderId).CountAsync();
            return count > 0;
        }

        /// <summary>
        /// 获取用户有权限的文件夹列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>文件夹列表</returns>
        public async Task<List<FileFolderViewModel>> GetUserFoldersAsync(int userId)
        {
            // 获取用户有权限访问的文件夹ID列表
            var folderPermissions = await _context.FolderPermissions
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .Select(p => p.FolderId)
                .ToListAsync();

            var folderIds = folderPermissions.ToList();

            // 添加用户创建的文件夹
            var userCreatedFolders = await _context.FileFolders
                .Where(f => f.CreatedBy == userId && !f.IsDeleted)
                .Select(f => f.Id)
                .ToListAsync();

            folderIds.AddRange(userCreatedFolders);
            folderIds = folderIds.Distinct().ToList();

            // 获取文件夹信息，包括文件数量，公开的根目录
            var folders = await _context.FileFolders
                .Where(f => (folderIds.Contains(f.Id) || (f.IsPublic && f.ParentId > 0)) && !f.IsDeleted)
                .Select(f => new FileFolderViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    ParentId = f.ParentId,
                    CreatedBy = f.CreatedBy,
                    CreatedTime = f.CreatedTime,
                    UpdatedTime = f.UpdatedTime,
                    Description = f.Description,
                    FileCount = _context.FileRecords.Count(fr => fr.FolderId == f.Id && !fr.IsDeleted), // 添加文件数量
                    IsPublic = f.IsPublic // 添加公开属性
                })
                .ToListAsync();

            return folders;
        }

        /// <summary>
        /// 获取文件夹中的文件列表
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <returns>文件列表</returns>
        public async Task<List<FileRecordViewModel>> GetFilesInFolderAsync(int folderId)
        {
            var files = from f in _context.FileRecords
                        join u in _context.SysUsers on f.UploadedBy equals u.Id
                        where f.FolderId == folderId && !f.IsDeleted
                        select new FileRecordViewModel
                        {
                            Id = f.Id,
                            OriginalName = f.OriginalName,
                            Size = f.Size,
                            Extension = f.Extension,
                            FolderId = f.FolderId,
                            UploadedBy = f.UploadedBy,
                            UploadTime = f.UploadTime,
                            UpdatedTime = f.UpdatedTime,
                            Description = f.Description,
                            UserName = u.DisplayName
                        };

            return await files.ToListAsync();
        }

        /// <summary>
        /// 授权用户访问文件夹
        /// </summary>
        /// <param name="request">授权请求</param>
        /// <param name="currentUserId">当前用户ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> GrantFolderPermissionAsync(GrantPermissionRequest request, int currentUserId)
        {
            // 检查文件夹是否存在且当前用户有权限
            var folder = await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == request.FolderId && !f.IsDeleted);

            if (folder == null)
                return false;

            // 检查当前用户是否有权限授权（必须是文件夹创建者）
            if (folder.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("没有权限授权该文件夹");

            // 创建或更新权限记录
            var permission = await _context.FolderPermissions
                .FirstOrDefaultAsync(p => p.FolderId == request.FolderId && p.UserId == request.UserId && !p.IsDeleted);

            if (permission == null)
            {
                permission = new FolderPermission
                {
                    FolderId = request.FolderId,
                    UserId = request.UserId,
                    CanRead = request.CanRead,
                    CanUpload = request.CanUpload,
                    CanDelete = request.CanDelete,
                    CanCreateFolder = false, // 默认不授予创建文件夹权限
                    GrantedBy = currentUserId
                };
                _context.FolderPermissions.Add(permission);
            }
            else
            {
                permission.CanRead = request.CanRead;
                permission.CanUpload = request.CanUpload;
                permission.CanDelete = request.CanDelete;
                // 注意：不修改CanCreateFolder权限，需要使用专门的方法
                permission.GrantedBy = currentUserId;
                permission.GrantTime = DateTime.Now;
                _context.FolderPermissions.Update(permission);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 撤销用户对文件夹的访问权限
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> RevokeFolderPermissionAsync(int folderId, int userId)
        {
            var permission = await _context.FolderPermissions
                .FirstOrDefaultAsync(p => p.FolderId == folderId && p.UserId == userId && !p.IsDeleted);

            if (permission == null)
                return false;

            permission.IsDeleted = true;
            permission.DeletedTime = DateTime.Now;
            _context.FolderPermissions.Update(permission);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 准备文件上传
        /// </summary>
        /// <param name="request">上传文件请求</param>
        /// <param name="createdBy">创建者用户ID</param>
        /// <returns>上传的文件记录</returns>
        public async Task<FileRecord> PrepareFileUploadAsync(UploadFileRequest request, int createdBy)
        {
            // 检查文件夹权限
            if (!await HasFolderPermissionAsync(request.FolderId, createdBy, false))
            {
                throw new UnauthorizedAccessException("没有在该文件夹上传文件的权限");
            }

            // 生成唯一文件名
            var uniqueFileName = GenerateUniqueFileName(request.OriginalName);

            var fileRecord = new FileRecord
            {
                OriginalName = request.OriginalName,
                StorageName = uniqueFileName,
                Size = request.Size,
                Extension = request.Extension,
                FolderId = request.FolderId,
                UploadedBy = createdBy,
                UploadTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                Description = request.Description
            };

            _context.FileRecords.Add(fileRecord);
            await _context.SaveChangesAsync();

            return fileRecord;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="request">创建文件夹请求</param>
        /// <param name="createdBy">创建者用户ID</param>
        /// <returns>创建的文件夹</returns>
        public async Task<FileFolder> CreateFolderAsync(CreateFolderRequest request, int createdBy)
        {
            // 检查用户是否有权限在指定的父文件夹中创建子文件夹
            if (request.ParentId.HasValue)
            {
                var hasCreatePermission = await HasCreateFolderPermissionAsync(request.ParentId.Value, createdBy);
                if (!hasCreatePermission)
                {
                    throw new UnauthorizedAccessException("没有在该文件夹下创建子文件夹的权限");
                }
            }
            else
            {
                // 根目录创建文件夹权限检查（默认允许管理员创建）
                var user = await _context.SysUsers.FirstOrDefaultAsync(u => u.Id == createdBy);
                if (user == null || user.Role != "Admin")
                {
                    throw new UnauthorizedAccessException("没有在根目录创建文件夹的权限");
                }
            }

            var folder = new FileFolder
            {
                Name = request.Name,
                ParentId = request.ParentId,
                CreatedBy = createdBy,
                Description = request.Description,
                IsPublic = request.IsPublic // 添加公开属性
            };

            _context.FileFolders.Add(folder);
            await _context.SaveChangesAsync();

            // 创建物理文件夹
            await CreatePhysicalFolderAsync(folder);

            return folder;
        }

        /// <summary>
        /// 创建物理文件夹
        /// </summary>
        /// <param name="folder">文件夹实体</param>
        private async Task CreatePhysicalFolderAsync(FileFolder folder)
        {
            try
            {
                var storagePath = await GetStoragePathAsync();
                var folderPath = Path.Combine(storagePath, folder.Id.ToString());

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }
            catch (Exception ex)
            {
                // 记录日志但不中断操作
                Console.WriteLine($"创建物理文件夹失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新文件夹
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="request">更新请求</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateFolderAsync(int folderId, UpdateFolderRequest request, int userId)
        {
            var folder = await _context.FileFolders.FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted);
            if (folder == null)
                return false;

            // 检查用户是否有权限更新该文件夹
            if (!await HasFolderPermissionAsync(folderId, userId, true))
            {
                throw new UnauthorizedAccessException("没有更新该文件夹的权限");
            }

            folder.Name = request.Name;
            folder.Description = request.Description;
            folder.IsPublic = request.IsPublic;
            folder.UpdatedTime = DateTime.Now;

            _context.FileFolders.Update(folder);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 删除文件夹（软删除）
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteFolderAsync(int folderId, int userId)
        {
            var folder = await _context.FileFolders.FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted);
            if (folder == null)
                return false;

            // 检查用户是否有权限删除该文件夹
            if (!await HasFolderPermissionAsync(folderId, userId, true))
            {
                throw new UnauthorizedAccessException("没有删除该文件夹的权限");
            }

            // 软删除文件夹
            folder.IsDeleted = true;
            folder.DeletedTime = DateTime.Now;
            folder.UpdatedTime = DateTime.Now;

            // 同时删除文件夹内的所有文件记录
            var files = await _context.FileRecords
                .Where(f => f.FolderId == folderId && !f.IsDeleted)
                .ToListAsync();

            foreach (var file in files)
            {
                file.IsDeleted = true;
                file.DeletedTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            // 删除物理文件夹
            await DeletePhysicalFolderAsync(folder);

            return true;
        }

        /// <summary>
        /// 删除物理文件夹
        /// </summary>
        /// <param name="folder">文件夹实体</param>
        private async Task DeletePhysicalFolderAsync(FileFolder folder)
        {
            try
            {
                var storagePath = await GetStoragePathAsync();
                var folderPath = Path.Combine(storagePath, folder.Id.ToString());

                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
            }
            catch (Exception ex)
            {
                // 记录日志但不中断操作
                Console.WriteLine($"删除物理文件夹失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取文件夹列表
        /// </summary>
        /// <param name="parentId">父文件夹ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>文件夹列表</returns>
        public async Task<List<FileFolderViewModel>> GetFoldersAsync(int parentId, int userId)
        {
            // 检查用户是否有权限访问该文件夹
            if (!await HasFolderPermissionAsync(parentId, userId, true))
            {
                throw new UnauthorizedAccessException("没有访问该文件夹的权限");
            }

            var folders = await _context.FileFolders
                .Where(f => f.ParentId == parentId && !f.IsDeleted)
                .Select(f => new FileFolderViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    ParentId = f.ParentId,
                    CreatedBy = f.CreatedBy,
                    CreatedTime = f.CreatedTime,
                    UpdatedTime = f.UpdatedTime,
                    Description = f.Description,
                    IsPublic = f.IsPublic // 添加公开属性
                })
                .ToListAsync();

            return folders;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="request">上传文件请求</param>
        /// <param name="createdBy">创建者用户ID</param>
        /// <returns>上传的文件记录</returns>
        public async Task<FileRecord> UploadFileAsync(UploadFileRequest request, int createdBy)
        {
            // 检查文件夹权限
            if (!await HasFolderPermissionAsync(request.FolderId, createdBy, false))
            {
                throw new UnauthorizedAccessException("没有在该文件夹上传文件的权限");
            }

            // 生成唯一文件名
            var uniqueFileName = GenerateUniqueFileName(request.OriginalName);

            var fileRecord = new FileRecord
            {
                OriginalName = request.OriginalName,
                StorageName = uniqueFileName,
                Size = request.Size,
                Extension = request.Extension,
                FolderId = request.FolderId,
                UploadedBy = createdBy,
                UploadTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                Description = request.Description
            };

            _context.FileRecords.Add(fileRecord);
            await _context.SaveChangesAsync();

            return fileRecord;
        }

        /// <summary>
        /// 完成文件上传
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="fileData">文件数据</param>
        /// <returns>是否成功</returns>
        public async Task<bool> CompleteFileUploadAsync(int fileId, byte[] fileData)
        {
            var fileRecord = await _context.FileRecords.FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted);
            if (fileRecord == null)
                return false;

            try
            {
                // 获取存储路径
                var storagePath = await GetStoragePathAsync();

                // 确保文件夹目录存在
                var folderPath = Path.Combine(storagePath, fileRecord.FolderId.ToString());
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 保存文件到磁盘（在对应的文件夹目录下）
                var filePath = Path.Combine(folderPath, fileRecord.StorageName);
                await File.WriteAllBytesAsync(filePath, fileData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取文件存储路径
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="userId">用户ID（0表示匿名访问）</param>
        /// <returns>文件路径</returns>
        public async Task<string> GetFilePathAsync(int fileId, int userId)
        {
            // 检查文件是否存在
            var file = await _context.FileRecords.FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted);
            if (file == null)
                throw new ArgumentException("文件不存在");

            // 检查用户权限
            if (userId == 0)
            {
                // 匿名访问需要通过其他方式验证（如临时token）
                throw new UnauthorizedAccessException("匿名访问需要通过其他方式验证");
            }
            else
            {
                // 检查文件夹权限
                if (!await HasFolderPermissionAsync(file.FolderId, userId, true))
                {
                    throw new UnauthorizedAccessException("没有访问该文件的权限");
                }
            }

            // 获取存储路径
            var storagePath = await GetStoragePathAsync();
            var folderPath = Path.Combine(storagePath, file.FolderId.ToString());
            var filePath = Path.Combine(folderPath, file.StorageName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("文件不存在");

            return filePath;
        }

        /// <summary>
        /// 获取文件存储路径和原始文件名
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>文件路径和原始文件名的元组</returns>
        public async Task<(string filePath, string originalFileName)> GetFilePathAndNameAsync(int fileId, int userId)
        {
            // 检查文件是否存在
            var file = await _context.FileRecords.FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted);
            if (file == null)
                throw new ArgumentException("文件不存在");

            // 检查用户权限
            if (!await HasFolderPermissionAsync(file.FolderId, userId, true))
            {
                throw new UnauthorizedAccessException("没有访问该文件的权限");
            }

            return await GetFilePathAndNameAsync(fileId);
        }

        /// <summary>
        /// 获取文件存储路径和原始文件名
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>文件路径和原始文件名的元组</returns>
        public async Task<(string filePath, string originalFileName)> GetFilePathAndNameAsync(int fileId)
        {
            // 检查文件是否存在
            var file = await _context.FileRecords.FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted);
            if (file == null)
                throw new ArgumentException("文件不存在");

            // 获取存储路径
            var storagePath = await GetStoragePathAsync();
            var folderPath = Path.Combine(storagePath, file.FolderId.ToString());
            var filePath = Path.Combine(folderPath, file.StorageName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("文件不存在");

            return (filePath, file.OriginalName);
        }

        /// <summary>
        /// 分享文件
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="request">分享请求</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> ShareFileAsync(int fileId, ShareFileRequest request, int userId)
        {
            // 这个方法现在由ShareService处理
            throw new NotImplementedException("请使用ShareService.ShareFileAsync方法");
        }

        /// <summary>
        /// 生成文件分享短链接
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns>分享短链接</returns>
        public async Task<FileRecord> GetFileItem(int fileId)
        {
            return await _context.FileRecords.Where(f => f.Id == fileId && !f.IsDeleted).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 验证分享密码
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="password">密码</param>
        /// <returns>是否正确</returns>
        public async Task<bool> ValidateSharePasswordAsync(int fileId, string password)
        {
            // 这个方法现在由ShareService处理
            throw new NotImplementedException("请使用ShareService.ValidateSharePassword方法");
        }

        /// <summary>
        /// 获取文件夹的权限列表
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <returns>权限列表</returns>
        public async Task<List<FolderPermissionViewModel>> GetFolderPermissionsAsync(int folderId)
        {
            var permissions = await _context.FolderPermissions
                .Where(p => p.FolderId == folderId && !p.IsDeleted)
                .Join(_context.SysUsers,
                    permission => permission.UserId,
                    user => user.Id,
                    (permission, user) => new { Permission = permission, User = user })
                .Join(_context.SysUsers,
                    combined => combined.Permission.GrantedBy,
                    grantUser => grantUser.Id,
                    (combined, grantUser) => new FolderPermissionViewModel
                    {
                        Id = combined.Permission.Id,
                        FolderId = combined.Permission.FolderId,
                        UserId = combined.Permission.UserId,
                        UserName = combined.User.Username,
                        UserDisplayName = combined.User.DisplayName ?? combined.User.Username,
                        CanRead = combined.Permission.CanRead,
                        CanUpload = combined.Permission.CanUpload,
                        CanDelete = combined.Permission.CanDelete,
                        CanCreateFolder = combined.Permission.CanCreateFolder,
                        GrantTime = combined.Permission.GrantTime,
                        GrantedBy = combined.Permission.GrantedBy,
                        GrantedByName = grantUser.DisplayName ?? grantUser.Username
                    })
                .ToListAsync();

            return permissions;
        }

        /// <summary>
        /// 生成唯一文件名
        /// </summary>
        /// <param name="originalName">原始文件名</param>
        /// <returns>唯一文件名</returns>
        private string GenerateUniqueFileName(string originalName)
        {
            var extension = Path.GetExtension(originalName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalName);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var random = new Random().Next(1000, 9999);
            return $"{fileNameWithoutExtension}_{timestamp}_{random}{extension}";
        }

        /// <summary>
        /// 检查文件夹权限
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="isRead">是否为读取操作</param>
        /// <returns>是否有权限</returns>
        public async Task<bool> HasFolderPermissionAsync(int folderId, int userId, bool isRead)
        {
            // 管理员
            var user = await _context.SysUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user.Role == "Admin")
                return true;

            // 根目录（folderId = 0）对所有用户开放
            if (folderId == 0)
                return false;

            // 获取文件夹信息
            var folder = await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted);

            if (folder == null)
                return false;

            // 如果文件夹是公开的，所有登录用户都有读取权限
            if (folder.IsPublic && isRead)
                return true;

            // 检查是否为文件夹创建者
            if (folder.CreatedBy == userId)
                return true;

            // 检查是否有共享权限
            var permission = await _context.FolderPermissions
                .FirstOrDefaultAsync(p => p.FolderId == folderId && p.UserId == userId && !p.IsDeleted);

            if (permission != null)
            {
                // 读取操作需要读权限或写权限
                if (isRead && (permission.CanRead || permission.CanUpload))
                    return true;

                // 写入操作需要写权限
                if (!isRead && permission.CanUpload)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 删除文件（软删除）
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteFileAsync(int fileId, int userId)
        {
            var file = await _context.FileRecords.FirstOrDefaultAsync(f => f.Id == fileId && !f.IsDeleted);
            if (file == null)
                return false;

            // 检查用户是否有权限删除该文件
            if (!await HasFolderPermissionAsync(file.FolderId, userId, true))
            {
                throw new UnauthorizedAccessException("没有删除该文件的权限");
            }

            // 软删除文件
            file.IsDeleted = true;
            file.DeletedTime = DateTime.Now;
            file.UpdatedTime = DateTime.Now;

            await _context.SaveChangesAsync();

            // 删除物理文件
            await DeletePhysicalFileAsync(file);

            return true;
        }

        /// <summary>
        /// 删除物理文件
        /// </summary>
        /// <param name="file">文件实体</param>
        private async Task DeletePhysicalFileAsync(FileRecord file)
        {
            try
            {
                var storagePath = await GetStoragePathAsync();
                var folderPath = Path.Combine(storagePath, file.FolderId.ToString());
                var filePath = Path.Combine(folderPath, file.StorageName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // 记录日志但不中断操作
                Console.WriteLine($"删除物理文件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 搜索文件和文件夹
        /// </summary>
        /// <param name="query">搜索关键词</param>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>搜索结果</returns>
        public async Task<SearchResultViewModel> SearchItemsAsync(string query, int folderId, int userId)
        {
            // 检查用户是否有权限访问该文件夹
            if (!await HasFolderPermissionAsync(folderId, userId, true))
            {
                throw new UnauthorizedAccessException("没有访问该文件夹的权限");
            }

            var result = new SearchResultViewModel();

            // 搜索文件夹（在指定文件夹及其子文件夹中搜索）
            var foldersQuery = from f in _context.FileFolders
                               join u in _context.SysUsers on f.CreatedBy equals u.Id
                               where !f.IsDeleted &&
                                     (f.Name.Contains(query) || (f.Description != null && f.Description.Contains(query)))
                               select new FileFolderViewModel
                               {
                                   Id = f.Id,
                                   Name = f.Name,
                                   ParentId = f.ParentId,
                                   CreatedBy = f.CreatedBy,
                                   CreatedTime = f.CreatedTime,
                                   UpdatedTime = f.UpdatedTime,
                                   Description = f.Description,
                                   FileCount = _context.FileRecords.Count(fr => fr.FolderId == f.Id && !fr.IsDeleted),
                                   IsPublic = f.IsPublic,
                                   UserName = u.DisplayName ?? u.Username // 添加创建者名称
                               };
            if (folderId > 0)
                foldersQuery.Where(x => x.Id == folderId);

            result.Folders = await foldersQuery.ToListAsync();

            // 搜索文件（在指定文件夹中搜索）
            var filesQuery = from f in _context.FileRecords
                             join u in _context.SysUsers on f.UploadedBy equals u.Id
                             where !f.IsDeleted &&
                                   (f.OriginalName.Contains(query) || (f.Description != null && f.Description.Contains(query)))
                             select new FileRecordViewModel
                             {
                                 Id = f.Id,
                                 OriginalName = f.OriginalName,
                                 Size = f.Size,
                                 Extension = f.Extension,
                                 FolderId = f.FolderId,
                                 UploadedBy = f.UploadedBy,
                                 UploadTime = f.UploadTime,
                                 UpdatedTime = f.UpdatedTime,
                                 Description = f.Description,
                                 UserName = u.DisplayName ?? u.Username, // 添加上传者名称
                                 FormattedSize = FormatFileSize(f.Size) // 添加格式化的文件大小
                             };

            if (folderId > 0)
                filesQuery.Where(x => x.FolderId == folderId);

            result.Files = await filesQuery.ToListAsync();

            return result;
        }

        /// <summary>
        /// 检查用户是否有创建文件夹权限
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否有创建文件夹权限</returns>
        public async Task<bool> HasCreateFolderPermissionAsync(int folderId, int userId)
        {
            // 管理员拥有所有权限
            var user = await _context.SysUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null && user.Role == "Admin")
                return true;

            // 获取文件夹信息
            var folder = await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted);

            if (folder == null)
                return false;

            // 如果文件夹是公开的，所有登录用户都有读取权限，但不一定有创建权限
            // 公开文件夹的创建权限仍需单独授权

            // 检查是否为文件夹创建者
            if (folder.CreatedBy == userId)
                return true;

            // 检查是否有共享的创建文件夹权限
            var permission = await _context.FolderPermissions
                .FirstOrDefaultAsync(p => p.FolderId == folderId && p.UserId == userId && !p.IsDeleted);

            if (permission != null && permission.CanCreateFolder)
                return true;

            return false;
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>格式化的文件大小字符串</returns>
        private static string FormatFileSize(long bytes)
        {
            if (bytes == 0) return "0 Bytes";

            var sizes = new[] { "Bytes", "KB", "MB", "GB", "TB" };
            var i = Math.Floor(Math.Log(bytes) / Math.Log(1024));
            var size = Math.Round(bytes / Math.Pow(1024, i), 2);
            return $"{size} {sizes[(int)i]}";
        }

        /// <summary>
        /// 根据ID获取文件夹
        /// </summary>
        /// <param name="folderId">文件夹ID</param>
        /// <returns>文件夹信息</returns>
        public async Task<FileFolder?> GetFolderByIdAsync(int folderId)
        {
            return await _context.FileFolders
                .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted);
        }

        /// <summary>
        /// 批量授权用户访问文件夹
        /// </summary>
        /// <param name="requests">授权请求列表</param>
        /// <param name="currentUserId">当前用户ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> GrantFolderPermissionsBatchAsync(List<GrantPermissionRequest> requests, int currentUserId)
        {
            foreach (var request in requests)
            {
                // 检查文件夹是否存在且当前用户有权限
                var folder = await _context.FileFolders
                    .FirstOrDefaultAsync(f => f.Id == request.FolderId && !f.IsDeleted);

                if (folder == null)
                    continue;

                // 检查当前用户是否有权限授权（必须是文件夹创建者）
                if (folder.CreatedBy != currentUserId)
                    throw new UnauthorizedAccessException("没有权限授权该文件夹");

                // 创建或更新权限记录
                var permission = await _context.FolderPermissions
                    .FirstOrDefaultAsync(p => p.FolderId == request.FolderId && p.UserId == request.UserId && !p.IsDeleted);

                if (permission == null)
                {
                    permission = new FolderPermission
                    {
                        FolderId = request.FolderId,
                        UserId = request.UserId,
                        CanRead = request.CanRead,
                        CanUpload = request.CanUpload,
                        CanDelete = request.CanDelete,
                        CanCreateFolder = request.CanCreateFolder,
                        GrantedBy = currentUserId
                    };
                    _context.FolderPermissions.Add(permission);
                }
                else
                {
                    permission.CanRead = request.CanRead;
                    permission.CanUpload = request.CanUpload;
                    permission.CanDelete = request.CanDelete;
                    permission.CanCreateFolder = request.CanCreateFolder;
                    permission.GrantedBy = currentUserId;
                    permission.GrantTime = DateTime.Now;
                    _context.FolderPermissions.Update(permission);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 批量撤销用户对文件夹的访问权限
        /// </summary>
        /// <param name="requests">撤销权限请求列表</param>
        /// <param name="currentUserId">当前用户ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> RevokeFolderPermissionsBatchAsync(List<RevokePermissionRequest> requests, int currentUserId)
        {
            foreach (var request in requests)
            {
                var permission = await _context.FolderPermissions
                    .FirstOrDefaultAsync(p => p.FolderId == request.FolderId && p.UserId == request.UserId && !p.IsDeleted);

                if (permission == null)
                    continue;

                // 检查当前用户是否有权限撤销（必须是文件夹创建者）
                var folder = await _context.FileFolders
                    .FirstOrDefaultAsync(f => f.Id == request.FolderId && !f.IsDeleted);

                if (folder == null || folder.CreatedBy != currentUserId)
                    throw new UnauthorizedAccessException("没有权限撤销该文件夹的权限");

                permission.IsDeleted = true;
                permission.DeletedTime = DateTime.Now;
                _context.FolderPermissions.Update(permission);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}