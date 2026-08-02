using System;
using System.Collections.Generic;

namespace Flyweight.Database.ConnectionPool.Component
{
    // Intrinsic State: Shared connection configuration
    public class ConnectionConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public int Timeout { get; set; }
        public string Charset { get; set; }

        public override string ToString() => $"{Host}:{Port}/{Database}";
    }

    // Flyweight Factory for connection configs
    public class ConnectionConfigFactory
    {
        private Dictionary<string, ConnectionConfig> _configPool = new();

        public ConnectionConfig GetConfig(string host, int port, string database, string username, int timeout = 30, string charset = "UTF-8")
        {
            var key = $"{host}_{port}_{database}_{username}";
            
            if (!_configPool.ContainsKey(key))
            {
                _configPool[key] = new ConnectionConfig
                {
                    Host = host,
                    Port = port,
                    Database = database,
                    Username = username,
                    Timeout = timeout,
                    Charset = charset
                };
            }

            return _configPool[key];
        }

        public int GetPoolSize() => _configPool.Count;
        public IReadOnlyDictionary<string, ConnectionConfig> GetPool() => _configPool;
    }

    // Extrinsic State: Per-connection unique data
    public class DatabaseConnection
    {
        public string ConnectionId { get; set; }
        public ConnectionConfig Config { get; set; }
        public bool IsActive { get; set; }
        public string CurrentQuery { get; set; }
        public int BorrowedByThreadId { get; set; }
        public DateTime CreatedTime { get; set; }

        public override string ToString() => $"Conn[{ConnectionId}] {Config} Active={IsActive}";
    }

    // Connection Pool using Flyweight pattern
    public class ConnectionPool
    {
        private Queue<DatabaseConnection> _availableConnections = new();
        private HashSet<DatabaseConnection> _activeConnections = new();
        private ConnectionConfigFactory _configFactory = new();
        private int _connectionIdCounter = 0;
        private int _maxPoolSize;

        public ConnectionPool(int maxPoolSize = 100)
        {
            _maxPoolSize = maxPoolSize;
        }

        public DatabaseConnection AcquireConnection(string host, int port, string database, string username)
        {
            DatabaseConnection conn;

            if (_availableConnections.Count > 0)
            {
                conn = _availableConnections.Dequeue();
            }
            else if (_activeConnections.Count < _maxPoolSize)
            {
                var config = _configFactory.GetConfig(host, port, database, username);
                conn = new DatabaseConnection
                {
                    ConnectionId = $"CONN_{++_connectionIdCounter}",
                    Config = config,
                    CreatedTime = DateTime.UtcNow
                };
            }
            else
            {
                return null; // Pool exhausted
            }

            conn.IsActive = true;
            conn.BorrowedByThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            _activeConnections.Add(conn);
            return conn;
        }

        public void ReleaseConnection(DatabaseConnection conn)
        {
            if (conn != null && _activeConnections.Contains(conn))
            {
                _activeConnections.Remove(conn);
                conn.IsActive = false;
                conn.CurrentQuery = null;
                _availableConnections.Enqueue(conn);
            }
        }

        public int GetActiveConnections() => _activeConnections.Count;
        public int GetAvailableConnections() => _availableConnections.Count;
        public int GetConfigPoolSize() => _configFactory.GetPoolSize();
        public long EstimateMemorySavings() => (long)(_activeConnections.Count + _availableConnections.Count) * 2000 - _configFactory.GetPoolSize() * 2000;
    }
}
