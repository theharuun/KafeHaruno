using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KafeHaruno.Entities
{
    public class Bill
    {
        public int Id { get; set; }
        public decimal BillPrice { get; set; }
        public bool IsPaid { get; set; }  // Faturanın ödenip ödenmediğini gösterir

        // Faturanın ait olduğu masa
        public int TableId { get; set; }
        public Tables Tables { get; set; }

    }

}
