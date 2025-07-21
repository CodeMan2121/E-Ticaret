
using Azure.Messaging;
using FirstProject.Models.Context;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
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
            //var product = _context.Products.Find(productId);
            var product = _context.Products.SingleOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }
            _context.Remove(product);
            _context.SaveChanges();
            
        }

        public void Update(Product product)
        {
            var existingProduct = _context.Products.SingleOrDefault(p => p.ProductId == product.ProductId);
            if (existingProduct != null)
            {
                existingProduct.ProductName = product.ProductName;
                existingProduct.Quantity = product.Quantity;
             //_context.Entry(existingProduct).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            }
        }

        public Product Get(int productId)
        {

            var product = _context.Products.SingleOrDefault(c => c.ProductId == productId);
            if (product!=null)
            {
                
            return product;
            }
            else
            {
                throw new ArgumentNullException("Product not found");
            }
            
            
        }

        public List<Product> GetAll()
        {
            return _context.Products.ToList();
        }

    }
}
