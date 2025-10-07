using System.Globalization;

namespace TimeSheet.Controls;

public partial class NumericEntry : Entry {
    public static readonly BindableProperty AllowDecimalProperty =
        BindableProperty.Create(nameof(AllowDecimal), typeof(bool), typeof(NumericEntry), true);

    public static readonly BindableProperty AllowNegativeProperty =
        BindableProperty.Create(nameof(AllowNegative), typeof(bool), typeof(NumericEntry), true);

    public static readonly BindableProperty MaximumDecimalPlacesProperty =
        BindableProperty.Create(nameof(MaximumDecimalPlaces), typeof(int), typeof(NumericEntry), 2,
            validateValue: ValidateMaximumDecimalPlaces, propertyChanged: OnMaximumDecimalPlacesChanged);

    public static readonly BindableProperty NumericValueProperty =
        BindableProperty.Create(nameof(NumericValue), typeof(double?), typeof(NumericEntry), null,
            BindingMode.TwoWay, propertyChanged: OnNumericValueChanged);

    public static readonly BindableProperty AlwaysShowDecimalPlacesProperty =
        BindableProperty.Create(nameof(AlwaysShowDecimalPlaces), typeof(bool), typeof(NumericEntry), true);

    public static readonly BindableProperty FormatOnFocusLostProperty =
        BindableProperty.Create(nameof(FormatOnFocusLost), typeof(bool), typeof(NumericEntry), true);

    private const char DecimalSeparator = '.';
    private const char NegativeSign = '-';
    private readonly HashSet<char> _allowedChars = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

    private bool _isFormatting;
    private string _previousText = string.Empty;

    public bool AllowDecimal {
        get => (bool)GetValue(AllowDecimalProperty);
        set => SetValue(AllowDecimalProperty, value);
    }

    public bool AllowNegative {
        get => (bool)GetValue(AllowNegativeProperty);
        set => SetValue(AllowNegativeProperty, value);
    }

    public int MaximumDecimalPlaces {
        get => (int)GetValue(MaximumDecimalPlacesProperty);
        set => SetValue(MaximumDecimalPlacesProperty, value);
    }

    public double? NumericValue {
        get => (double?)GetValue(NumericValueProperty);
        set => SetValue(NumericValueProperty, value);
    }

    public bool AlwaysShowDecimalPlaces {
        get => (bool)GetValue(AlwaysShowDecimalPlacesProperty);
        set => SetValue(AlwaysShowDecimalPlacesProperty, value);
    }

    public bool FormatOnFocusLost {
        get => (bool)GetValue(FormatOnFocusLostProperty);
        set => SetValue(FormatOnFocusLostProperty, value);
    }

    public NumericEntry() {
        InitializeComponent();

        if (AllowDecimal) {
            _allowedChars.Add(DecimalSeparator);
        }

        if (AllowNegative) {
            _allowedChars.Add(NegativeSign);
        }
    }

    private static bool ValidateMaximumDecimalPlaces(BindableObject bindable, object value) {
        return value is >= 0;
    }

    private static void OnMaximumDecimalPlacesChanged(BindableObject bindable, object oldValue, object newValue) {
        var numericEntry = (NumericEntry)bindable;
        numericEntry.FormatDisplayText();
    }

    private void OnUnfocused(object sender, FocusEventArgs e) {
        if (FormatOnFocusLost && AlwaysShowDecimalPlaces) {
            FormatDisplayText();
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e) {
        if (_isFormatting)
            return;

        var newText = e.NewTextValue;
        _previousText = e.OldTextValue ?? string.Empty;

        if (string.IsNullOrEmpty(newText)) {
            NumericValue = null;
            return;
        }

        var filteredText = new string(newText.Where(_allowedChars.Contains).ToArray());
        if (!IsValidNumericFormat(filteredText)) {
            filteredText = _previousText;
        }

        filteredText = EnforceDecimalPlaces(filteredText);

        NumericValue = double.TryParse(filteredText, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out var result)
            ? result
            : null;

        UpdateTextIfChanged(filteredText);
    }

    private void UpdateTextIfChanged(string newText) {
        if (Text == newText) {
            return;
        }

        _isFormatting = true;
        Text = newText;
        _isFormatting = false;
    }

    private string EnforceDecimalPlaces(string text) {
        if (string.IsNullOrEmpty(text) || !text.Contains(DecimalSeparator)) {
            return text;
        }

        var decimalIndex = text.IndexOf(DecimalSeparator);
        var decimalPartLength = text.Length - decimalIndex - 1;

        if (decimalPartLength > MaximumDecimalPlaces) {
            return text[..(decimalIndex + 1 + MaximumDecimalPlaces)];
        }

        return text;
    }

    private void FormatDisplayText() {
        if (!AlwaysShowDecimalPlaces || !NumericValue.HasValue || MaximumDecimalPlaces == 0) {
            return;
        }

        var formattedText = FormatNumber(NumericValue.Value);
        UpdateTextIfChanged(formattedText);
    }

    private string FormatNumber(double value) {
        if (MaximumDecimalPlaces == 0) {
            return value.ToString("0", CultureInfo.InvariantCulture);
        }

        var format = $"0.{new string('0', MaximumDecimalPlaces)}";
        return value.ToString(format, CultureInfo.InvariantCulture);
    }

    private bool IsValidNumericFormat(string text) {
        if (string.IsNullOrEmpty(text)) {
            return true;
        }

        if (text.Count(c => c == DecimalSeparator) > 1) {
            return false;
        }

        if (text.Count(c => c == NegativeSign) > 1) {
            return false;
        }

        var negativeIndex = text.IndexOf(NegativeSign);
        if (negativeIndex > 0) {
            return false;
        }

        if (text.Contains(DecimalSeparator) && (!AllowDecimal || MaximumDecimalPlaces == 0)) {
            return false;
        }

        if (negativeIndex >= 0 && !AllowNegative) {
            return false;
        }

        return true;
    }

    private static void OnNumericValueChanged(BindableObject bindable, object oldValue, object newValue) {
        var numericEntry = (NumericEntry)bindable;

        if (newValue is double numericValue) {
            var formattedText = numericEntry.AlwaysShowDecimalPlaces
                ? numericEntry.FormatNumber(numericValue)
                : numericValue.ToString(CultureInfo.InvariantCulture);

            if (numericEntry.Text == formattedText || numericEntry.IsFocused) {
                return;
            }

            numericEntry.UpdateTextIfChanged(formattedText);
        }
        else {
            numericEntry.Text = string.Empty;
        }
    }

    protected override void OnHandlerChanged() {
        base.OnHandlerChanged();

        if (Handler != null && AlwaysShowDecimalPlaces && NumericValue.HasValue)
            Dispatcher.Dispatch(FormatDisplayText);
    }

    public int GetCurrentDecimalPlaces() {
        if (string.IsNullOrEmpty(Text) || !Text.Contains(DecimalSeparator)) {
            return 0;
        }

        var decimalIndex = Text.IndexOf(DecimalSeparator);
        return Text.Length - decimalIndex - 1;
    }

    public void RoundToMaximumDecimalPlaces() {
        if (!NumericValue.HasValue)
            return;

        var roundedValue = Math.Round(NumericValue.Value, MaximumDecimalPlaces, MidpointRounding.AwayFromZero);
        NumericValue = roundedValue;
        FormatDisplayText();
    }

    public void ApplyFormatting() {
        FormatDisplayText();
    }
}