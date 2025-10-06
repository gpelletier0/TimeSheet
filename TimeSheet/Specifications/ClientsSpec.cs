using TimeSheet.Interfaces;

namespace TimeSheet.Specifications;

public class ClientsSpec : ISpecification {
    public string? Name { get; set; }
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }

    private bool HasActiveFilters() => !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(ContactName) || !string.IsNullOrEmpty(ContactPhone) || !string.IsNullOrEmpty(ContactEmail);

    public SqlQuery GetQuery() {
        var builder = new SqlQueryBuilder()
            .Select(
                "Id",
                "Name",
                "ContactName",
                "ContactPhone",
                "ContactEmail")
            .From("Clients")
            .OrderBy("Id");

        if (!string.IsNullOrEmpty(Name)) {
            builder.WhereLike("LOWER(Name)", $"%{Name.ToLower()}%");
        }

        if (!string.IsNullOrEmpty(ContactName)) {
            builder.WhereLike("LOWER(ContactName)", $"%{ContactName.ToLower()}%");
        }

        if (!string.IsNullOrEmpty(ContactPhone)) {
            builder.WhereLike("ContactPhone", $"%{ContactPhone}%");
        }

        if (!string.IsNullOrEmpty(ContactEmail)) {
            builder.WhereLike("LOWER(ContactEmail)", $"%{ContactEmail.ToLower()}%");
        }

        return builder.ToQuery();
    }

    public string GetFilterNames() {
        if (!HasActiveFilters()) {
            return string.Empty;
        }

        var activeFilters = new List<string>();

        if (!string.IsNullOrEmpty(Name)) {
            activeFilters.Add("Name");
        }

        if (!string.IsNullOrEmpty(ContactName)) {
            activeFilters.Add("Contact Name");
        }

        if (!string.IsNullOrEmpty(ContactPhone)) {
            activeFilters.Add("Contact Phone");
        }

        if (!string.IsNullOrEmpty(ContactEmail)) {
            activeFilters.Add("Contact Email");
        }

        return $"Filters: {string.Join(" | ", activeFilters)}";
    }
}