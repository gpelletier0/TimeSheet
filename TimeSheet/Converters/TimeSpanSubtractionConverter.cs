using System.Globalization;

namespace TimeSheet.Converters;

public class TimeSpanSubtractionConverter : IMultiValueConverter {
    public object Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture) {
        if (values is null ||
            values.Length < 2 ||
            values[0] is not TimeSpan endTime ||
            values[1] is not TimeSpan startTime) {
            return TimeSpan.Zero;
        }

        return endTime - startTime;
    }

    public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture) {
        return null;
    }
}