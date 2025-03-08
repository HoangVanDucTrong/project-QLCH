
﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    [Table("ChiTietDonHangs")]
    public class ChiTietDonHang
    {
        [Key]
        public int CTDHId { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public string? ImageCheckBank { get; set; } // Ảnh hóa đơn lưu dưới dạng Base64

        [Required]
        public int StoreId { get; set; }

        [Required]
        public int BanId { get; set; }

        // Liên kết danh sách sản phẩm trong đơn hàng
        public virtual ICollection<SanPhamDonHang> SanPhamDonHangs { get; set; }
    }

}
