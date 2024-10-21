using System.ComponentModel.DataAnnotations;

namespace KafeHaruno.Entities
{
    public class Product
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product Name is Required!!!")]
        [MaxLength(100, ErrorMessage = "Max lenght 100")]
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public long? Price { get; set; }

        public int? ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; } // Intermediate table relationship between Product and Order
    }
}

