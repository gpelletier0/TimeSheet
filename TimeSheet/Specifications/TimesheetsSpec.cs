using System.Diagnostics;
using TimeSheet.Extensions;
using TimeSheet.Interfaces;
using TimeSheet.Models;

namespace TimeSheet.Specifications;

public class TimesheetsSpec : ISpecification {
    public TimePeriod TimeFilter { get; set; } = TimePeriod.Month;
    public int ProjectId { get; set; }
    public int ClientId { get; set; }
    public HashSet<int> StatusIds { get; set; } = [];
    public bool Ascending { get; set; } = false;

    private static (DateTime startTime, DateTime endTime) GetDatePeriods(TimePeriod timeFilter) {
        return timeFilter switch {
            TimePeriod.Day => (DateTime.UtcNow.Date, DateTime.UtcNow.Date),
            TimePeriod.Week => DateTime.UtcNow.Date.WeekPeriod(),
            TimePeriod.Month => DateTime.UtcNow.Date.MonthPeriod(),
            TimePeriod.Year => DateTime.UtcNow.Date.YearPeriod(),
            _ => throw new ArgumentOutOfRangeException(nameof(timeFilter), timeFilter, null)
        };
    }

    private bool HasActiveFilters() => ProjectId > 0 || ClientId > 0 || StatusIds.Count > 0;

    public SqlQuery GetQuery() {
        var builder = new SqlQueryBuilder()
            .Select(
                "t.Id           AS Id",
                "t.Date         AS Date",
                "t.StartTime    AS StartTime",
                "t.EndTime      AS EndTime",
                "p.Name         AS ProjectName",
                "p.HourlyWage   AS ProjectHourlyWage",
                "s.ColorArgb    AS ColorArgb")
            .From("Timesheets t")
            .LeftJoin("Projects p", "t.ProjectId", "p.Id")
            .LeftJoin("Statuses s", "t.StatusId", "s.Id")
            .OrderBy("t.Date", Ascending);

        if (TimeFilter != TimePeriod.All) {
            try {
                var dates = GetDatePeriods(TimeFilter);
                builder.WhereBetween("t.Date", dates.startTime, dates.endTime);
            }
            catch (Exception e) {
                Debug.WriteLine(e);
            }
        }

        if (ProjectId > 0) {
            builder.Where("t.ProjectId", "=", ProjectId);
        }

        if (ClientId > 0) {
            builder.Join("Clients c", "p.ClientId", "c.Id");
            builder.Where("p.ClientId", "=", ClientId);
        }

        if (StatusIds.Count != 0) {
            builder.WhereIn("t.StatusId", StatusIds.Cast<object>());
        }

        return builder.ToQuery();
    }

    public string GetFilterNames() {
        if (!HasActiveFilters())
            return string.Empty;

        var activeFilters = new List<string>();

        if (ProjectId > 0) {
            activeFilters.Add("Project");
        }

        if (ClientId > 0) {
            activeFilters.Add("Client");
        }

        if (StatusIds.Count > 0) {
            activeFilters.Add("Status");
        }

        return $"Filters: {string.Join(" | ", activeFilters)}";
    }
}