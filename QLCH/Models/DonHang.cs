<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    public class DonHang
    {
        [Key]
        public int DonHangId { get; set; }

        public int UserId { get; set; }

        public DateTime NgayDatHang { get; set; }

        [StringLength(30)]
        public string TrangThai { get; set; }

        public int TongTien { get; set; }

        [ForeignKey("UserId")]
        public Client Client { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public ICollection<ThanhToan> ThanhToans { get; set; }
    }
}
=======
﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    public class DonHang
    {
        [Key]
        public int DonHangId { get; set; }

        public int UserId { get; set; }

        public DateTime NgayDatHang { get; set; }

        [StringLength(30)]
        public string TrangThai { get; set; }

        public int TongTien { get; set; }

        [ForeignKey("UserId")]
        public Client Client { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public ICollection<ThanhToan> ThanhToans { get; set; }
    }
}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
