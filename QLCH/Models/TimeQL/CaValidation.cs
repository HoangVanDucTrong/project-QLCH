using System.ComponentModel.DataAnnotations;

namespace QLCH.Models.TimeQL
{
    public class CaValidation: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string calam = value as string;
            var validDays = new[] { "Sáng", "Chiều", "Tối" };

            if (!Array.Exists(validDays, day => day.Equals(calam, StringComparison.OrdinalIgnoreCase)))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
