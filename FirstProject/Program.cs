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

//CookieAuthentication için ExpireTimeSpan = TimeSpan.FromMinutes(20); ayarý var.
//Bu, Cookie tabanlý oturumun 20 dakika sonra süresinin dolacaðý anlamýna gelir.
//SlidingExpiration = true olsa bile, eðer kullanýcý 20 dakika boyunca
//hiçbir aktif istek yapmazsa veya tarayýcý oturumu kapanýrsa bu cookie oturumu düþer.
builder.Services.AddAuthentication(options =>
{//Authorize attribute'leri için default deðerler
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Yetkilendirme hatasý alýndýðýnda varsayýlan olarak Cookie Challange'ý tetiklensin
}).AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Giriþ yapýlmadýðýnda yönlendirilecek sayfa
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20); //Cookie'nin geçerlilik süresi... 20 dakika kullanýcý biþe yapmazsa cookie silinir.
    options.SlidingExpiration = true; // Her istekte süreyi yenile
    options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisiz eriþimde yönlendirilecek sayfa

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
            // Debugging için hata loglama
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // Token baþarýyla doðrulandýðýnda debug için loglama
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

app.UseSession();//bunun yerine artýk JWT kullanýlacak ama durabilir.


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();