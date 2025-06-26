namespace FirstProject.Models.Repositories
{
    public interface IProductRepository
    {
        public void Add(Product product);
        public void Delete(int productId);
        public void Update(int productId);
        public Product Get(int productId);
        public List<Product> GetAll();

    }
}
