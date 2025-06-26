
using Azure.Messaging;
using FirstProject.Models.Context;
using System.Text.Json;

namespace FirstProject.Models.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) {
            _context = context;
        }

        public void Add(Product product)
        {
            _context.Add(product);
            _context.SaveChanges();
        }

        public void Delete(int productId)
        {
            var product = _context.Products.SingleOrDefault(p => p.ProductId == productId);
            if (product != null) 
            _context.Remove(product);
            _context.SaveChanges();
        }

        public Product Get(int productId)
        {

            var product = _context.Products.SingleOrDefault(c => c.ProductId == productId);
            return product;
        }

        public List<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public void Update(int productId)
        {
            var product = _context.Products.SingleOrDefault(p => p.ProductId == productId);
            _context.Update(product);
            
        }
    }
}
