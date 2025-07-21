using FirstProject.Models;

namespace FirstProject.Interfaces
{
    public interface IAuthService
    {
        public UserLoginResponse AuthLogin(UserLoginRequest userLoginRequest);
    }
}
