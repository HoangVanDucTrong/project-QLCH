using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QLCH.Models
{
    public class transaction
    {
        [Key]
        public int? TransactionId { get; set; }
        public int? StoreId { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Note { get; set; }
    }
}
