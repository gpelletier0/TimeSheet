using System.Text;

namespace TimeSheet.Specifications {
    public class SqlQueryBuilder {
        private readonly List<string> _selectColumns = [];
        private string _tableName;
        private readonly List<WhereCondition> _whereConditions = [];
        private readonly List<JoinClause> _joins = [];
        private readonly List<string> _orderByColumns = [];
        private readonly List<string> _groupByColumns = [];
        private string _havingClause;
        private int? _limit;
        private int? _offset;
        private readonly List<object> _parameters = [];

        public SqlQueryBuilder Select(params string[] columns) {
            _selectColumns.AddRange(columns);
            return this;
        }

        public SqlQueryBuilder SelectAll() {
            _selectColumns.Clear();
            _selectColumns.Add("*");
            return this;
        }

        public SqlQueryBuilder SelectMax(string column, string? alias = null) {
            var maxExpr = alias != null 
                ? $"MAX({column}) AS {alias}" 
                : $"MAX({column})";
            
            _selectColumns.Add(maxExpr);
            return this;
        }
        
        public SqlQueryBuilder From(string tableName) {
            _tableName = tableName;
            return this;
        }

        public SqlQueryBuilder Where(string column, string op, object value) {
            _whereConditions.Add(new WhereCondition {
                Column = column,
                Operator = op,
                LogicalOperator = _whereConditions.Count == 0 ? "" : "AND"
            });
            _parameters.Add(value);
            return this;
        }

        public SqlQueryBuilder OrWhere(string column, string op, object value) {
            _whereConditions.Add(new WhereCondition {
                Column = column,
                Operator = op,
                LogicalOperator = "OR"
            });
            _parameters.Add(value);
            return this;
        }

        public SqlQueryBuilder WhereIn(string column, IEnumerable<object> values) {
            var valueList = values.ToList();
            var placeholders = string.Join(", ", Enumerable.Repeat("?", valueList.Count));

            _whereConditions.Add(new WhereCondition {
                Column = column,
                Operator = "IN",
                CustomClause = $"{column} IN ({placeholders})",
                LogicalOperator = _whereConditions.Count == 0 ? "" : "AND",
                IsCustom = true
            });

            _parameters.AddRange(valueList);
            return this;
        }

        public SqlQueryBuilder WhereBetween(string column, object start, object end) {
            _whereConditions.Add(new WhereCondition {
                Column = column,
                Operator = "BETWEEN",
                CustomClause = $"{column} BETWEEN ? AND ?",
                LogicalOperator = _whereConditions.Count == 0 ? "" : "AND",
                IsCustom = true
            });
            _parameters.Add(start);
            _parameters.Add(end);
            return this;
        }

        public SqlQueryBuilder WhereNull(string column) {
            _whereConditions.Add(new WhereCondition {
                Column = column,
                Operator = "IS NULL",
                LogicalOperator = _whereConditions.Count == 0 ? "" : "AND",
                IsNullCheck = true
            });
            return this;
        }

        public SqlQueryBuilder WhereNotNull(string column) {
            _whereConditions.Add(new WhereCondition {
                Column = column,
                Operator = "IS NOT NULL",
                LogicalOperator = _whereConditions.Count == 0 ? "" : "AND",
                IsNullCheck = true
            });
            return this;
        }

        public SqlQueryBuilder WhereLike(string column, string pattern) {
            _whereConditions.Add(new WhereCondition {
                Column = column,
                Operator = "LIKE",
                LogicalOperator = _whereConditions.Count == 0 ? "" : "AND"
            });
            _parameters.Add(pattern);
            return this;
        }

        public SqlQueryBuilder Join(string table, string leftColumn, string rightColumn, JoinType joinType = JoinType.Inner) {
            _joins.Add(new JoinClause {
                Table = table,
                LeftColumn = leftColumn,
                RightColumn = rightColumn,
                Type = joinType
            });
            return this;
        }

        public SqlQueryBuilder LeftJoin(string table, string leftColumn, string rightColumn) {
            return Join(table, leftColumn, rightColumn, JoinType.Left);
        }

        public SqlQueryBuilder RightJoin(string table, string leftColumn, string rightColumn) {
            return Join(table, leftColumn, rightColumn, JoinType.Right);
        }

        public SqlQueryBuilder OrderBy(string column, bool ascending = true) {
            _orderByColumns.Add($"{column} {(ascending ? "ASC" : "DESC")}");
            return this;
        }

        public SqlQueryBuilder GroupBy(params string[] columns) {
            _groupByColumns.AddRange(columns);
            return this;
        }

        public SqlQueryBuilder Having(string condition) {
            _havingClause = condition;
            return this;
        }

        public SqlQueryBuilder Limit(int limit) {
            _limit = limit;
            return this;
        }

        public SqlQueryBuilder Offset(int offset) {
            _offset = offset;
            return this;
        }

        private string Build() {
            var sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append(_selectColumns.Count > 0 ? string.Join(", ", _selectColumns) : "*");

            if (string.IsNullOrEmpty(_tableName))
                throw new InvalidOperationException("Table name must be specified using From()");

            sb.Append($" FROM {_tableName}");

            foreach (var join in _joins) {
                var joinTypeStr = join.Type switch {
                    JoinType.Inner => "INNER JOIN",
                    JoinType.Left => "LEFT JOIN",
                    JoinType.Right => "RIGHT JOIN",
                    JoinType.Full => "FULL OUTER JOIN",
                    _ => "INNER JOIN"
                };
                sb.Append($" {joinTypeStr} {join.Table} ON {join.LeftColumn} = {join.RightColumn}");
            }

            if (_whereConditions.Count > 0) {
                sb.Append(" WHERE ");
                for (var i = 0; i < _whereConditions.Count; i++) {
                    var condition = _whereConditions[i];
                    if (i > 0)
                        sb.Append($" {condition.LogicalOperator} ");

                    if (condition.IsCustom) {
                        sb.Append(condition.CustomClause);
                    }
                    else if (condition.IsNullCheck) {
                        sb.Append($"{condition.Column} {condition.Operator}");
                    }
                    else {
                        sb.Append($"{condition.Column} {condition.Operator} ?");
                    }
                }
            }

            if (_groupByColumns.Count > 0) {
                sb.Append($" GROUP BY {string.Join(", ", _groupByColumns)}");
            }

            if (!string.IsNullOrEmpty(_havingClause)) {
                sb.Append($" HAVING {_havingClause}");
            }

            if (_orderByColumns.Count > 0) {
                sb.Append($" ORDER BY {string.Join(", ", _orderByColumns)}");
            }

            if (_limit.HasValue) {
                sb.Append($" LIMIT {_limit.Value}");
            }

            if (_offset.HasValue) {
                sb.Append($" OFFSET {_offset.Value}");
            }

            return sb.ToString();
        }

        private object[] GetParameters() {
            return _parameters.ToArray();
        }

        public SqlQuery ToQuery() {
            return new SqlQuery {
                Sql = Build(),
                Parameters = GetParameters()
            };
        }

        private class WhereCondition {
            public required string Column { get; init; }
            public required string Operator { get; init; }
            public required string LogicalOperator { get; init; }
            public string? CustomClause { get; init; }
            public bool IsCustom { get; init; }
            public bool IsNullCheck { get; init; }
        }

        private class JoinClause {
            public required string Table { get; init; }
            public required string LeftColumn { get; init; }
            public required string RightColumn { get; init; }
            public JoinType Type { get; init; }
        }

        public enum JoinType {
            Inner,
            Left,
            Right,
            Full
        }
    }

}