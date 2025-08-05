using FirstProject.Models;
using FirstProject.Models.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FirstProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Logout()
        {
            var userEmail = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpireDate = null;
                _context.SaveChanges();
            }
            //HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
            HttpContext.Response.Cookies.Delete("access_token");
            HttpContext.Response.Cookies.Delete("refresh_token");
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles ="Admin")]
        public IActionResult AdminPanel()
        {
            //HttpContext.Session.GetString("email");
            ViewBag.HgAdmin = TempData["email"];//from AccountController
            return View();
        }
    }
}
