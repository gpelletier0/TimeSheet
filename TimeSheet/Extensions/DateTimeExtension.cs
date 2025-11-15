using TimeSheet.Models;

namespace TimeSheet.Extensions;

public static class DateTimeExtension {
    public static (DateTime startDate, DateTime endDate) WeekPeriod(this DateTime date) {
        var daysSinceMonday = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        var startDate = date.AddDays(-daysSinceMonday);
        var endDate = startDate.AddDays(6);
        return (startDate, endDate);
    }

    public static (DateTime startDate, DateTime endDate) MonthPeriod(this DateTime date) {
        var startDate = new DateTime(date.Year, date.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return (startDate, endDate);
    }

    public static (DateTime startDate, DateTime endDate) YearPeriod(this DateTime date) {
        var startDate = new DateTime(date.Year, 1, 1);
        var endDate = startDate.AddYears(1).AddDays(-1);
        return (startDate, endDate);
    }

    public static (DateTime startDate, DateTime endDate) GetDatePeriods(this DateTime startDate, TimePeriod timeFilter) {
        return timeFilter switch {
            TimePeriod.Day => (startDate.Date, startDate.Date),
            TimePeriod.Week => startDate.WeekPeriod(),
            TimePeriod.Month => startDate.MonthPeriod(),
            TimePeriod.Year => startDate.YearPeriod(),
            _ => throw new ArgumentOutOfRangeException(nameof(timeFilter), timeFilter, null)
        };
    }
}