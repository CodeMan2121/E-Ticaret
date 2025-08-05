using Microsoft.EntityFrameworkCore;
namespace FirstProject.Models.Context
{
    public class ApplicationDbContext: DbContext
    {
        //public ApplicationDbContext()
        //{
        //}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<User>().HasKey(x => x.UserId);
            builder.Entity<User>().Property(x => x.UserId).HasColumnName("UserId");
            builder.Entity<User>().Property(x => x.FirstName).HasColumnName("FirstName");
            builder.Entity<User>().Property(x => x.LastName).HasColumnName("LastName");
            builder.Entity<User>().Property(x => x.Email).HasColumnName("Email");
            builder.Entity<User>().Property(x => x.Password).HasColumnName("Password");
            builder.Entity<User>().ToTable("Users");

            builder.Entity<Role>().HasKey(x => x.RoleId);
            builder.Entity<Role>().Property(x => x.RoleId).HasColumnName("RoleId");
            builder.Entity<Role>().Property(x => x.RoleName).HasColumnName("RoleName");
            builder.Entity<Role>().ToTable("Roles");

            builder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
            builder.Entity<UserRole>().Property(x => x.UserId).HasColumnName("UserId");
            builder.Entity<UserRole>().Property(x => x.RoleId).HasColumnName("RoleId");
            builder.Entity<UserRole>().HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
            builder.Entity<UserRole>().HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
            builder.Entity<UserRole>().ToTable("UserRoles");


            builder.Entity<Product>().HasKey(x => x.ProductId);
            builder.Entity<Product>().Property(x => x.ProductId).HasColumnName("ProductId");
            builder.Entity<Product>().Property(x => x.ProductName).HasColumnName("ProductName");
            builder.Entity<Product>().Property(x => x.Quantity).HasColumnName("Quantity");
            builder.Entity<Product>().ToTable("Products");

            base.OnModelCreating(builder);
        }
    }
}
