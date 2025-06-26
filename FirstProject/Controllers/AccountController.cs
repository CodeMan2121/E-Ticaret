using FirstProject.Models;
using FirstProject.Models.Context;
using Microsoft.AspNetCore.Mvc;

namespace FirstProject.Controllers
{
    
    public class AccountController : Controller
    {
        //public ApplicationDbContext _context = new(); DI kullanmazsak parametresiz const kullanmak zorunda kalacam.Bunun için de kendim OnConfiguring'de ayar çekmem gerekecek.
        // OnConfiguring metodunun yerine DbContextOptions'li const kullandık. İki parametre hataya sebep olabilir.

        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;//DI, DbContextOptions ile otomatik bağlanır.Konfigürasyonlar otomatik yapılır.
        }

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                _context.SaveChanges();
             
                return RedirectToAction("Login");
            }
            return View(user);
        }
    }
}
