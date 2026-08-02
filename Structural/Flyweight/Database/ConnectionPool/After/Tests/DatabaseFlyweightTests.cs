using Xunit;
using Flyweight.Database.ConnectionPool.Component;

namespace Flyweight.Database.ConnectionPool.Tests
{
    public class DatabaseFlyweightTests
    {
        [Fact]
        public void ConnectionConfigFactory_ShouldReuseConfig()
        {
            var factory = new ConnectionConfigFactory();
            var config1 = factory.GetConfig("localhost", 5432, "mydb", "user");
            var config2 = factory.GetConfig("localhost", 5432, "mydb", "user");
            
            Assert.Same(config1, config2);
        }

        [Fact]
        public void ConnectionConfigFactory_ShouldCreateUniqueConfigs()
        {
            var factory = new ConnectionConfigFactory();
            var config1 = factory.GetConfig("localhost", 5432, "mydb", "user");
            var config2 = factory.GetConfig("remotehost", 5432, "mydb", "user");
            
            Assert.NotSame(config1, config2);
        }

        [Fact]
        public void ConnectionPool_ShouldAcquireConnection()
        {
            var pool = new ConnectionPool(10);
            var conn = pool.AcquireConnection("localhost", 5432, "mydb", "user");
            
            Assert.NotNull(conn);
            Assert.True(conn.IsActive);
        }

        [Fact]
        public void ConnectionPool_ShouldReleaseConnection()
        {
            var pool = new ConnectionPool(10);
            var conn = pool.AcquireConnection("localhost", 5432, "mydb", "user");
            
            Assert.Equal(1, pool.GetActiveConnections());
            
            pool.ReleaseConnection(conn);
            
            Assert.Equal(0, pool.GetActiveConnections());
            Assert.Equal(1, pool.GetAvailableConnections());
        }

        [Fact]
        public void ConnectionPool_ShouldReuseReleasedConnections()
        {
            var pool = new ConnectionPool(10);
            var conn1 = pool.AcquireConnection("localhost", 5432, "mydb", "user");
            var connId1 = conn1.ConnectionId;
            
            pool.ReleaseConnection(conn1);
            
            var conn2 = pool.AcquireConnection("localhost", 5432, "mydb", "user");
            var connId2 = conn2.ConnectionId;
            
            Assert.Equal(connId1, connId2); // Reused same connection
        }

        [Fact]
        public void ConnectionPool_ShouldShareConfig()
        {
            var pool = new ConnectionPool(10);
            
            var conn1 = pool.AcquireConnection("localhost", 5432, "db1", "user");
            var conn2 = pool.AcquireConnection("localhost", 5432, "db1", "user");
            
            Assert.Same(conn1.Config, conn2.Config); // Shared config
        }

        [Fact]
        public void ConnectionPool_ShouldRespectMaxPoolSize()
        {
            var pool = new ConnectionPool(5);
            
            for (int i = 0; i < 5; i++)
                pool.AcquireConnection("localhost", 5432, "mydb", "user");
            
            var overflowConn = pool.AcquireConnection("localhost", 5432, "mydb", "user");
            Assert.Null(overflowConn);
        }

        [Fact]
        public void ConnectionConfig_ShouldPreserveAllSettings()
        {
            var factory = new ConnectionConfigFactory();
            var config = factory.GetConfig("myhost", 3306, "testdb", "admin", 60, "latin1");
            
            Assert.Equal("myhost", config.Host);
            Assert.Equal(3306, config.Port);
            Assert.Equal("testdb", config.Database);
            Assert.Equal("admin", config.Username);
            Assert.Equal(60, config.Timeout);
            Assert.Equal("latin1", config.Charset);
        }

        [Fact]
        public void ConnectionPool_ShouldCalculateMemorySavings()
        {
            var pool = new ConnectionPool(100);
            for (int i = 0; i < 100; i++)
            {
                var conn = pool.AcquireConnection("localhost", 5432, "mydb", "user");
                pool.ReleaseConnection(conn);
            }
            
            var savings = pool.EstimateMemorySavings();
            Assert.True(savings > 0);
        }

        [Fact]
        public void ConnectionPool_ShouldTrackConfigPoolGrowth()
        {
            var pool = new ConnectionPool(10);
            
            pool.AcquireConnection("localhost", 5432, "db1", "user");
            Assert.Equal(1, pool.GetConfigPoolSize());
            
            pool.AcquireConnection("localhost", 5433, "db2", "user");
            Assert.Equal(2, pool.GetConfigPoolSize());
            
            pool.AcquireConnection("localhost", 5432, "db1", "user");
            Assert.Equal(2, pool.GetConfigPoolSize()); // Reused config
        }

        [Fact]
        public void LargeConnectionPool_ShouldHandleManyConnections()
        {
            var pool = new ConnectionPool(1000);
            
            for (int i = 0; i < 1000; i++)
            {
                var conn = pool.AcquireConnection("localhost", 5432, "mydb", "user");
                Assert.NotNull(conn);
            }
            
            Assert.Equal(1000, pool.GetActiveConnections());
        }
    }
}
