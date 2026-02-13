using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Validations
{
    public class FirstUpperCaseValidation: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null || String.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var valueToString = value.ToString()!;
            var firstChar = valueToString[0].ToString();

            if (firstChar != firstChar.ToUpper())
            {
                return new ValidationResult("Error, first char is not upper");
            }

            return ValidationResult.Success;
        }
    }
}
