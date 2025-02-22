<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;

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
=======
﻿using System.ComponentModel.DataAnnotations;

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
        /*
        [Required]
        [StringLength(50)]
        public string BankAccount { get; set; }*/
    }
}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
