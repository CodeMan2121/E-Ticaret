using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models
{
    public class Role
    {
        //[Key] // Not needed if using EF Core conventions
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
