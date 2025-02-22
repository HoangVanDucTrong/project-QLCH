<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    public class DanhMuc
    {
        [Key]
        public int? DanhMucId { get; set; }

        [Required]
        [StringLength(30)]
        public string TenDanhMuc { get; set; }

        [Required]
        [StringLength(30)]
        public string MoTa { get; set; }
        
        public ICollection<SanPham>? SanPhams { get; set; }
    }
}
=======
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    public class DanhMuc
    {
        [Key]
        public int? DanhMucId { get; set; }

        [Required]
        [StringLength(30)]
        public string TenDanhMuc { get; set; }

        [Required]
        [StringLength(30)]
        public string MoTa { get; set; }
        
        public ICollection<SanPham>? SanPhams { get; set; }
    }
}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
