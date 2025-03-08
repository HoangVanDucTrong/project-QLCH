
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class Ban
    {
        [Key]

        public int? BanId { get; set; }
        public int SoBan { get; set; }

        public string? IsInUse { get; set; }
    }
}
