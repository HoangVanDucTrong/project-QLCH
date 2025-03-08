
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class TaiKhoanNhanVien
    {
        [Key]
        public int? TaiKhoanId { get; set; }
        public int? NVid { get; set; }
        [Required]
        public string TenDangNhap { get; set; }
        [Required]
        public string MatKhau { get; set; }
    }
}
