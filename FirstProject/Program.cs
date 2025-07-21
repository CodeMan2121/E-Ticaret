using FirstProject.Concretes;
using FirstProject.Interfaces;
using FirstProject.Models;
using FirstProject.Models.Authentication;
using FirstProject.Models.Context;
using FirstProject.Models.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddControllersWithViews();

builder.Services.AddSession();
builder.Services.AddAuthentication();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddRazorPages();


var app = builder.Build();



//JWT devreye giriyor.
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();//bunun yerine artýk JWT kullanýlacak ama durabilir.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();