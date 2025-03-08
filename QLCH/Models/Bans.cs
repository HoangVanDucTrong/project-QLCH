
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLCH.Models
{
    [Table("Bans")]
    public class Bans
    {
        [Key] 
        
        public int? BanId {  get; set; }
        public int SoBan {  get; set; }
        public string? IsInUse { get; set; }

        public int? StoreId { get; set; }
    }
}
