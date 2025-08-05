using FirstProject.Interfaces;
using FirstProject.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FirstProject.Concretes
{
    public class TokenService : ITokenService
    {
        readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }
        public GenerateTokenResponse GenerateToken(GenerateTokenRequest request)
        {
            //Claim'leri AccountController'da oluşturmuştuk, requestler üzerinden buraya alıyoruz.
            var claims = new List<Claim>
            {
                new Claim("email", request.Email)
            };
            if (request.Roles != null)
            {
                foreach (var role in request.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Appsettings:Secret"]));
            var dateTime = DateTime.UtcNow;
            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: _configuration["Appsettings:ValidIssuer"],
                audience: _configuration["Appsettings:ValidAudience"],
                claims: claims,
                notBefore: dateTime,
                expires: dateTime.AddMinutes(1),
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );
        return new GenerateTokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
                AccessTokenExpireDate = dateTime.AddMinutes(1),
            };
        }
        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            //using kullanmamızın sebebi, RandomNumberGenerator.Create() metodu IDisposable arayüzünü implement etmesidir.
            //using ifadesi, bu nesnenin kullanımının bitiminde otomatik olarak Dispose metodunun çağrılmasını sağlar.
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }
            return Convert.ToBase64String(random);
        }
    }
}
