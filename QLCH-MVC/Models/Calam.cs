
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
        public class Calam
        {
            public int? CaLamId { get; set; }

            [Required]
            public int NVid { get; set; }
            public string calam { get; set; }
            public string TenNhanVien { get; set; }
            public DateTime StartOfWeek { get; set; }
            public DateTime EndOfWeek { get; set; }
        
            public int? StoreId { get; set; }

            [Required]
            [DataType(DataType.Date)]
 
            public DateTime NgayLam { get; set; }

            [Required]
            [DataType(DataType.Time)]
     
            public TimeSpan GioBatDau { get; set; }

            [Required]
            [DataType(DataType.Time)]
            public TimeSpan GioKetThuc { get; set; }

            public string? GhiChu { get; set; }

            [Required]
            public string Thu { get; set; }
        }
}
