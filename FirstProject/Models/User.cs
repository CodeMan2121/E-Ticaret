using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models
{
    public class User
    {
        [Key]
        //[Column("UserId")]
        public int UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireDate { get; set; }
        //public string Role { get; set; } = "User";

    }
}
