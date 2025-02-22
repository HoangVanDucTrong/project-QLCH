<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    public class Client
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(30)]
        public string Username { get; set; }

        [Required]
        [StringLength(30)]
        public string Email { get; set; }

        [Required]
        [StringLength(30)]
        public string Pass { get; set; }
    }
}
=======
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH.Models
{
    public class Client
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(30)]
        public string Username { get; set; }

        [Required]
        [StringLength(30)]
        public string Email { get; set; }

        [Required]
        [StringLength(30)]
        public string Pass { get; set; }
    }
}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
