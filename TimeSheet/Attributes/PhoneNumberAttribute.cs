using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TimeSheet.Attributes {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public partial class PhoneNumberAttribute : ValidationAttribute {

        [GeneratedRegex(@"\D")]
        private static partial Regex DigitsOnlyRegEx();

        [GeneratedRegex(@"^\d{3}-\d{3}-\d{4}$")]
        private static partial Regex PhoneNumberRegEx();

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
            var phoneNumber = value?.ToString();

            if (string.IsNullOrEmpty(phoneNumber)) {
                return ValidationResult.Success;
            }

            var digitsOnly = DigitsOnlyRegEx().Replace(phoneNumber, string.Empty);
            
            if (digitsOnly.Length == 0) 
                return ValidationResult.Success;
            
            if (digitsOnly.Length is < 10 or > 10) {
                return new ValidationResult("Phone number must be 10 digits.");
            }

            if (!PhoneNumberRegEx().IsMatch(phoneNumber)) {
                return new ValidationResult("Phone number must be in a 123-456-7890 format.");
            }

            return ValidationResult.Success;
        }
    }
}