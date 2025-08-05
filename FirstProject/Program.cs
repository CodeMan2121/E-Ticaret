using FirstProject.Concretes;
using FirstProject.Interfaces;
using FirstProject.Models;
using FirstProject.Models.Context;
using FirstProject.Models.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddControllersWithViews();

builder.Services.AddSession();
//builder.Services.AddAuthentication();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

//CookieAuthentication i�in ExpireTimeSpan = TimeSpan.FromMinutes(20); ayar� var.
//Bu, Cookie tabanl� oturumun 20 dakika sonra s�resinin dolaca�� anlam�na gelir.
//SlidingExpiration = true olsa bile, e�er kullan�c� 20 dakika boyunca
//hi�bir aktif istek yapmazsa veya taray�c� oturumu kapan�rsa bu cookie oturumu d��er.
builder.Services.AddAuthentication(options =>
{//Authorize attribute'leri i�in default de�erler
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Yetkilendirme hatas� al�nd���nda varsay�lan olarak Cookie Challange'� tetiklensin
}).AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Giri� yap�lmad���nda y�nlendirilecek sayfa
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20); //Cookie'nin ge�erlilik s�resi... 20 dakika kullan�c� bi�e yapmazsa cookie silinir.
    options.SlidingExpiration = true; // Her istekte s�reyi yenile
    options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisiz eri�imde y�nlendirilecek sayfa

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["AppSettings:ValidIssuer"],
        ValidAudience = builder.Configuration["AppSettings:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Secret"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            // Debugging i�in hata loglama
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // Token ba�ar�yla do�ruland���nda debug i�in loglama
            Console.WriteLine("Token validated successfully.");
            return Task.CompletedTask;
        }
    };
});
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddAuthorization();

builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddRazorPages();


var app = builder.Build();


app.UseRouting();
app.UseStaticFiles();
app.UseCookiePolicy();
//JWT devreye giriyor.
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();//bunun yerine art�k JWT kullan�lacak ama durabilir.


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();