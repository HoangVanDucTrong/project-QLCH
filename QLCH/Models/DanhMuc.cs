using System.ComponentModel.DataAnnotations;

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
