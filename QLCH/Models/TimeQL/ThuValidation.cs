using System;
using System.ComponentModel.DataAnnotations;

public class ThuValidation : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string thu = value as string;
        var validDays = new[] { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" };

        if (!Array.Exists(validDays, day => day.Equals(thu, StringComparison.OrdinalIgnoreCase)))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
