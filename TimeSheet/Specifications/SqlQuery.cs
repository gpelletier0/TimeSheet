namespace TimeSheet.Specifications;

public class SqlQuery {
    public required string Sql { get; init; }
    public object[]? Parameters { get; init; }
}