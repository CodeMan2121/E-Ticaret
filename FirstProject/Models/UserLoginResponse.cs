namespace FirstProject.Models
{
    public class UserLoginResponse
    {
        public bool AuthenticationResult { get; set; }
        public string AuthToken { get; set; }
        public DateTime AccessTokenExpireDate { get; set; }
    }
}
