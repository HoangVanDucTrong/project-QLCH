
﻿using QLCH.Models.TimeQL;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QLCH.Models
{
    [Table("CaLamNhanVien")]
    public class CaLamNhanVien
    {
        [Key]
        public int? CaLamId { get; set; }

        [Required]
        public int NVid { get; set; }
       
        public int? StoreId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [NgayLamValidation(ErrorMessage = "Ngày làm không được trước ngày hiện tại.")]
        public DateTime NgayLam { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [GioBatDauValidation(ErrorMessage = "Giờ bắt đầu và kết thúc không hợp lệ.")]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan GioKetThuc { get; set; }

        public string? GhiChu { get; set; }

        
      
        public string? Thu { get; set; }

        [Required]
        [CaValidation(ErrorMessage = "Ca Làm phải là Sáng,Chiều hoặc Tối")]
        public string calam { get; set; }
    }
}