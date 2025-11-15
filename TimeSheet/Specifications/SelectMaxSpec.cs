using TimeSheet.Interfaces;

namespace TimeSheet.Specifications;

public class SelectMaxSpec : ISpecification {
    public required string ColumnName { get; init; }
    public required int Start { get; init; }
    public int? Length { get; init; }
    public required SqliteDataType DataType { get; init; }
    public required int Increment { get; init; }
    public required string TableName { get; init; }
    public required string Pattern { get; init; }

    public SqlQuery GetQuery() {
        var subStr = Length.HasValue 
            ? SqlFuncs.Substr(ColumnName, Start, Length.Value) 
            : SqlFuncs.Substr(ColumnName, Start);
        
        var builder = new SqlQueryBuilder()
            .SelectMax(SqlFuncs.Add(SqlFuncs.Cast(subStr, DataType), Increment))
            .From(TableName)
            .WhereLike(ColumnName, Pattern);
            
        return builder.ToQuery();
    }

    public string GetFilterNames() {
        return string.Empty;
    }
}