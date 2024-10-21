using System.ComponentModel.DataAnnotations;

namespace KafeHaruno.Entities
{
    public class ProductType
    {
        [Key]
        public int Id { get; set; }
        public string ProductTypeName { get; set; }

        // Product ile ilişki
        public ICollection<Product> Products { get; set; } = new List<Product>(); // Bir ürün türünde birden fazla ürün olabilir
    }

}
