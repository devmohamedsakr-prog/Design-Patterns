using Xunit;
using Bridge.Database.Drivers.Abstraction;
using Bridge.Database.Drivers.Implementation;
using System.Collections.Generic;

namespace Bridge.Database.Drivers.Tests
{
    public class QueryExecutionTests
    {
        [Fact]
        public void SelectQuery_ExecuteWithSQLServer_Success()
        {
            var driver = new SqlServerDriver("Server=localhost;Database=Test");
            driver.Connect();
            var query = new SelectQuery(driver) { TableName = "Users" };
            query.Columns.Add("id");
            query.Columns.Add("name");

            var result = query.Execute();

            Assert.True(result.Success);
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public void SelectQuery_ExecuteWithPostgreSQL_Success()
        {
            var driver = new PostgreSQLDriver("Host=localhost;Database=test");
            driver.Connect();
            var query = new SelectQuery(driver) { TableName = "Users" };
            query.Columns.Add("*");

            var result = query.Execute();

            Assert.True(result.Success);
        }

        [Fact]
        public void InsertQuery_ExecuteWithMySQL_Success()
        {
            var driver = new MySQLDriver("Server=localhost;Database=test");
            driver.Connect();
            var query = new InsertQuery(driver) { TableName = "Users" };
            query.Values["name"] = "John";
            query.Values["email"] = "john@example.com";

            var result = query.Execute();

            Assert.True(result.Success);
            Assert.Equal(1, result.AffectedRows);
        }

        [Fact]
        public void UpdateQuery_ExecuteWithMongoDB_Success()
        {
            var driver = new MongoDBDriver("mongodb://localhost:27017");
            driver.Connect();
            var query = new UpdateQuery(driver) { TableName = "users" };
            query.Values["status"] = "active";
            query.Conditions["id"] = "123";

            var result = query.Execute();

            Assert.True(result.Success);
        }

        [Fact]
        public void DeleteQuery_ExecuteWithSQLServer_Success()
        {
            var driver = new SqlServerDriver("Server=localhost");
            driver.Connect();
            var query = new DeleteQuery(driver) { TableName = "Users" };
            query.Conditions["id"] = 999;

            var result = query.Execute();

            Assert.True(result.Success);
        }

        [Fact]
        public void Query_SwitchDriver_Success()
        {
            var driver1 = new SqlServerDriver("Server=localhost");
            var query = new SelectQuery(driver1) { TableName = "Users" };
            query.Columns.Add("*");

            var result1 = query.Execute();
            Assert.True(result1.Success);

            var driver2 = new PostgreSQLDriver("Host=localhost");
            query.SetDriver(driver2);

            var result2 = query.Execute();
            Assert.True(result2.Success);
        }

        [Fact]
        public void QueryBuilder_Select_FluentAPI()
        {
            var driver = new SqlServerDriver("Server=localhost");
            driver.Connect();
            var builder = new QueryBuilder(driver);

            var result = builder.Select()
                .From("Users")
                .Columns("id", "name", "email")
                .Where("status", "active")
                .Execute();

            Assert.True(result.Success);
        }

        [Fact]
        public void QueryBuilder_Insert_Success()
        {
            var driver = new MySQLDriver("Server=localhost");
            driver.Connect();
            var builder = new QueryBuilder(driver);

            var result = builder.Insert()
                .Into("Users")
                .Values("name", "Alice")
                .Values("email", "alice@example.com")
                .Execute();

            Assert.True(result.Success);
        }

        [Fact]
        public void QueryBuilder_Update_Success()
        {
            var driver = new PostgreSQLDriver("Host=localhost");
            driver.Connect();
            var builder = new QueryBuilder(driver);

            var result = builder.Update()
                .Table("Users")
                .Set("status", "inactive")
                .Where("id", 1)
                .Execute();

            Assert.True(result.Success);
        }

        [Fact]
        public void QueryBuilder_Delete_Success()
        {
            var driver = new SqlServerDriver("Server=localhost");
            driver.Connect();
            var builder = new QueryBuilder(driver);

            var result = builder.Delete()
                .From("Users")
                .Where("id", 999)
                .Execute();

            Assert.True(result.Success);
        }

        [Fact]
        public void Driver_ConnectDisconnect_Success()
        {
            var driver = new MySQLDriver("Server=localhost");
            
            var connected = driver.Connect();
            Assert.True(connected);

            driver.Disconnect();
        }

        [Fact]
        public void SelectQuery_WithConditions_Success()
        {
            var driver = new MongoDBDriver("mongodb://localhost");
            driver.Connect();
            var query = new SelectQuery(driver) { TableName = "products" };
            query.Columns.Add("name");
            query.Columns.Add("price");
            query.Conditions["category"] = "electronics";
            query.Conditions["price"] = 100;

            var result = query.Execute();

            Assert.True(result.Success);
        }

        [Fact]
        public void Query_WithNullDriver_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SelectQuery(null)
            );

            Assert.Contains("driver", exception.Message);
        }

        [Fact]
        public void ExecutionResult_ToString_ContainsInfo()
        {
            var result = new ExecutionResult
            {
                Success = true,
                AffectedRows = 5,
                ExecutionTimeMs = 150
            };

            var str = result.ToString();
            Assert.Contains("True", str);
            Assert.Contains("5", str);
            Assert.Contains("150", str);
        }

        [Fact]
        public void Query_ToString_ContainsInfo()
        {
            var driver = new SqlServerDriver("Server=localhost");
            var query = new SelectQuery(driver) { TableName = "Users" };
            query.Columns.Add("id");
            query.Columns.Add("name");

            var str = query.ToString();
            Assert.Contains("SELECT", str);
            Assert.Contains("Users", str);
        }

        [Fact]
        public void AllDrivers_ExecuteQueries_Success()
        {
            var drivers = new IDatabaseDriver[]
            {
                new SqlServerDriver(""),
                new PostgreSQLDriver(""),
                new MySQLDriver(""),
                new MongoDBDriver("")
            };

            foreach (var driver in drivers)
            {
                driver.Connect();
                var query = new SelectQuery(driver) { TableName = "test" };
                query.Columns.Add("*");

                var result = query.Execute();
                Assert.True(result.Success);
            }
        }

        [Fact]
        public void QueryBuilder_WithNullDriver_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new QueryBuilder(null)
            );

            Assert.Contains("driver", exception.Message);
        }
    }
}
