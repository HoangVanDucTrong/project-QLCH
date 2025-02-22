<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email")]
     
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
=======
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email")]
     
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
