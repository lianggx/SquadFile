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
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public JwtHelper()
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="secretKey">��Կ</param>
        /// <param name="issuer">������</param>
        /// <param name="audience">������</param>
        /// <param name="expireDays">��������</param>
        public JwtHelper(string secretKey, string issuer, string audience, int expireDays)
        {
            Key = secretKey;
            Issuer = issuer;
            Audience = audience;
            ExpireDays = expireDays;
        }

        /// <summary>
        /// ������¼Token
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
        /// ��֤��¼Token
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
        /// ��Կ
        /// </summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// ������
        /// </summary>
        public string Issuer { get; set; } = string.Empty;
        /// <summary>
        /// ������
        /// </summary>
        public string Audience { get; set; } = string.Empty;
        /// <summary>
        /// ��������
        /// </summary>
        public int ExpireDays { get; set; }
    }
}