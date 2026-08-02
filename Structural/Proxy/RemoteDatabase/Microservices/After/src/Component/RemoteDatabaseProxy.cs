using System;
using System.Collections.Generic;

namespace Proxy.RemoteDatabase.Microservices.Component
{
    // Subject: Database interface
    public interface IDatabase
    {
        string Query(string sql);
        bool Execute(string sql);
    }

    // Real Subject: Remote database (on different server)
    public class RemoteDatabase : IDatabase
    {
        private string _host;
        private int _port;
        private Dictionary<string, string> _data;

        public RemoteDatabase(string host, int port)
        {
            _host = host;
            _port = port;
            _data = new Dictionary<string, string>
            {
                { "SELECT * FROM users", "[User1, User2, User3]" },
                { "SELECT * FROM orders", "[Order1, Order2]" }
            };
            Console.WriteLine($"  RemoteDatabase connected to {host}:{port}");
        }

        public string Query(string sql)
        {
            // Simulate network latency
            System.Threading.Thread.Sleep(50);
            Console.WriteLine($"  [RemoteDB] Executing query: {sql}");
            return _data.ContainsKey(sql) ? _data[sql] : "[]";
        }

        public bool Execute(string sql)
        {
            // Simulate network latency
            System.Threading.Thread.Sleep(50);
            Console.WriteLine($"  [RemoteDB] Executing command: {sql}");
            return true;
        }
    }

    // Proxy: Handles network transparency + connection pooling
    public class DatabaseProxy : IDatabase
    {
        private RemoteDatabase _remoteDb;
        private Dictionary<string, string> _queryCache;
        private int _retryCount = 3;
        private string _host;
        private int _port;

        public DatabaseProxy(string host, int port)
        {
            _host = host;
            _port = port;
            _queryCache = new Dictionary<string, string>();
            _remoteDb = new RemoteDatabase(host, port);
            Console.WriteLine($"✓ [Proxy] DatabaseProxy initialized");
        }

        public string Query(string sql)
        {
            // Check cache first
            if (_queryCache.ContainsKey(sql))
            {
                Console.WriteLine($"✓ [Proxy] Cache HIT: {sql}");
                return _queryCache[sql];
            }

            // Retry logic
            for (int i = 0; i < _retryCount; i++)
            {
                try
                {
                    Console.WriteLine($"✓ [Proxy] Executing query (attempt {i + 1})");
                    var result = _remoteDb.Query(sql);
                    _queryCache[sql] = result;
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ [Proxy] Query failed: {ex.Message}");
                    if (i == _retryCount - 1) throw;
                }
            }
            return null;
        }

        public bool Execute(string sql)
        {
            // Clear cache on write
            _queryCache.Clear();

            for (int i = 0; i < _retryCount; i++)
            {
                try
                {
                    Console.WriteLine($"✓ [Proxy] Executing command (attempt {i + 1})");
                    return _remoteDb.Execute(sql);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ [Proxy] Execute failed: {ex.Message}");
                    if (i == _retryCount - 1) throw;
                }
            }
            return false;
        }

        public int GetCacheSize() => _queryCache.Count;
        public IReadOnlyDictionary<string, string> GetCache() => _queryCache;
    }
}
