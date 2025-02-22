<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QLCH.Models
{
    public class transaction
    {
        [Key]
        public int? TransactionId { get; set; }
        public int? StoreId { get; set; }
        public decimal Amount { get; set; }
        public string QRCodeUrl { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Note { get; set; }
    }
}
=======
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QLCH.Models
{
    public class transaction
    {
        [Key]
        public int? TransactionId { get; set; }
        public int? StoreId { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Note { get; set; }
    }
}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
