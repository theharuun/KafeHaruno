using KafeHaruno.Entities;

namespace KafeHaruno.Models
{
    public class CreateBillViewModel
    {
        public decimal BillPrice { get; set; }
        public bool IsPaid { get; set; }  // Faturanın ödenip ödenmediğini gösterir

        // Faturanın ait olduğu masa
        public int TableId { get; set; }
        public Tables Tables { get; set; }
    }
}
