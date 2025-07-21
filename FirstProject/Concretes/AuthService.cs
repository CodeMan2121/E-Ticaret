using FirstProject.Interfaces;
using FirstProject.Models;
using FirstProject.Models.Context;
using FirstProject.Models.Helpers;

namespace FirstProject.Concretes
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        public AuthService(ApplicationDbContext dbContext)
        {
            _context = dbContext;

        }
        public UserLoginResponse AuthLogin(UserLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                throw new ArgumentNullException(nameof(request));
            }
            var user = _context.Users.SingleOrDefault(u => u.Email == request.Email && u.Password == HashHelper.HashPassword(request.Password));
            UserLoginResponse response = new UserLoginResponse();
            if (user!=null)
            {
                response.AuthenticationResult = true;
                response.AuthToken = string.Empty;
                response.AccessTokenExpireDate = DateTime.Now;
            }
            return response;

        }
    }
}
