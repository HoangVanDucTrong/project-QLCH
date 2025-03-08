
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLCH.Models
{
    [Table("NhapHang")]
    public class NhapHang
    {
        [Key]
        public int? NHid { get; set; }
        public string TenHangHoa { get; set; }
        public string DonVi { get; set; }
        public decimal DonGia { get; set; }
        public int  soluong { get; set; }
        public decimal? ThanhTien { get; set; }
        public DateTime NgayNhapHang { get; set; }
        public int? StoreId { get; set; }

    }

}
