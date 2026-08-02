using Xunit;
using Builder.Database.SqlQuery.Context;
using System;

namespace Builder.Database.SqlQuery.Tests
{
    public class SqlQueryTests
    {
        [Fact]
        public void Builder_CreateBasicSelectQuery_Success()
        {
            var query = SqlQuery.Select
                .Columns("id", "name", "email")
                .From("users")
                .Build();

            Assert.True(query.IsSelectQuery);
            Assert.Contains("SELECT", query.Query);
            Assert.Contains("FROM users", query.Query);
            Assert.Equal(3, query.SelectedColumns.Count);
        }

        [Fact]
        public void Builder_SelectWithWhereClause_Success()
        {
            var query = SqlQuery.Select
                .Columns("id", "name", "email")
                .From("users")
                .Where("age > @minAge", "@minAge", 18)
                .Build();

            Assert.Contains("WHERE", query.Query);
            Assert.Single(query.WhereConditions);
            Assert.Equal(18, query.Parameters["@minAge"]);
        }

        [Fact]
        public void Builder_SelectWithAndCondition_Success()
        {
            var query = SqlQuery.Select
                .Columns("id", "name")
                .From("users")
                .Where("age > @minAge", "@minAge", 18)
                .And("status = @status", "@status", "active")
                .Build();

            Assert.Equal(2, query.WhereConditions.Count);
            Assert.Equal(2, query.Parameters.Count);
            Assert.Equal("active", query.Parameters["@status"]);
        }

        [Fact]
        public void Builder_SelectWithOrCondition_Success()
        {
            var query = SqlQuery.Select
                .Columns("id", "name")
                .From("users")
                .Where("role = @admin", "@admin", "admin")
                .Or("role = @moderator", "@moderator", "moderator")
                .Build();

            Assert.Equal(2, query.WhereConditions.Count);
            Assert.Contains("OR", query.Query);
        }

        [Fact]
        public void Builder_SelectWithInnerJoin_Success()
        {
            var query = SqlQuery.Select
                .Columns("u.id", "u.name", "p.title")
                .From("users u")
                .InnerJoin("posts p ON u.id = p.user_id")
                .Build();

            Assert.Single(query.Joins);
            Assert.Contains("INNER JOIN", query.Query);
        }

        [Fact]
        public void Builder_SelectWithLeftJoin_Success()
        {
            var query = SqlQuery.Select
                .Columns("u.id", "u.name", "o.order_id")
                .From("users u")
                .LeftJoin("orders o ON u.id = o.user_id")
                .Build();

            Assert.Single(query.Joins);
            Assert.Contains("LEFT JOIN", query.Query);
        }

        [Fact]
        public void Builder_SelectWithMultipleJoins_Success()
        {
            var query = SqlQuery.Select
                .Columns("u.id", "u.name", "p.title", "c.content")
                .From("users u")
                .InnerJoin("posts p ON u.id = p.user_id")
                .LeftJoin("comments c ON p.id = c.post_id")
                .Build();

            Assert.Equal(2, query.Joins.Count);
            Assert.Contains("INNER JOIN", query.Query);
            Assert.Contains("LEFT JOIN", query.Query);
        }

        [Fact]
        public void Builder_SelectWithOrderBy_Success()
        {
            var query = SqlQuery.Select
                .Columns("id", "name", "created")
                .From("users")
                .OrderBy("created", "DESC")
                .OrderBy("name", "ASC")
                .Build();

            Assert.Equal(2, query.OrderByColumns.Count);
            Assert.Contains("ORDER BY", query.Query);
        }

        [Fact]
        public void Builder_SelectWithLimitOffset_Success()
        {
            var query = SqlQuery.Select
                .Columns("id", "name")
                .From("users")
                .Limit(10)
                .Offset(20)
                .Build();

            Assert.Equal(10, query.LimitRows);
            Assert.Equal(20, query.OffsetRows);
            Assert.Contains("LIMIT 10", query.Query);
            Assert.Contains("OFFSET 20", query.Query);
        }

        [Fact]
        public void Builder_ComplexSelectQuery_Success()
        {
            var query = SqlQuery.Select
                .Columns("u.id", "u.name", "u.email", "COUNT(p.id)")
                .From("users u")
                .LeftJoin("posts p ON u.id = p.user_id")
                .Where("u.status = @status", "@status", "active")
                .And("u.created > @minDate", "@minDate", "2024-01-01")
                .OrderBy("u.name", "ASC")
                .Limit(50)
                .Offset(0)
                .Build();

            Assert.Equal(4, query.SelectedColumns.Count);
            Assert.Single(query.Joins);
            Assert.Equal(2, query.WhereConditions.Count);
            Assert.Single(query.OrderByColumns.Count);
            Assert.Equal(50, query.LimitRows);
            Assert.Equal(0, query.OffsetRows);
        }

        [Fact]
        public void Builder_SelectWithCommandTimeout_Success()
        {
            var query = SqlQuery.Select
                .Columns("id", "name")
                .From("users")
                .CommandTimeout(60)
                .Build();

            Assert.Equal(60, query.CommandTimeout);
        }

        [Fact]
        public void Builder_ParameterizedQuery_PreventsSqlInjection()
        {
            var query = SqlQuery.Select
                .Columns("id", "name")
                .From("users")
                .Where("name = @name", "@name", "'; DROP TABLE users; --")
                .Build();

            // Parameters are safely stored, not embedded in query
            Assert.Equal("'; DROP TABLE users; --", query.Parameters["@name"]);
            Assert.DoesNotContain("DROP TABLE", query.Query);
        }

        [Fact]
        public void Builder_MultipleWhereConditions_Success()
        {
            var query = SqlQuery.Select
                .Columns("*")
                .From("products")
                .Where("price > @minPrice", "@minPrice", 10)
                .And("category = @category", "@category", "electronics")
                .And("stock > @minStock", "@minStock", 0)
                .Build();

            Assert.Equal(3, query.WhereConditions.Count);
            Assert.Equal(3, query.Parameters.Count);
        }

        [Fact]
        public void Builder_MissingColumns_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                SqlQuery.Select
                    .From("users")
                    .Build()
            );

            Assert.Contains("At least one column must be specified", exception.Message);
        }

        [Fact]
        public void Builder_MissingFromTable_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                SqlQuery.Select
                    .Columns("id", "name")
                    .Build()
            );

            Assert.Contains("FROM table is required", exception.Message);
        }

        [Fact]
        public void Builder_EmptyColumns_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                SqlQuery.Select.Columns()
            );

            Assert.Contains("At least one column must be specified", exception.Message);
        }

        [Fact]
        public void Builder_InvalidFromTable_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                SqlQuery.Select
                    .Columns("id")
                    .From("")
                    .Build()
            );

            Assert.Contains("Table name cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_InvalidOrderByDirection_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                SqlQuery.Select
                    .Columns("id")
                    .From("users")
                    .OrderBy("name", "INVALID")
                    .Build()
            );

            Assert.Contains("Invalid column or direction", exception.Message);
        }

        [Fact]
        public void Builder_InvalidLimit_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                SqlQuery.Select
                    .Columns("id")
                    .From("users")
                    .Limit(-1)
                    .Build()
            );

            Assert.Contains("Limit must be greater than 0", exception.Message);
        }

        [Fact]
        public void Builder_InvalidOffset_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                SqlQuery.Select
                    .Columns("id")
                    .From("users")
                    .Offset(-1)
                    .Build()
            );

            Assert.Contains("Offset cannot be negative", exception.Message);
        }

        [Fact]
        public void Builder_InvalidCommandTimeout_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                SqlQuery.Select
                    .Columns("id")
                    .From("users")
                    .CommandTimeout(0)
                    .Build()
            );

            Assert.Contains("Timeout must be greater than 0", exception.Message);
        }

        [Fact]
        public void Builder_NullJoinClause_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                SqlQuery.Select
                    .Columns("id")
                    .From("users")
                    .InnerJoin(null)
                    .Build()
            );

            Assert.Contains("Join clause cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_IsImmutable_Parameters()
        {
            var query = SqlQuery.Select
                .Columns("id", "name")
                .From("users")
                .Where("age > @age", "@age", 18)
                .Build();

            Assert.Throws<NotSupportedException>(() =>
            {
                ((System.Collections.Generic.Dictionary<string, object>)query.Parameters).Add("@new", 99);
            });
        }

        [Fact]
        public void Builder_FluentChaining_Success()
        {
            var query = SqlQuery.Select
                .Columns("id", "name", "email")
                .From("users")
                .Where("active = @active", "@active", true)
                .OrderBy("name", "ASC")
                .Limit(20)
                .Build();

            Assert.NotNull(query);
            Assert.True(query.IsSelectQuery);
        }

        [Fact]
        public void SqlQuery_ToString_ContainsRelevantInfo()
        {
            var query = SqlQuery.Select
                .Columns("id", "name")
                .From("users")
                .Where("status = @status", "@status", "active")
                .Limit(10)
                .Build();

            var str = query.ToString();
            Assert.Contains("SELECT", str);
            Assert.Contains("10", str);
        }

        [Fact]
        public void Builder_DefaultValues_Applied()
        {
            var query = SqlQuery.Select
                .Columns("*")
                .From("users")
                .Build();

            Assert.Equal("Text", query.CommandType);
            Assert.Equal(30, query.CommandTimeout);
            Assert.Empty(query.Joins);
            Assert.Empty(query.WhereConditions);
            Assert.Null(query.LimitRows);
            Assert.Null(query.OffsetRows);
        }

        [Fact]
        public void Builder_NullParameterValue_AllowedAsDbNull()
        {
            var query = SqlQuery.Select
                .Columns("id", "name")
                .From("users")
                .Where("deleted_at = @deleted", "@deleted", null)
                .Build();

            Assert.Equal(DBNull.Value, query.Parameters["@deleted"]);
        }
    }
}
