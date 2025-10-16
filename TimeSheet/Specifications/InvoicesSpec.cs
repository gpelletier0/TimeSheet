using TimeSheet.Interfaces;
using TimeSheet.Models.Entities;

namespace TimeSheet.Specifications;

public class InvoicesSpec : ISpecification {

    public string? Number { get; set; }
    public int ClientId { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public HashSet<int> StatusIds { get; set; } = [];

    private bool HasActiveFilters() => !string.IsNullOrEmpty(Number) ||
                                       ClientId > 0 ||
                                       IssueDate.HasValue ||
                                       DueDate.HasValue ||
                                       StatusIds.Count > 0;

    public SqlQuery GetQuery() {
        var builder = new SqlQueryBuilder()
            .Select(
                "i.Number",
                "c.Name         AS ClientName",
                "i.IssueDate",
                "i.DueDate",
                "s.ColorArgb")
            .From("Invoices i")
            .LeftJoin("Clients c", "i.ClientId", "c.Id")
            .LeftJoin("Statuses s", "i.StatusId", "s.Id")
            .OrderBy(nameof(Invoice.IssueDate), false);

        if (!string.IsNullOrEmpty(Number)) {
            builder.WhereLike("LOWER(i.Number)", $"%{Number.ToLower()}%");
        }

        if (ClientId > 0) {
            builder.Where("i.ClientId", "=", ClientId);
        }

        if (IssueDate.HasValue) {
            builder.Where("i.IssueDate", "=", IssueDate.Value.ToString("yyyy-MM-dd"));
        }

        if (DueDate.HasValue) {
            builder.Where("i.DueDate", "=", DueDate.Value.ToString("yyyy-MM-dd"));
        }

        if (StatusIds.Count > 0) {
            builder.WhereIn("i.StatusId", StatusIds.Cast<object>());
        }

        return builder.ToQuery();
    }

    public string GetFilterNames() {
        if (!HasActiveFilters())
            return string.Empty;

        var activeFilters = new List<string>();

        if (!string.IsNullOrEmpty(Number)) {
            activeFilters.Add("Number");
        }

        if (IssueDate.HasValue) {
            activeFilters.Add("Issue Date");
        }

        if (DueDate.HasValue) {
            activeFilters.Add("Due Date");
        }

        if (StatusIds.Count > 0) {
            activeFilters.Add("Status");
        }

        return $"Filters: {string.Join(" | ", activeFilters)}";
    }
}