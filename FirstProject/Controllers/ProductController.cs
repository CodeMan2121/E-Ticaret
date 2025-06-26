using FirstProject.Models;
using FirstProject.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FirstProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("AddProduct")]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            _repository.Add(product);
            return View();
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);
            return View();
        }

        [HttpGet("Get")]
        public IActionResult Get(int id) { 
            _repository.Get(id);
            return View();
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll() {
            var products = _repository.GetAll();
            return View(products);
        }
    }
}
