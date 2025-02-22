<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
namespace QLCH_MVC.Models.resetpassword
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress]
        public string Email { get; set; }
    }
=======
﻿using System.ComponentModel.DataAnnotations;
namespace QLCH_MVC.Models.resetpassword
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress]
        public string Email { get; set; }
    }
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
}