using System.Text.RegularExpressions;

namespace TimeSheet.Behaviors;

public partial class PhoneValidationBehavior : Behavior<Entry> {
    [GeneratedRegex(@"\D")]
    private static partial Regex DigitsOnlyRegEx();

    private bool _isFormatting;

    protected override void OnAttachedTo(Entry entry) {
        entry.TextChanged += OnEntryTextChanged;
        entry.Keyboard = Keyboard.Telephone;
        base.OnAttachedTo(entry);
    }

    protected override void OnDetachingFrom(Entry entry) {
        entry.TextChanged -= OnEntryTextChanged;
        base.OnDetachingFrom(entry);
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs e) {
        if (_isFormatting)
            return;

        var entry = (Entry)sender;
        _isFormatting = true;

        var formatted = Format(e.NewTextValue);
        entry.Text = formatted;

        _isFormatting = false;
    }

    private static string Format(string phoneNumber) {
        if (string.IsNullOrEmpty(phoneNumber))
            return string.Empty;

        var digitsOnly = DigitsOnlyRegEx().Replace(phoneNumber, string.Empty);
        return digitsOnly.Length switch {
            0 => string.Empty,
            <= 3 => digitsOnly,
            <= 6 => $"{digitsOnly[..3]}-{digitsOnly[3..]}",
            >= 10 => $"{digitsOnly[..3]}-{digitsOnly[3..6]}-{digitsOnly[6..10]}",
            _ => $"{digitsOnly[..3]}-{digitsOnly[3..6]}-{digitsOnly[6..]}"
        };
    }
}