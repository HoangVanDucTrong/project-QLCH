using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace QLCH.Models
{
    public class ThongTinTaiKhoanViewModel : IValidatableObject
    {
        public int? bankid { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngân hàng.")]
        public int AcqId { get; set; } // Mã ngân hàng từ API VietQR

   
        [Required(ErrorMessage = "Tên tài khoản không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên tài khoản không được dài quá 100 ký tự.")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Số tài khoản không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Số tài khoản phải có ít nhất 2 ký tự.")]
        public string BankAccount { get; set; }

        /// <summary>
        /// Custom Validation để kiểm tra và chuẩn hóa dữ liệu
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            // ✅ Kiểm tra & Chuyển đổi AccountName thành chữ hoa không dấu
            if (!string.IsNullOrEmpty(AccountName))
            {
                AccountName = ConvertToUnSignUpperCase(AccountName);
                if (!Regex.IsMatch(AccountName, "^[A-Z ]+$"))
                {
                    errors.Add(new ValidationResult("Tên tài khoản chỉ được chứa chữ hoa không dấu và khoảng trắng.", new[] { nameof(AccountName) }));
                }
            }

            // ✅ Kiểm tra số tài khoản có ít nhất 2 ký tự
            if (!string.IsNullOrEmpty(BankAccount) && BankAccount.Length < 2)
            {
                errors.Add(new ValidationResult("Số tài khoản phải có ít nhất 2 ký tự.", new[] { nameof(BankAccount) }));
            }

            return errors;
        }

        /// <summary>
        /// Hàm loại bỏ dấu tiếng Việt và chuyển thành chữ in hoa
        /// </summary>
        private string ConvertToUnSignUpperCase(string text)
        {
            text = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in text)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().ToUpper();
        }
    }
}
