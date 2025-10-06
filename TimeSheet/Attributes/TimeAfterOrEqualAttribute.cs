using System.ComponentModel.DataAnnotations;

namespace TimeSheet.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TimeAfterOrEqualAttribute : ValidationAttribute {
    private readonly string _comparisonProperty;

    public TimeAfterOrEqualAttribute(string comparisonProperty, string? errorMessage = null) {
        _comparisonProperty = comparisonProperty;
        ErrorMessage = errorMessage ?? "{0} must equal or after {1}";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
        if (value == null)
            return ValidationResult.Success;

        var currentValue = (TimeSpan)value;
        var property = validationContext.ObjectType.GetProperty(_comparisonProperty) ?? throw new ArgumentException($"Property {_comparisonProperty} not found");
        var comparisonValue = property.GetValue(validationContext.ObjectInstance);

        if (comparisonValue == null)
            return ValidationResult.Success;

        var isAfterOrEqual = currentValue >= (TimeSpan)comparisonValue;
        return isAfterOrEqual
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName), [validationContext.MemberName ?? string.Empty]);
    }

    public override string FormatErrorMessage(string name) {
        return string.Format(ErrorMessage ?? ErrorMessageString, name, _comparisonProperty);
    }
}