namespace QLCH_MVC.Models
{
    public class RevenueData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime? Date { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
