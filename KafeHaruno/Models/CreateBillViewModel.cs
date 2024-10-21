using KafeHaruno.Entities;

namespace KafeHaruno.Models
{
    public class CreateBillViewModel
    {
        public decimal BillPrice { get; set; }
        public bool IsPaid { get; set; }  // Indicates whether the invoice has been paid or not

        // Table to which the invoice belongs
        public int TableId { get; set; }
        public Tables Tables { get; set; }
    }
}
