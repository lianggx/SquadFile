using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SquadFile.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtHelper
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public JwtHelper()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="secretKey">密钥</param>
        /// <param name="issuer">发行者</param>
        /// <param name="audience">受众人</param>
        /// <param name="expireDays">过期天数</param>
        public JwtHelper(string secretKey, string issuer, string audience, int expireDays)
        {
            Key = secretKey;
            Issuer = issuer;
            Audience = audience;
            ExpireDays = expireDays;
        }

        /// <summary>
        /// 创建登录Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="username"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public string GenerateToken(int userId, string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.Now.AddDays(ExpireDays),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 验证登录Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public bool ValidateToken(string token, out ClaimsPrincipal? principal)
        {
            principal = null;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Key);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                principal = new ClaimsPrincipal(new ClaimsIdentity(tokenHandler.ReadJwtToken(token).Claims));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 密钥
        /// </summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// 发行者
        /// </summary>
        public string Issuer { get; set; } = string.Empty;
        /// <summary>
        /// 受众人
        /// </summary>
        public string Audience { get; set; } = string.Empty;
        /// <summary>
        /// 过期天数
        /// </summary>
        public int ExpireDays { get; set; }
    }
}