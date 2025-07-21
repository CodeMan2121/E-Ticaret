using Azure;
using FirstProject.Models;
using FirstProject.Models.Context;
using FirstProject.Models.DTOs;
using FirstProject.Models.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

namespace FirstProject.Controllers
{
    
    public class AccountController : Controller
    {
        //public ApplicationDbContext _context = new(); DI kullanmazsak parametresiz const kullanmak zorunda kalacam.Bunun için de kendim OnConfiguring'de ayar çekmem gerekecek.
        // OnConfiguring metodunun yerine DbContextOptions'li const kullandık. İki parametre hataya sebep olabilir.

        private readonly ApplicationDbContext _context;
        private readonly IValidator<User> _validator;        

        public AccountController(ApplicationDbContext context, IValidator<User> validator)
        {
            _context = context;//DI, DbContextOptions ile otomatik bağlanır.Konfigürasyonlar otomatik yapılır.
            _validator = validator;
        }

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserRequestDto request)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);
            if (user != null)
            {
                
                if (user.Password == HashHelper.HashPassword(request.Password) )
                {
                    

                    HttpContext.Session.SetString("email", request.Email);
                    ViewBag.SuccessLogin = "Başarıyla giriş yapıldı!";
                    return RedirectToAction("Index", "Home");
                }
                
            }

            ViewBag.ErrorMessage = "Yanlış kullanıcı adı veya şifre";
            return View();
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
            
            if (user.Role == "Admin" && HashHelper.HashPassword(request.Password) == user.Password)
            {
                HttpContext.Session.SetString("email", request.Email);
                TempData["email"] = "Hoşgeldin Admin!";
                return RedirectToAction("AdminPanel", "Home");
            }
                return Content("You are not authorized to access this page.");
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
                request.Password = HashHelper.HashPassword(request.Password);
                _context.Add(request);
                _context.SaveChanges();
             
                return RedirectToAction("Login");
            }
            return View(request);
        }
        
    }
}
