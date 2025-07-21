using FirstProject.Models;
using FirstProject.Models.Context;
using Microsoft.AspNetCore.Mvc;

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
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AdminPanel()
        {
            HttpContext.Session.GetString("email");
            var tempData = TempData["email"];
            return View(tempData);
        }
    }
}
