<<<<<<< HEAD
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
=======
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
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
