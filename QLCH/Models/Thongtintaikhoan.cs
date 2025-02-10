using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QLCH.Models
{
    public class Thongtintaikhoan
    {
        [Key]
        public int? bankid { get; set; }
        public string BankAccount { get; set; }
        public string QRCodeUrl { get; set; }
        public int? StoreId { get; set; }
    }
}
