using Azure;
using FirstProject.Interfaces;
using FirstProject.Models;
using FirstProject.Models.Context;
using FirstProject.Models.DTOs;
using FirstProject.Models.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FirstProject.Controllers
{
    
    public class AccountController : Controller
    {
        //public ApplicationDbContext _context = new(); DI kullanmazsak parametresiz const kullanmak zorunda kalacam.Bunun için de kendim OnConfiguring'de ayar çekmem gerekecek.
        // OnConfiguring metodunun yerine DbContextOptions'li const kullandık. İki parametre hataya sebep olabilir.

        private readonly ApplicationDbContext _context;
        private readonly IValidator<User> _validator;  
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AccountController(ApplicationDbContext context, IValidator<User> validator, IAuthService authService, ITokenService tokenService)
        {
            _context = context;//DI, DbContextOptions ile otomatik bağlanır.Konfigürasyonlar otomatik yapılır.
            _validator = validator;
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserRequestDto request)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);
            if (user != null)
            {
                
                if (user.Password == HashHelper.HashPassword(request.Password))
                {
                    ClaimsIdentity identity = null;
                    bool isAuthenticate = false;

                    //Claim veriyoruz içinde Role de var.
                    _context.UserRoles.Where(ur => ur.UserId == user.UserId);
                    var roles = _context.Roles.Where(r => r.UserRoles.Any(ur => ur.UserId == user.UserId));
                    identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    //Her role'de claim nesnesi oluşturuluyor.
                    foreach (var role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role.RoleName));
                    }
                    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.LastName));
                    isAuthenticate = true;


                    // Kullanıcının doğrulanma durumu true ise, oturumu başlatmak için kimlik doğrulama işlemini gerçekleştirir
                    if (isAuthenticate)//true ise oturum ac
                    {
                        var principal = new ClaimsPrincipal(identity);//ClaimsPrincipal, kullanıcının kimlik bilgilerini tutar.
                        //Cookie vasıtasıyla oturum acılır.
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    }

                    var token = _authService.AuthLogin(new UserLoginRequest
                    {
                        Email = request.Email,
                        Password = request.Password,
                        Roles = roles.Select(r=>r.RoleName).ToList()
                    });
                    //Cookie'e token ekleniyor
                    HttpContext.Response.Cookies.Append("access_token", token.AuthToken, new CookieOptions
                    {
                        Expires = token.AccessTokenExpireDate,
                        HttpOnly = true,
                        Secure = false, // HTTPS kullanıyorsan true yap
                        SameSite = SameSiteMode.Lax // CSRF koruması için
                        
                    });
                    //Refresh token'ı cookie'ye ekliyoruz.
                    HttpContext.Response.Cookies.Append("refresh_token", token.RefreshToken, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(7), // Refresh token 7 gün geçerli
                        HttpOnly = true,
                        Secure = false, // HTTPS kullanıyorsan true yap
                        SameSite = SameSiteMode.Strict // CSRF koruması için
                    });
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.IncorrectPassword = "Yanlış şifre!";
                return View();
            }

            ViewBag.ErrorMessage = "Böyle bir kullanıcı bulunamadı!";
            return View();
        }

        
        [HttpPost]
        public IActionResult Refresh()
        {
            // Refresh token'ı cookie'den alıyoruz. Kullanıcı giriş yaparken eklenmişti
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized();
            }
            //access token'ın süresi dolmuşsa Client js ile api'ye istek atar ve 401 unauthorized hatası alır ve
            // refresh token'ı kullanarak yeni access token almak için bu metoda gelir.
            //Eğer refresh token veri tabanındaki ile uyuşuyorsa yeni bir access token ve refresh token oluşturulur.
            var user = _context.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpireDate < DateTime.UtcNow)
            {
                return Unauthorized();
            }
            // Kullanıcının rollerini al
            var userRoles = _context.UserRoles.Where(ur => ur.UserId == user.UserId).Select(ur=>ur.Role.RoleName).ToList();
            // Yeni access token ve refresh token oluştur
            var newAccessToken = _tokenService.GenerateToken(new GenerateTokenRequest { Email = user.Email, Roles = userRoles });


            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;//eski refreshi yeniyle değiştir
            user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(7);
            _context.SaveChanges();//database'e kaydet
            // Yeni access token cookie'ye eklendi.
            Response.Cookies.Append("access_token", newAccessToken.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newAccessToken.AccessTokenExpireDate
            });
            //Yeni refresh token cookie'ye eklendi.
            Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7),
            });
            

            return Ok(new{ 
                accessToken = newAccessToken.AccessToken,
                accessTokenExpireDate = newAccessToken.AccessTokenExpireDate
            });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return Content("Yetkisiz erişim!");
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(UserRequestDto request)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == request.Email );
            
            if (HashHelper.HashPassword(request.Password) == user.Password)
            {
                ClaimsIdentity identity = null;
                bool isAuthenticate = false;

                _context.UserRoles.Where(ur => ur.UserId == user.UserId);
                var roles = _context.Roles.Where(r => r.UserRoles.Any(ur => ur.UserId == user.UserId));
                identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                //Her role'de claim nesnesi oluşturuluyor.
                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role.RoleName));
                }
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
                isAuthenticate = true;


                var token = _authService.AuthLogin(new UserLoginRequest
                {
                    Email = request.Email,
                    Password = request.Password
                });

                

                if (isAuthenticate)
                {
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "Home");
                }

                HttpContext.Response.Cookies.Append("access_token", token.AuthToken, new CookieOptions
                {
                    Expires = token.AccessTokenExpireDate,
                    HttpOnly = true,
                    Secure = true, // HTTPS kullanıyorsanız true yapın
                    SameSite = SameSiteMode.Strict // CSRF koruması için
                });

                TempData["email"] = "Hoşgeldin Admin!";
                return RedirectToAction("AdminPanel", "Home");
            }
                return Content("You are not authorized to access this page.");
        }


        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public IActionResult AuthTryPage()
        {
            return View();
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(UserRequestDto request)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = HashHelper.HashPassword(request.Password)
                };
                _context.Add(user);
                _context.SaveChanges();
             
                return RedirectToAction("Login");
            }
            return View(request);
        }
        
    }
}
