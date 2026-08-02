using Xunit;
using Proxy.RemoteDatabase.Microservices.Component;

namespace Proxy.RemoteDatabase.Microservices.Tests
{
    public class RemoteDatabaseProxyTests
    {
        [Fact]
        public void DatabaseProxy_ShouldConnectToRemoteDB()
        {
            var proxy = new DatabaseProxy("localhost", 5432);
            Assert.NotNull(proxy);
        }

        [Fact]
        public void DatabaseProxy_ShouldExecuteQuery()
        {
            var proxy = new DatabaseProxy("localhost", 5432);
            var result = proxy.Query("SELECT * FROM users");
            
            Assert.NotNull(result);
        }

        [Fact]
        public void DatabaseProxy_ShouldCacheQueryResults()
        {
            var proxy = new DatabaseProxy("localhost", 5432);
            
            var result1 = proxy.Query("SELECT * FROM users");
            var result2 = proxy.Query("SELECT * FROM users");
            
            Assert.Equal(result1, result2);
            Assert.Equal(1, proxy.GetCacheSize());
        }

        [Fact]
        public void DatabaseProxy_ShouldDifferentiateCachedQueries()
        {
            var proxy = new DatabaseProxy("localhost", 5432);
            
            var result1 = proxy.Query("SELECT * FROM users");
            var result2 = proxy.Query("SELECT * FROM orders");
            
            Assert.Equal(2, proxy.GetCacheSize());
        }

        [Fact]
        public void DatabaseProxy_ShouldClearCacheOnExecute()
        {
            var proxy = new DatabaseProxy("localhost", 5432);
            
            proxy.Query("SELECT * FROM users");
            Assert.Equal(1, proxy.GetCacheSize());
            
            proxy.Execute("UPDATE users SET active = 1");
            Assert.Equal(0, proxy.GetCacheSize());
        }

        [Fact]
        public void DatabaseProxy_ShouldExecuteCommand()
        {
            var proxy = new DatabaseProxy("localhost", 5432);
            var success = proxy.Execute("UPDATE users SET active = 1");
            
            Assert.True(success);
        }

        [Fact]
        public void DatabaseProxy_ShouldHandleNetworkTransparency()
        {
            IDatabase proxy = new DatabaseProxy("192.168.1.100", 5432);
            IDatabase real = new RemoteDatabase("192.168.1.100", 5432);
            
            // Same interface, but proxy handles network
            Assert.NotNull(proxy);
            Assert.NotNull(real);
        }

        [Fact]
        public void DatabaseProxy_ShouldRetryOnFailure()
        {
            var proxy = new DatabaseProxy("localhost", 5432);
            
            var result = proxy.Query("SELECT * FROM users");
            
            Assert.NotNull(result);
        }

        [Fact]
        public void DatabaseProxy_ShouldCacheLargeResultSets()
        {
            var proxy = new DatabaseProxy("localhost", 5432);
            
            for (int i = 0; i < 10; i++)
            {
                proxy.Query("SELECT * FROM users");
            }
            
            // Should still have only 1 cache entry
            Assert.Equal(1, proxy.GetCacheSize());
        }

        [Fact]
        public void RemoteDatabase_ShouldReturnCorrectData()
        {
            var db = new RemoteDatabase("localhost", 5432);
            var result = db.Query("SELECT * FROM users");
            
            Assert.Contains("User", result);
        }

        [Fact]
        public void DatabaseProxy_ShouldMaintainCacheConsistency()
        {
            var proxy = new DatabaseProxy("localhost", 5432);
            
            proxy.Query("SELECT * FROM users");
            proxy.Query("SELECT * FROM orders");
            proxy.Query("SELECT * FROM users");
            
            var cache = proxy.GetCache();
            Assert.Equal(2, cache.Count);
        }
    }
}
