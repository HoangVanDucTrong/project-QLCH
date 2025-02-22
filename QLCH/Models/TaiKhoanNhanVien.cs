using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLCH.Models
{

    [Table("TaiKhoanNhanVien")]
    public class TaiKhoanNhanVien
    {
        [Key]
        public int? TaiKhoanId { get; set; }
        public int StoreId { get; set; }
        public int? NVid { get; set; }
        [Required]
        public  string TenDangNhap {get; set; }
        [Required]
        public string MatKhau { get; set; }
         public string? Sdt { get; set; }
        public string? DiaChi { get; set; }
        public string? QuocGia { get; set; }
    }
}
