using TimeSheet.Interfaces;

namespace TimeSheet.Specifications;

public class InvoiceTimesheetsSpec : ISpecification {

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
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

        if (StartDate.HasValue && EndDate.HasValue) {
            builder.WhereBetween("t.Date", StartDate.Value, EndDate.Value);
        }

        if (ProjectIds.Count != 0) {
            builder.WhereIn("t.ProjectId", ProjectIds.Cast<object>());
        }

        if (Ascending) {
            builder.OrderBy("t.Date", Ascending);
        }

        return builder.ToQuery();
    }

    public string GetFilterNames() {
        return string.Empty;
    }
}