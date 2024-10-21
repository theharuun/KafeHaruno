namespace KafeHaruno.Entities
{
    public class Tables
    {
        public int Id { get; set; }

        public bool IsFull { get; set; }  // Masanın dolu olup olmadığını belirten bir özellik
        public ICollection<Order> Orders { get; set; } = new List<Order> { }; // İlişki: Bir masa birden fazla sipariş alabilir
        public ICollection<Bill> Bills { get; set; }

        // Masaya ait şu anki açık fatura (eğer varsa)
        public Bill CurrentBill => Bills?.FirstOrDefault(b => !b.IsPaid);

    }

}
