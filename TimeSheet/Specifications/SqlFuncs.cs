namespace TimeSheet.Specifications;

public static class SqlFuncs {
    public static string Add(string expression, object value) {
        return $"{expression} + {value}";
    }

    public static string Cast(string expression, SqliteDataType dataType) {
        return $"CAST({expression} AS {dataType.ToString().ToUpper()})";
    }

    public static string Substr(string column, int start, int? length = null) {
        return length.HasValue
            ? $"SUBSTR({column}, {start}, {length.Value})"
            : $"SUBSTR({column}, {start})";
    }
}