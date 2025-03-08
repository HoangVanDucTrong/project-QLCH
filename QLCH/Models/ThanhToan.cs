
﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    public class ThanhToan
    {
        [Key]
        public int ThanhToanId { get; set; }

        public int DonHangId { get; set; }

        public DateTime NgayThanhToan { get; set; }

        [Required]
        public int SoTien { get; set; }

        [Required]
        [StringLength(40)]
        public string PhuongThucThanhToan { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        [ForeignKey("DonHangId")]
        public DonHang DonHang { get; set; }
    }
}
