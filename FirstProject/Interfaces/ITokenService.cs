using FirstProject.Models;

namespace FirstProject.Interfaces
{
    public interface ITokenService
    {
        public GenerateTokenResponse GenerateToken(GenerateTokenRequest request);
        public string GenerateRefreshToken();
    }
}
