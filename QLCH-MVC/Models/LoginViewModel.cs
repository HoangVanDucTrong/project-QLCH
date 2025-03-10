
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
