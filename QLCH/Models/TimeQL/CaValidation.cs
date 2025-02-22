<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;

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
=======
﻿using System.ComponentModel.DataAnnotations;

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
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
