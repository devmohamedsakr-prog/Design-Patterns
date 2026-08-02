using System;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Database.SqlQuery.Context
{
    /// <summary>
    /// Product: Immutable SQL query constructed via builder.
    /// Demonstrates: Safe SQL construction with parameterized queries to prevent SQL injection.
    /// </summary>
    public class SqlQuery
    {
        public string Query { get; }
        public IReadOnlyDictionary<string, object> Parameters { get; }
        public string CommandType { get; } // Text, StoredProcedure
        public int CommandTimeout { get; }
        public bool IsSelectQuery { get; }
        public IReadOnlyList<string> SelectedColumns { get; }
        public IReadOnlyList<string> Joins { get; }
        public IReadOnlyList<string> WhereConditions { get; }
        public IReadOnlyList<string> OrderByColumns { get; }
        public int? LimitRows { get; }
        public int? OffsetRows { get; }

        private SqlQuery(
            string query,
            IReadOnlyDictionary<string, object> parameters,
            string commandType,
            int commandTimeout,
            bool isSelectQuery,
            IReadOnlyList<string> selectedColumns,
            IReadOnlyList<string> joins,
            IReadOnlyList<string> whereConditions,
            IReadOnlyList<string> orderByColumns,
            int? limitRows,
            int? offsetRows)
        {
            Query = query;
            Parameters = parameters;
            CommandType = commandType;
            CommandTimeout = commandTimeout;
            IsSelectQuery = isSelectQuery;
            SelectedColumns = selectedColumns;
            Joins = joins;
            WhereConditions = whereConditions;
            OrderByColumns = orderByColumns;
            LimitRows = limitRows;
            OffsetRows = offsetRows;
        }

        public static SqlQueryBuilder Select => new SqlQueryBuilder("SELECT");
        public static SqlQueryBuilder Insert => new SqlQueryBuilder("INSERT");
        public static SqlQueryBuilder Update => new SqlQueryBuilder("UPDATE");
        public static SqlQueryBuilder Delete => new SqlQueryBuilder("DELETE");

        public override string ToString()
        {
            return $"SqlQuery(Type={CommandType}, Timeout={CommandTimeout}, " +
                   $"Params={Parameters.Count}, Limit={LimitRows}, Offset={OffsetRows})";
        }

        /// <summary>
        /// Builder class: Fluent API for constructing SqlQuery.
        /// </summary>
        public class SqlQueryBuilder
        {
            private readonly string _baseCommand;
            private string _query;
            private readonly Dictionary<string, object> _parameters = new();
            private string _commandType = "Text";
            private int _commandTimeout = 30;
            private readonly List<string> _selectedColumns = new();
            private readonly List<string> _joins = new();
            private readonly List<string> _whereConditions = new();
            private readonly List<string> _orderByColumns = new();
            private int? _limitRows;
            private int? _offsetRows;
            private string _fromTable;
            private string _whereClause;
            private string _joinClause;

            public SqlQueryBuilder(string baseCommand)
            {
                _baseCommand = baseCommand;
            }

            /// <summary>
            /// For SELECT: specify columns.
            /// </summary>
            public SqlQueryBuilder Columns(params string[] columns)
            {
                if (columns == null || columns.Length == 0)
                    throw new ArgumentException("At least one column must be specified", nameof(columns));
                foreach (var col in columns)
                    _selectedColumns.Add(col);
                return this;
            }

            /// <summary>
            /// Specify FROM table.
            /// </summary>
            public SqlQueryBuilder From(string tableName)
            {
                if (string.IsNullOrWhiteSpace(tableName))
                    throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));
                _fromTable = tableName;
                return this;
            }

            /// <summary>
            /// Add INNER JOIN.
            /// </summary>
            public SqlQueryBuilder InnerJoin(string joinClause)
            {
                if (string.IsNullOrWhiteSpace(joinClause))
                    throw new ArgumentException("Join clause cannot be null or empty", nameof(joinClause));
                _joins.Add($"INNER JOIN {joinClause}");
                return this;
            }

            /// <summary>
            /// Add LEFT JOIN.
            /// </summary>
            public SqlQueryBuilder LeftJoin(string joinClause)
            {
                if (string.IsNullOrWhiteSpace(joinClause))
                    throw new ArgumentException("Join clause cannot be null or empty", nameof(joinClause));
                _joins.Add($"LEFT JOIN {joinClause}");
                return this;
            }

            /// <summary>
            /// Add WHERE condition with parameter.
            /// </summary>
            public SqlQueryBuilder Where(string condition, string paramName, object paramValue)
            {
                if (string.IsNullOrWhiteSpace(condition) || string.IsNullOrWhiteSpace(paramName))
                    throw new ArgumentException("Condition and parameter name cannot be null or empty");
                _whereConditions.Add(condition);
                _parameters[paramName] = paramValue ?? DBNull.Value;
                return this;
            }

            /// <summary>
            /// Add AND condition.
            /// </summary>
            public SqlQueryBuilder And(string condition, string paramName, object paramValue)
            {
                if (string.IsNullOrWhiteSpace(condition) || string.IsNullOrWhiteSpace(paramName))
                    throw new ArgumentException("Condition and parameter name cannot be null or empty");
                _whereConditions.Add($"AND {condition}");
                _parameters[paramName] = paramValue ?? DBNull.Value;
                return this;
            }

            /// <summary>
            /// Add OR condition.
            /// </summary>
            public SqlQueryBuilder Or(string condition, string paramName, object paramValue)
            {
                if (string.IsNullOrWhiteSpace(condition) || string.IsNullOrWhiteSpace(paramName))
                    throw new ArgumentException("Condition and parameter name cannot be null or empty");
                _whereConditions.Add($"OR {condition}");
                _parameters[paramName] = paramValue ?? DBNull.Value;
                return this;
            }

            /// <summary>
            /// Add ORDER BY column.
            /// </summary>
            public SqlQueryBuilder OrderBy(string column, string direction = "ASC")
            {
                if (string.IsNullOrWhiteSpace(column) || !new[] { "ASC", "DESC" }.Contains(direction))
                    throw new ArgumentException("Invalid column or direction", nameof(column));
                _orderByColumns.Add($"{column} {direction}");
                return this;
            }

            /// <summary>
            /// Set LIMIT clause.
            /// </summary>
            public SqlQueryBuilder Limit(int rows)
            {
                if (rows <= 0)
                    throw new ArgumentException("Limit must be greater than 0", nameof(rows));
                _limitRows = rows;
                return this;
            }

            /// <summary>
            /// Set OFFSET clause.
            /// </summary>
            public SqlQueryBuilder Offset(int rows)
            {
                if (rows < 0)
                    throw new ArgumentException("Offset cannot be negative", nameof(rows));
                _offsetRows = rows;
                return this;
            }

            /// <summary>
            /// Set command timeout in seconds.
            /// </summary>
            public SqlQueryBuilder CommandTimeout(int seconds)
            {
                if (seconds <= 0)
                    throw new ArgumentException("Timeout must be greater than 0", nameof(seconds));
                _commandTimeout = seconds;
                return this;
            }

            public SqlQuery Build()
            {
                if (_baseCommand == "SELECT")
                {
                    if (_selectedColumns.Count == 0)
                        throw new InvalidOperationException("At least one column must be specified for SELECT");
                    if (string.IsNullOrWhiteSpace(_fromTable))
                        throw new InvalidOperationException("FROM table is required for SELECT");

                    var selectClause = "SELECT " + string.Join(", ", _selectedColumns);
                    var fromClause = $"FROM {_fromTable}";
                    var joinClause = _joins.Count > 0 ? " " + string.Join(" ", _joins) : "";
                    var whereClause = _whereConditions.Count > 0 ? " WHERE " + string.Join(" ", _whereConditions) : "";
                    var orderByClause = _orderByColumns.Count > 0 ? " ORDER BY " + string.Join(", ", _orderByColumns) : "";
                    var limitClause = _limitRows.HasValue ? $" LIMIT {_limitRows}" : "";
                    var offsetClause = _offsetRows.HasValue ? $" OFFSET {_offsetRows}" : "";

                    _query = selectClause + " " + fromClause + joinClause + whereClause + orderByClause + limitClause + offsetClause;
                }
                else if (_baseCommand == "INSERT" || _baseCommand == "UPDATE" || _baseCommand == "DELETE")
                {
                    throw new NotImplementedException($"{_baseCommand} queries not implemented in this builder");
                }

                return new SqlQuery(
                    _query,
                    new Dictionary<string, object>(_parameters),
                    _commandType,
                    _commandTimeout,
                    _baseCommand == "SELECT",
                    _selectedColumns.AsReadOnly(),
                    _joins.AsReadOnly(),
                    _whereConditions.AsReadOnly(),
                    _orderByColumns.AsReadOnly(),
                    _limitRows,
                    _offsetRows
                );
            }
        }
    }
}
