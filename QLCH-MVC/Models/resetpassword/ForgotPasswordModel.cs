using System.ComponentModel.DataAnnotations;
namespace QLCH_MVC.Models.resetpassword
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress]
        public string Email { get; set; }
    }
}