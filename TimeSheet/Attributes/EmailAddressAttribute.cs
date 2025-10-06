using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TimeSheet.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public partial class EmailAddressAttribute : ValidationAttribute{

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegEx();
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
        var email = value?.ToString();
        
        return !string.IsNullOrEmpty(email) && !EmailRegEx().IsMatch(email)
            ? new ValidationResult("Invalid email.", [nameof(Email)])
            : ValidationResult.Success;
    }
}