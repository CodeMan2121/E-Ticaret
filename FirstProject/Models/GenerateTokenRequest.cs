namespace FirstProject.Models
{
    public class GenerateTokenRequest
    {
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
