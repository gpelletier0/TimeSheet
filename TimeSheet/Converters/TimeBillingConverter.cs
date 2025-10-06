using System.Globalization;

namespace TimeSheet.Converters;

public class TimeBillingConverter : IMultiValueConverter {
    public object Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture) {
        if (values is null ||
            values.Length < 3 ||
            values[0] is not TimeSpan endTime ||
            values[1] is not TimeSpan startTime ||
            values[2] is not decimal hourlyRate) {
            return 0m;
        }

        var duration = endTime - startTime;
        return (decimal)duration.TotalHours * hourlyRate;
    }

    public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture) {
        return null;
    }
}