using NUnit.Framework;
using DatabaseCursor.After.Context;
using System;
using System.Collections.Generic;

namespace DatabaseCursor.After.Tests
{
    [TestFixture]
    public class DatabaseCursorTests
    {
        private DatabaseResultSet _resultSet;

        [SetUp]
        public void Setup()
        {
            var rows = new List<DatabaseRow>
            {
                new() { Id = 1, Name = "Alice", Email = "alice@test.com", CreatedDate = DateTime.Now },
                new() { Id = 2, Name = "Bob", Email = "bob@test.com", CreatedDate = DateTime.Now },
                new() { Id = 3, Name = "Charlie", Email = "charlie@test.com", CreatedDate = DateTime.Now },
            };
            _resultSet = new DatabaseResultSet(rows);
        }

        [Test]
        public void Cursor_HasNext() => Assert.That(_resultSet.CreateIterator().HasNext(), Is.True);

        [Test]
        public void Cursor_Next() 
        { 
            var iterator = _resultSet.CreateIterator();
            var row = iterator.Next();
            Assert.That(row.Id, Is.EqualTo(1));
        }

        [Test]
        public void Cursor_IterateAll()
        {
            var iterator = _resultSet.CreateIterator();
            int count = 0;
            while (iterator.HasNext())
            {
                iterator.Next();
                count++;
            }
            Assert.That(count, Is.EqualTo(3));
        }

        [Test]
        public void Pagination_FirstPage()
        {
            var rows = new List<DatabaseRow>();
            for (int i = 1; i <= 25; i++)
                rows.Add(new() { Id = i, Name = $"User{i}", Email = $"user{i}@test.com" });
            
            var paginator = new PaginationIterator(rows, 5);
            Assert.That(paginator.HasNext(), Is.True);
            var page = paginator.Next();
            Assert.That(page.Count, Is.EqualTo(5));
        }

        [Test]
        public void Pagination_LastPage()
        {
            var rows = new List<DatabaseRow>();
            for (int i = 1; i <= 12; i++)
                rows.Add(new() { Id = i, Name = $"User{i}" });
            
            var paginator = new PaginationIterator(rows, 5);
            var page1 = paginator.Next();
            var page2 = paginator.Next();
            var page3 = paginator.Next();
            Assert.That(page3.Count, Is.EqualTo(2));
        }

        [Test]
        public void ReverseIterator()
        {
            var rows = new List<DatabaseRow>
            {
                new() { Id = 1, Name = "A" },
                new() { Id = 2, Name = "B" },
                new() { Id = 3, Name = "C" },
            };
            var iterator = new ReverseCursorIterator(rows);
            var first = iterator.Next();
            Assert.That(first.Id, Is.EqualTo(3));
        }
    }
}
