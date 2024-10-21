using System.ComponentModel.DataAnnotations;

namespace KafeHaruno.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int OrderPayment { get; set; }

        public int UserId { get; set;  }
        public User User { get; set; }  // Relation to User

        public int TableId { get; set; }  // Specifies which table it belongs to
        public Tables Tables { get; set; }  // Relation to table

        public ICollection<OrderProduct> OrderProducts { get; set; } // Intermediate table relationship between Order and Product
    }

}
