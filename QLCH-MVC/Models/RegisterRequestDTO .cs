<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email không hợp lệ.")]
        [EmailAddress]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải đúng 10 ký tự.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa các ký tự số.")]
        public string Sdt { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Địa chỉ phải có độ dài từ 5 đến 40 ký tự.")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Quốc gia là bắt buộc.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Tên quốc gia phải có độ dài từ 2 đến 30 ký tự.")]
        public string QuocGia { get; set; }

        /*
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải đúng 10 ký tự.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa các ký tự số.")]
        public string Sdt { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Địa chỉ phải có độ dài từ 5 đến 40 ký tự.")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Quốc gia là bắt buộc.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Tên quốc gia phải có độ dài từ 2 đến 30 ký tự.")]

        public string QuocGia { get; set; }
        public string? Roles { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }*/
    }

}
=======
﻿using System.ComponentModel.DataAnnotations;

namespace QLCH_MVC.Models
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email không hợp lệ.")]
        [EmailAddress]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải đúng 10 ký tự.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa các ký tự số.")]
        public string Sdt { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Địa chỉ phải có độ dài từ 5 đến 40 ký tự.")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Quốc gia là bắt buộc.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Tên quốc gia phải có độ dài từ 2 đến 30 ký tự.")]
        public string QuocGia { get; set; }

        /*
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải đúng 10 ký tự.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa các ký tự số.")]
        public string Sdt { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Địa chỉ phải có độ dài từ 5 đến 40 ký tự.")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Quốc gia là bắt buộc.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Tên quốc gia phải có độ dài từ 2 đến 30 ký tự.")]

        public string QuocGia { get; set; }
        public string? Roles { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }*/
    }

}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
