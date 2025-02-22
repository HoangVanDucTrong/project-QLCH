using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    public class QR
    {
        [Key]
        public int QRId { get; set; }

        public int? BanId { get; set; }

        [Required]
        [StringLength(30)]
        public string DuLieuMaQR { get; set; }

        [ForeignKey("BanId")]
        public Bans bans { get; set; }
    }
}
