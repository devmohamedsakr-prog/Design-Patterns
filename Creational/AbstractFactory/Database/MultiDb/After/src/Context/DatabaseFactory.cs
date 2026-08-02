using System;
using System.Collections.Generic;

namespace DatabaseFactory.After.Context
{
    // Abstract products
    public interface IConnection
    {
        void Open();
        void Close();
        string GetConnectionString();
    }

    public interface ICommand
    {
        void ExecuteQuery(string query);
        object ExecuteScalar(string query);
    }

    public interface IDataReader
    {
        List<Dictionary<string, object>> Read();
    }

    // Abstract factory
    public interface IDatabaseFactory
    {
        IConnection CreateConnection(string connStr);
        ICommand CreateCommand();
        IDataReader CreateDataReader();
    }

    // MySQL implementations
    public class MySqlConnection : IConnection
    {
        private string _connStr;
        public MySqlConnection(string connStr) => _connStr = connStr;
        public void Open() => Console.WriteLine("🔓 MySQL: Connection opened on port 3306");
        public void Close() => Console.WriteLine("🔒 MySQL: Connection closed");
        public string GetConnectionString() => _connStr;
    }

    public class MySqlCommand : ICommand
    {
        public void ExecuteQuery(string query) => Console.WriteLine($"📝 MySQL: Executing query: {query}");
        public object ExecuteScalar(string query)
        {
            Console.WriteLine($"🔢 MySQL: Executing scalar query: {query}");
            return 1;
        }
    }

    public class MySqlDataReader : IDataReader
    {
        public List<Dictionary<string, object>> Read()
        {
            Console.WriteLine("📖 MySQL: Reading data...");
            return new List<Dictionary<string, object>> { new() { { "id", 1 } } };
        }
    }

    // PostgreSQL implementations
    public class PostgresConnection : IConnection
    {
        private string _connStr;
        public PostgresConnection(string connStr) => _connStr = connStr;
        public void Open() => Console.WriteLine("🔓 PostgreSQL: Connection opened on port 5432");
        public void Close() => Console.WriteLine("🔒 PostgreSQL: Connection closed");
        public string GetConnectionString() => _connStr;
    }

    public class PostgresCommand : ICommand
    {
        public void ExecuteQuery(string query) => Console.WriteLine($"📝 PostgreSQL: Executing query: {query}");
        public object ExecuteScalar(string query)
        {
            Console.WriteLine($"🔢 PostgreSQL: Executing scalar query: {query}");
            return 1;
        }
    }

    public class PostgresDataReader : IDataReader
    {
        public List<Dictionary<string, object>> Read()
        {
            Console.WriteLine("📖 PostgreSQL: Reading data...");
            return new List<Dictionary<string, object>> { new() { { "id", 1 } } };
        }
    }

    // SQL Server implementations
    public class SqlServerConnection : IConnection
    {
        private string _connStr;
        public SqlServerConnection(string connStr) => _connStr = connStr;
        public void Open() => Console.WriteLine("🔓 SQL Server: Connection opened on port 1433");
        public void Close() => Console.WriteLine("🔒 SQL Server: Connection closed");
        public string GetConnectionString() => _connStr;
    }

    public class SqlServerCommand : ICommand
    {
        public void ExecuteQuery(string query) => Console.WriteLine($"📝 SQL Server: Executing query: {query}");
        public object ExecuteScalar(string query)
        {
            Console.WriteLine($"🔢 SQL Server: Executing scalar query: {query}");
            return 1;
        }
    }

    public class SqlServerDataReader : IDataReader
    {
        public List<Dictionary<string, object>> Read()
        {
            Console.WriteLine("📖 SQL Server: Reading data...");
            return new List<Dictionary<string, object>> { new() { { "id", 1 } } };
        }
    }

    // Concrete factories
    public class MySqlFactory : IDatabaseFactory
    {
        public IConnection CreateConnection(string connStr) => new MySqlConnection(connStr);
        public ICommand CreateCommand() => new MySqlCommand();
        public IDataReader CreateDataReader() => new MySqlDataReader();
    }

    public class PostgresFactory : IDatabaseFactory
    {
        public IConnection CreateConnection(string connStr) => new PostgresConnection(connStr);
        public ICommand CreateCommand() => new PostgresCommand();
        public IDataReader CreateDataReader() => new PostgresDataReader();
    }

    public class SqlServerFactory : IDatabaseFactory
    {
        public IConnection CreateConnection(string connStr) => new SqlServerConnection(connStr);
        public ICommand CreateCommand() => new SqlServerCommand();
        public IDataReader CreateDataReader() => new SqlServerDataReader();
    }

    // Factory provider
    public class DatabaseFactoryProvider
    {
        public static IDatabaseFactory GetFactory(string dbType)
        {
            return dbType.ToLower() switch
            {
                "mysql" => new MySqlFactory(),
                "postgres" => new PostgresFactory(),
                "sqlserver" => new SqlServerFactory(),
                _ => throw new ArgumentException($"Unknown database: {dbType}")
            };
        }
    }

    // Database access layer
    public class DatabaseAccessLayer
    {
        private IConnection _connection;
        private ICommand _command;
        private IDataReader _reader;

        public DatabaseAccessLayer(IDatabaseFactory factory, string connStr)
        {
            _connection = factory.CreateConnection(connStr);
            _command = factory.CreateCommand();
            _reader = factory.CreateDataReader();
        }

        public void ExecuteQuery(string query)
        {
            Console.WriteLine($"\n📊 Database Access Layer");
            _connection.Open();
            _command.ExecuteQuery(query);
            _reader.Read();
            _connection.Close();
        }
    }
}
