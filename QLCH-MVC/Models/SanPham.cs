<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class SanPham
    {
        [Key]
        public int? SanPhamId { get; set; }
        [Required]
        public string Ten { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Giá không được âm.")]
        public int Gia { get; set; }
      //  public string GiaHienThi { get; set; }
        [Required]
        [StringLength(30)]
        public string MoTa { get; set; }

        public string? ImageBase64 { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn danh mục sản phẩm.")]
        public int DanhMucId { get; set; }
        public int? StoreId { get; set; }
   
    }
}
=======
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class SanPham
    {
        [Key]
        public int? SanPhamId { get; set; }
        [Required]
        public string Ten { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Giá không được âm.")]
        public int Gia { get; set; }
        public string GiaHienThi { get; set; }
        [Required]
        [StringLength(30)]
        public string MoTa { get; set; }

        public string? ImageBase64 { get; set; }
        public int DanhMucId { get; set; }
        public int? StoreId { get; set; }
   
    }
}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
