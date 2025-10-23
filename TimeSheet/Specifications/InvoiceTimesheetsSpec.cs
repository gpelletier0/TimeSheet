using System.Diagnostics;
using TimeSheet.Extensions;
using TimeSheet.Interfaces;
using TimeSheet.Models;

namespace TimeSheet.Specifications;

public class InvoiceTimesheetsSpec : ISpecification {

    public TimePeriod TimeFilter { get; set; }
    public DateTime? StartDate { get; set; }
    public HashSet<int> ProjectIds { get; set; } = [];
    public bool Ascending { get; set; } = false;

    public SqlQuery GetQuery() {
        var builder = new SqlQueryBuilder()
            .Select(
                "t.Id",
                "t.Date",
                "t.StartTime",
                "t.EndTime",
                "p.Name         AS ProjectName",
                "p.HourlyWage   AS ProjectHourlyWage",
                "s.ColorArgb    AS ColorArgb")
            .From("Timesheets t")
            .LeftJoin("Projects p", "t.ProjectId", "p.Id")
            .LeftJoin("Statuses s", "t.StatusId", "s.Id")
            .OrderBy("t.Date", Ascending);

        if (TimeFilter != TimePeriod.All) {
            try {
                var dates = StartDate?.GetDatePeriods(TimeFilter) ?? DateTime.UtcNow.GetDatePeriods(TimeFilter);
                builder.WhereBetween("t.Date", dates.startTime, dates.endTime);
            }
            catch (Exception e) {
                Debug.WriteLine(e);
            }
        }

        if (ProjectIds.Count != 0) {
            builder.WhereIn("t.ProjectId", ProjectIds.Cast<object>());
        }

        return builder.ToQuery();
    }

    public string GetFilterNames() {
        return string.Empty;
    }
}