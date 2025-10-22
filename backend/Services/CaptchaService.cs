using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace SquadFile.Services
{
    public class CaptchaService
    {
        private readonly IDistributedCache _cache;
        private const string Characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // 去除容易混淆的字符
        private const int CodeLength = 4;
        private const int CacheExpirationMinutes = 5; // 验证码5分钟后过期
        
        public CaptchaService(IDistributedCache cache)
        {
            _cache = cache;
        }
        
        public class CaptchaResult
        {
            public string Id { get; set; } = string.Empty;
            public string Code { get; set; } = string.Empty;
            public byte[] ImageData { get; set; } = [];
            public string ContentType { get; set; } = "image/png";
        }

        public CaptchaResult GenerateCaptcha()
        {
            // 生成唯一的验证码ID
            var id = Guid.NewGuid().ToString("N");
            var code = GenerateRandomCode();
            var imageData = GenerateCaptchaImage(code);
            
            // 将验证码存储在分布式缓存中
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
            };
            
            _cache.SetString($"captcha_{id}", code, cacheOptions);
            
            return new CaptchaResult
            {
                Id = id,
                Code = code,
                ImageData = imageData,
                ContentType = "image/png"
            };
        }

        private string GenerateRandomCode()
        {
            var random = new Random();
            var result = new StringBuilder(CodeLength);
            
            for (int i = 0; i < CodeLength; i++)
            {
                result.Append(Characters[random.Next(Characters.Length)]);
            }
            
            return result.ToString();
        }

        private byte[] GenerateCaptchaImage(string code)
        {
            const int width = 120;
            const int height = 40;
            
            // 创建位图和图形对象
            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);
            
            // 设置高质量绘图
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            // 设置背景
            graphics.Clear(Color.White);
            
            // 添加干扰线
            var random = new Random();
            for (int i = 0; i < 5; i++)
            {
                using var pen = new Pen(GetRandomColor(), 1);
                var startPoint = new Point(random.Next(width), random.Next(height));
                var endPoint = new Point(random.Next(width), random.Next(height));
                graphics.DrawLine(pen, startPoint, endPoint);
            }
            
            // 绘制验证码字符，每个字符随机倾斜和位置
            var charWidth = width / (code.Length + 1); // 每个字符的平均宽度
            for (int i = 0; i < code.Length; i++)
            {
                // 随机字体大小和样式
                using var font = new Font("Arial", random.Next(14, 18), GetRandomFontStyle());
                using var brush = new SolidBrush(GetRandomColor());
                
                // 为每个字符创建单独的变换
                var state = graphics.Save();
                
                // 随机旋转角度 (-30 到 30 度)
                var angle = random.Next(-30, 31);
                
                // 随机位置，使字符不排列整齐
                var x = (i * charWidth) + random.Next(-5, 10) + 5;
                var y = random.Next(5, 15);
                
                // 应用变换
                graphics.TranslateTransform(x, y);
                graphics.RotateTransform(angle);
                
                // 绘制字符
                graphics.DrawString(code[i].ToString(), font, brush, 0, 0);
                
                // 恢复图形状态
                graphics.Restore(state);
            }
            
            // 添加噪点
            for (int i = 0; i < 50; i++)
            {
                var xPoint = random.Next(width);
                var yPoint = random.Next(height);
                bitmap.SetPixel(xPoint, yPoint, GetRandomColor());
            }
            
            // 保存为内存流
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            return memoryStream.ToArray();
        }
        
        // 获取随机颜色
        private Color GetRandomColor()
        {
            var random = new Random();
            return Color.FromArgb(random.Next(50, 200), random.Next(50, 200), random.Next(50, 200));
        }
        
        // 获取随机字体样式
        private FontStyle GetRandomFontStyle()
        {
            var random = new Random();
            var styles = new[] { FontStyle.Regular, FontStyle.Bold };
            return styles[random.Next(styles.Length)];
        }
        
        // 验证验证码
        public bool ValidateCaptcha(string id, string userInput)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userInput))
                return false;
                
            var code = _cache.GetString($"captcha_{id}");
            if (string.IsNullOrEmpty(code))
                return false;
                
            var isValid = code.Equals(userInput, StringComparison.OrdinalIgnoreCase);
            
            // 验证后删除验证码（一次性使用）
            if (isValid)
            {
                _cache.Remove($"captcha_{id}");
            }
                
            return isValid;
        }
    }
}