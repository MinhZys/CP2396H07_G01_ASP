using System;
using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models.Validations
{
    public class MinAgeAttribute : ValidationAttribute
    {
        private readonly int _minAge;

        public MinAgeAttribute(int minAge)
        {
            _minAge = minAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Ngày sinh là bắt buộc");

            if (value is not DateTime dob)
                return new ValidationResult("Ngày sinh không hợp lệ");

            var today = DateTime.Today;
            var age = today.Year - dob.Year;

            if (dob.Date > today.AddYears(-age))
                age--;

            if (age < _minAge)
                return new ValidationResult($"Bạn phải đủ {_minAge} tuổi trở lên");

            return ValidationResult.Success;
        }
    }
}
