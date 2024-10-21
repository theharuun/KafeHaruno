using System.ComponentModel.DataAnnotations;

namespace KafeHaruno.Entities
{
    public class ProductType
    {
        [Key]
        public int Id { get; set; }
        public string ProductTypeName { get; set; }

        // Relationship with Product
        public ICollection<Product> Products { get; set; } = new List<Product>(); // There can be more than one product in a product type
    }

}
