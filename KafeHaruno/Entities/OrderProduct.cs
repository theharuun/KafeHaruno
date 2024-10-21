using System.ComponentModel.DataAnnotations;

namespace KafeHaruno.Entities
{
    public class OrderProduct
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }  // Sipariş edilen ürünün miktarı
    }

}
