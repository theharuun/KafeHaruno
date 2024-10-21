using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KafeHaruno.Entities
{
    public class Bill
    {
        public int Id { get; set; }
        public decimal BillPrice { get; set; }
        public bool IsPaid { get; set; } // Indicates whether the invoice has been paid or not

        // Table to which the invoice belongs
        public int TableId { get; set; }
        public Tables Tables { get; set; }

    }

}
