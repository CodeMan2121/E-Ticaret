using FirstProject.Models;
using FirstProject.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FirstProject.Controllers
{
    
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

        [HttpPost("AddProduct")]
        public IActionResult AddProduct(Product product)
        {
            _repository.Add(product);
            return RedirectToAction("GetAll");
        }


        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var product = _repository.Get(id);
            return View(product);
        }

        [HttpPost("Delete/{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.Delete(id);
            return RedirectToAction("GetAll");
        }


        [HttpGet("Update/{id}")]
        public IActionResult Update(int id)
        {
            var product = _repository.Get(id);
            return View(product);
        }

        [HttpPost("Update/{id}")]
        public IActionResult Update(Product product)
        {
            _repository.Update(product);
            return RedirectToAction("GetAll");
        }

        
        [HttpGet("Get/{id}")]
        public IActionResult Get(int id) { 
         var product = _repository.Get(id);
            return View(product);//bu int ile id'si belirtilen product, cshtml dosyasına gidip orada @model olarak tanımlanır.
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll() {
            var products = _repository.GetAll();
            return View(products);
        }
    }
}
