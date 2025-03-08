
﻿using QLCH.Models;
using System;
using System.ComponentModel.DataAnnotations;

public class GioBatDauValidation : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var caLamNhanVien = (CaLamNhanVien)validationContext.ObjectInstance;
        if (caLamNhanVien.GioBatDau >= caLamNhanVien.GioKetThuc)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
