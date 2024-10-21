using System.ComponentModel.DataAnnotations;

namespace KafeHaruno.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int OrderPayment { get; set; }

        public int UserId { get; set;  }
        public User User { get; set; }

        public int TableId { get; set; }  // Hangi masaya ait olduğunu belirtir
        public Tables Tables { get; set; }  // Masa ile ilişki

        public ICollection<OrderProduct> OrderProducts { get; set; }  // Order ile Product arasındaki ara tablo ilişkisi
    }

}
