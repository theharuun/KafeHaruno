namespace KafeHaruno.Models
{
    public class OrderViewModel
    {
        public int TableId { get; set; }
        public int UserId { get; set; }

        public int OrderPayment {  get; set; }
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
    }

}
