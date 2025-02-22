using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    public class store
    {
        [Key]
        public int StoreId { get; set; }

        [Required]
        [StringLength(30)]
        public string Email { get; set; }

        [Required]
        [StringLength(10)]
        public string Sdt { get; set; }

        [Required]
        [StringLength(30)]
        public string Pass { get; set; }

        [Required]
        [StringLength(40)]
        public string DiaChi { get; set; }

        [Required]
        [StringLength(30)]
        public string QuocGia { get; set; }
       
    }
}
