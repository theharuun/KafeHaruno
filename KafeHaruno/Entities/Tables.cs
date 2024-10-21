namespace KafeHaruno.Entities
{
    public class Tables
    {
        public int Id { get; set; }

        public bool IsFull { get; set; }  // A property indicating whether the table is full or not
        public ICollection<Order> Orders { get; set; } = new List<Order> { }; // Relationship: A table can receive multiple orders
        public ICollection<Bill> Bills { get; set; }

        // Current open invoice for the table (if any)
        public Bill CurrentBill => Bills?.FirstOrDefault(b => !b.IsPaid);

    }

}
