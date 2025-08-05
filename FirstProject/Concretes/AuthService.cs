using FirstProject.Interfaces;
using FirstProject.Models;
using FirstProject.Models.Context;
using FirstProject.Models.Helpers;

namespace FirstProject.Concretes
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;
        public AuthService(ApplicationDbContext dbContext, ITokenService tokenService)
        {
            _context = dbContext;
            _tokenService = tokenService;
        }
        public UserLoginResponse AuthLogin(UserLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                throw new ArgumentNullException(nameof(request));
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == request.Email && u.Password == HashHelper.HashPassword(request.Password));
            
            UserLoginResponse response = new UserLoginResponse();
            
                //Token üretildi!
                var generatedTokenInformation = _tokenService.GenerateToken(new GenerateTokenRequest { 
                    Email = request.Email, 
                    Roles = request.Roles
                });
                
                //RefreshToken üretildi!
                string refreshToken = _tokenService.GenerateRefreshToken();
            
            if (user!=null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(7);
                //RefreshToken veritabanına kaydedilmeli ki her expiry süresi sonunda gidip kullanıcıya yeni token'ı verebilsin
                _context.SaveChanges();

                //Response ayarları
                response.AuthenticationResult = true;
                response.AuthToken = generatedTokenInformation.AccessToken;
                response.AccessTokenExpireDate = generatedTokenInformation.AccessTokenExpireDate;
                response.RefreshToken = refreshToken;
            }
            return response;

        }
    }
}
