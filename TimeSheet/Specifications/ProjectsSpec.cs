using TimeSheet.Interfaces;

namespace TimeSheet.Specifications;

public class ProjectsSpec : ISpecification {
    public string? Name { get; set; }
    public decimal? HourlyWage { get; set; }
    public int ClientId { get; set; }

    private bool HasActiveFilters() => !string.IsNullOrEmpty(Name) || HourlyWage is not null || ClientId > 0;

    public SqlQuery GetQuery() {
        var builder = new SqlQueryBuilder()
            .Select(
                "p.Id          AS Id",
                "p.Name        AS Name",
                "p.Description AS Description",
                "p.HourlyWage  AS HourlyWage",
                "c.Name        AS ClientName")
            .From("Projects p")
            .LeftJoin("Clients c", "p.ClientId", "c.Id")
            .OrderBy("p.Id");

        if (!string.IsNullOrEmpty(Name)) {
            builder.WhereLike("LOWER(p.Name)", $"%{Name.ToLower()}%");
        }

        if (HourlyWage.HasValue) {
            builder.WhereLike("CAST(p.HourlyWage AS TEXT)", $"{HourlyWage}%");
        }

        if (ClientId > 0) {
            builder.Where("p.ClientId", "=", ClientId);
        }

        return builder.ToQuery();
    }
    
    public string GetFilterNames() {
        if (!HasActiveFilters())
            return string.Empty;

        var activeFilters = new List<string>();

        if (!string.IsNullOrEmpty(Name)) {
            activeFilters.Add("Name");
        }

        if (HourlyWage is not null) {
            activeFilters.Add("Wage");
        }

        if (ClientId > 0) {
            activeFilters.Add("Client");
        }

        return $"Filters: {string.Join(" | ", activeFilters)}";
    }
}