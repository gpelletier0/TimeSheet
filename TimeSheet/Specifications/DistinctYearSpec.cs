using TimeSheet.Interfaces;

namespace TimeSheet.Specifications;

public class DistinctTimeSpec : ISpecification {

    public required string ColumnName { get; init; }
    public required string TableName { get; init; }
    public required string FormatSpecifier { get; init; }

    public SqlQuery GetQuery() {
        var builder = new SqlQueryBuilder()
            .Select($"DISTINCT strftime('%{FormatSpecifier}', {ColumnName})")
            .From(TableName);

        return builder.ToQuery();
    }

    public string GetFilterNames() {
        return string.Empty;
    }
}