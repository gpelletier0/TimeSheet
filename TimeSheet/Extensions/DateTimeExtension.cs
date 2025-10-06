namespace TimeSheet.Extensions;

public static class DateTimeExtension {
    public static (DateTime startTime, DateTime endTime) WeekPeriod(this DateTime date) {
        var daysSinceMonday = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        var startDate = date.AddDays(-daysSinceMonday);
        var endDate = startDate.AddDays(6);
        return (startDate, endDate);
    }

    public static (DateTime startTime, DateTime endTime) MonthPeriod(this DateTime date) {
        var startDate = new DateTime(date.Year, date.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return (startDate, endDate);
    }

    public static (DateTime startTime, DateTime endTime) YearPeriod(this DateTime date) {
        var startDate = new DateTime(date.Year, 1, 1);
        var endDate = startDate.AddYears(1).AddDays(-1);
        return (startDate, endDate);
    }
}