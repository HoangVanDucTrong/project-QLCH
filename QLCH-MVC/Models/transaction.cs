using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class transaction
    {
        [Key]
        public int? TransactionId { get; set; }
        public int? StoreId { get; set; }
        public decimal Amount { get; set; }
        public string QRCodeUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; }
    }
}
