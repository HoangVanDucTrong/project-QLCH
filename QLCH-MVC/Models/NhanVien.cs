
﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class NhanVien
    {
        public int? NVid { get; set; }

        [StringLength(50)]
        public string TenNV { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có đúng 10 ký tự.")]
        public string SDT { get; set; }
        public string? AccountStatus { get; set; }
        public DateTime NgayVaoLam { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MucLuong { get; set; }

        public int? StoreId { get; set; }


        [Column(TypeName = "nvarchar(max)")]
        public string? AnhNhanVien { get; set; }
    }
}
