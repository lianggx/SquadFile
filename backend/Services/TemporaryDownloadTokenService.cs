using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using SquadFile.Models;

namespace SquadFile.Services
{
    /// <summary>
    /// 临时下载Token服务（使用内存存储）
    /// </summary>
    public class TemporaryDownloadTokenService
    {
        private readonly IDistributedCache _cache;

        public TemporaryDownloadTokenService(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 创建临时下载Token
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="expireSeconds">过期时间（秒）</param>
        /// <returns>临时Token</returns>
        public async Task<string> CreateTemporaryTokenAsync(int fileId, int userId, int expireSeconds = 30)
        {
            // 生成唯一的Token
            var token = Guid.NewGuid().ToString("N");

            // 创建Token记录
            var temporaryToken = new TemporaryDownloadToken
            {
                Token = token,
                FileId = fileId,
                UserId = userId,
                CreatedTime = DateTime.Now,
                ExpireTime = DateTime.Now.AddSeconds(expireSeconds),
                IsUsed = false
            };

            // 存储到内存中
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds)
            };
            
            // 序列化并存储到缓存
            var json = JsonSerializer.Serialize(temporaryToken);
            await _cache.SetStringAsync(token, json, options);

            return token;
        }

        /// <summary>
        /// 验证临时下载Token
        /// </summary>
        /// <param name="token">Token值</param>
        /// <returns>Token信息，如果无效则返回null</returns>
        public async Task<TemporaryDownloadToken?> ValidateTokenAsync(string token)
        {
            var json = await _cache.GetStringAsync(token);
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                var temporaryToken = JsonSerializer.Deserialize<TemporaryDownloadToken>(json);
                
                // 检查Token是否过期
                if (temporaryToken != null && temporaryToken.ExpireTime >= DateTime.Now)
                {
                    return temporaryToken;
                }
            }
            catch (JsonException)
            {
                // 如果反序列化失败，返回null
                return null;
            }

            return null;
        }
    }
}