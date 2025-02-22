<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
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
=======
﻿using System.ComponentModel.DataAnnotations;
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
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
