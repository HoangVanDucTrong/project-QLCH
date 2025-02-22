using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class CalamViewModel
    {
        [Required]
        public int NVid { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayLam { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan GioKetThuc { get; set; }

        [Required]
        public string calam { get; set; }

        public string? GhiChu { get; set; }
    }

}
