using System;
using System.Collections.Generic;
using Bridge.Database.Drivers.Implementation;

namespace Bridge.Database.Drivers.Abstraction
{
    /// <summary>
    /// Abstraction: Database query operations.
    /// Demonstrates: Bridge pattern for database independence.
    /// </summary>
    public abstract class Query
    {
        protected IDatabaseDriver _driver;

        public Query(IDatabaseDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        public abstract ExecutionResult Execute();

        public void SetDriver(IDatabaseDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }
    }

    /// <summary>
    /// Concrete abstraction: SELECT query.
    /// </summary>
    public class SelectQuery : Query
    {
        public string TableName { get; set; }
        public List<string> Columns { get; set; }
        public Dictionary<string, object> Conditions { get; set; }

        public SelectQuery(IDatabaseDriver driver) : base(driver)
        {
            Columns = new List<string>();
            Conditions = new Dictionary<string, object>();
        }

        public override ExecutionResult Execute()
        {
            return _driver.ExecuteSelect(TableName, Columns, Conditions);
        }

        public override string ToString() => 
            $"SELECT {string.Join(", ", Columns)} FROM {TableName}";
    }

    /// <summary>
    /// Concrete abstraction: INSERT query.
    /// </summary>
    public class InsertQuery : Query
    {
        public string TableName { get; set; }
        public Dictionary<string, object> Values { get; set; }

        public InsertQuery(IDatabaseDriver driver) : base(driver)
        {
            Values = new Dictionary<string, object>();
        }

        public override ExecutionResult Execute()
        {
            return _driver.ExecuteInsert(TableName, Values);
        }

        public override string ToString() => 
            $"INSERT INTO {TableName} ({string.Join(", ", Values.Keys)})";
    }

    /// <summary>
    /// Concrete abstraction: UPDATE query.
    /// </summary>
    public class UpdateQuery : Query
    {
        public string TableName { get; set; }
        public Dictionary<string, object> Values { get; set; }
        public Dictionary<string, object> Conditions { get; set; }

        public UpdateQuery(IDatabaseDriver driver) : base(driver)
        {
            Values = new Dictionary<string, object>();
            Conditions = new Dictionary<string, object>();
        }

        public override ExecutionResult Execute()
        {
            return _driver.ExecuteUpdate(TableName, Values, Conditions);
        }

        public override string ToString() => 
            $"UPDATE {TableName} SET {string.Join(", ", Values.Keys)}";
    }

    /// <summary>
    /// Concrete abstraction: DELETE query.
    /// </summary>
    public class DeleteQuery : Query
    {
        public string TableName { get; set; }
        public Dictionary<string, object> Conditions { get; set; }

        public DeleteQuery(IDatabaseDriver driver) : base(driver)
        {
            Conditions = new Dictionary<string, object>();
        }

        public override ExecutionResult Execute()
        {
            return _driver.ExecuteDelete(TableName, Conditions);
        }

        public override string ToString() => 
            $"DELETE FROM {TableName}";
    }

    /// <summary>
    /// Execution result from database query.
    /// </summary>
    public class ExecutionResult
    {
        public bool Success { get; set; }
        public int AffectedRows { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public string ErrorMessage { get; set; }
        public long ExecutionTimeMs { get; set; }

        public ExecutionResult()
        {
            Data = new List<Dictionary<string, object>>();
        }

        public override string ToString() =>
            $"Result(Success={Success}, Rows={AffectedRows}, Time={ExecutionTimeMs}ms)";
    }

    /// <summary>
    /// Query builder for fluent API.
    /// </summary>
    public class QueryBuilder
    {
        private readonly IDatabaseDriver _driver;

        public QueryBuilder(IDatabaseDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        public SelectQueryBuilder Select()
        {
            return new SelectQueryBuilder(_driver);
        }

        public InsertQueryBuilder Insert()
        {
            return new InsertQueryBuilder(_driver);
        }

        public UpdateQueryBuilder Update()
        {
            return new UpdateQueryBuilder(_driver);
        }

        public DeleteQueryBuilder Delete()
        {
            return new DeleteQueryBuilder(_driver);
        }
    }

    public class SelectQueryBuilder
    {
        private readonly SelectQuery _query;

        public SelectQueryBuilder(IDatabaseDriver driver)
        {
            _query = new SelectQuery(driver);
        }

        public SelectQueryBuilder From(string table)
        {
            _query.TableName = table;
            return this;
        }

        public SelectQueryBuilder Columns(params string[] cols)
        {
            foreach (var col in cols)
                _query.Columns.Add(col);
            return this;
        }

        public SelectQueryBuilder Where(string key, object value)
        {
            _query.Conditions[key] = value;
            return this;
        }

        public ExecutionResult Execute() => _query.Execute();
    }

    public class InsertQueryBuilder
    {
        private readonly InsertQuery _query;

        public InsertQueryBuilder(IDatabaseDriver driver)
        {
            _query = new InsertQuery(driver);
        }

        public InsertQueryBuilder Into(string table)
        {
            _query.TableName = table;
            return this;
        }

        public InsertQueryBuilder Values(string key, object value)
        {
            _query.Values[key] = value;
            return this;
        }

        public ExecutionResult Execute() => _query.Execute();
    }

    public class UpdateQueryBuilder
    {
        private readonly UpdateQuery _query;

        public UpdateQueryBuilder(IDatabaseDriver driver)
        {
            _query = new UpdateQuery(driver);
        }

        public UpdateQueryBuilder Table(string name)
        {
            _query.TableName = name;
            return this;
        }

        public UpdateQueryBuilder Set(string key, object value)
        {
            _query.Values[key] = value;
            return this;
        }

        public UpdateQueryBuilder Where(string key, object value)
        {
            _query.Conditions[key] = value;
            return this;
        }

        public ExecutionResult Execute() => _query.Execute();
    }

    public class DeleteQueryBuilder
    {
        private readonly DeleteQuery _query;

        public DeleteQueryBuilder(IDatabaseDriver driver)
        {
            _query = new DeleteQuery(driver);
        }

        public DeleteQueryBuilder From(string table)
        {
            _query.TableName = table;
            return this;
        }

        public DeleteQueryBuilder Where(string key, object value)
        {
            _query.Conditions[key] = value;
            return this;
        }

        public ExecutionResult Execute() => _query.Execute();
    }
}
