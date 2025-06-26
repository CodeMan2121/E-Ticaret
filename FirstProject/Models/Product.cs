using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public float Quantity { get; set; }
    }
}
