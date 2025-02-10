using System;
using System.ComponentModel.DataAnnotations;

namespace QLCH.Models.TimeQL
{
    public class NgayLamValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CaLamNhanVien)validationContext.ObjectInstance;

            // Lấy ngày từ `NgayLam`
            var ngayLam = model.NgayLam;
            // Lấy thứ tương ứng từ `NgayLam`
            var actualDayOfWeek = GetThuFromDate(ngayLam);

            // So sánh `Thu` nhập vào với thứ thực tế
           if (!string.IsNullOrEmpty(model.Thu) && 
            !string.Equals(model.Thu, actualDayOfWeek, StringComparison.OrdinalIgnoreCase))
        {
            return new ValidationResult($"Thứ không khớp với ngày {ngayLam:dd/MM/yyyy}. Ngày này phải là {actualDayOfWeek}.");
        }


            return ValidationResult.Success;
        }

        private string GetThuFromDate(DateTime date)
        {
            var thuArray = new[] { "Chủ nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7" };
            return thuArray[(int)date.DayOfWeek];
        }
    }
}
