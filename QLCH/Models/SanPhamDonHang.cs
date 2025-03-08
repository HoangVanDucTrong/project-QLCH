using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    [Table("SanPhamDonHang")]
    public class SanPhamDonHang
    {
        [Key]
        public int SPDHId { get; set; }

        [Required]
        public int CTDHId { get; set; } // Khóa ngoại liên kết `ChiTietDonHangs`

        [Required]
        public int SanPhamId { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [Required]
        public decimal Gia { get; set; }

        [Required]
        public decimal TongTien { get; set; } // Số lượng * Giá



        [ForeignKey("CTDHId")]
        public virtual ChiTietDonHang ChiTietDonHang { get; set; }
    }

}
