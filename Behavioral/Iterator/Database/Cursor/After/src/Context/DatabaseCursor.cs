using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseCursor.After.Context
{
    /// <summary>
    /// Iterator interface
    /// </summary>
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
    }

    /// <summary>
    /// Collection interface
    /// </summary>
    public interface ICollection<T>
    {
        IIterator<T> CreateIterator();
    }

    /// <summary>
    /// Database row model
    /// </summary>
    public class DatabaseRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime CreatedDate { get; set; }

        public override string ToString() => $"ID: {Id}, Name: {Name}, Email: {Email}";
    }

    /// <summary>
    /// Concrete iterator for database cursor
    /// </summary>
    public class DatabaseCursorIterator : IIterator<DatabaseRow>
    {
        private List<DatabaseRow> _results;
        private int _position = 0;
        private int _pageSize;

        public DatabaseCursorIterator(List<DatabaseRow> results, int pageSize = 10)
        {
            _results = results;
            _pageSize = pageSize;
        }

        public bool HasNext() => _position < _results.Count;

        public DatabaseRow Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more rows");
            return _results[_position++];
        }

        public int GetCurrentPosition() => _position;
        public int GetTotalRows() => _results.Count;
    }

    /// <summary>
    /// Database result set collection
    /// </summary>
    public class DatabaseResultSet : ICollection<DatabaseRow>
    {
        private List<DatabaseRow> _rows;

        public DatabaseResultSet(List<DatabaseRow> rows)
        {
            _rows = rows;
        }

        public IIterator<DatabaseRow> CreateIterator() => new DatabaseCursorIterator(_rows);

        public int GetRowCount() => _rows.Count;

        public void AddRow(DatabaseRow row) => _rows.Add(row);
    }

    /// <summary>
    /// Pagination iterator
    /// </summary>
    public class PaginationIterator : IIterator<List<DatabaseRow>>
    {
        private List<DatabaseRow> _allRows;
        private int _pageSize;
        private int _currentPage = 0;

        public PaginationIterator(List<DatabaseRow> rows, int pageSize = 5)
        {
            _allRows = rows;
            _pageSize = pageSize;
        }

        public bool HasNext() => _currentPage * _pageSize < _allRows.Count;

        public List<DatabaseRow> Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more pages");
            
            var startIndex = _currentPage * _pageSize;
            var endIndex = Math.Min(startIndex + _pageSize, _allRows.Count);
            _currentPage++;
            return _allRows.GetRange(startIndex, endIndex - startIndex);
        }

        public int GetCurrentPage() => _currentPage;
        public int GetTotalPages() => (int)Math.Ceiling((double)_allRows.Count / _pageSize);
    }

    /// <summary>
    /// Reverse cursor iterator
    /// </summary>
    public class ReverseCursorIterator : IIterator<DatabaseRow>
    {
        private List<DatabaseRow> _results;
        private int _position;

        public ReverseCursorIterator(List<DatabaseRow> results)
        {
            _results = results;
            _position = results.Count - 1;
        }

        public bool HasNext() => _position >= 0;

        public DatabaseRow Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more rows");
            return _results[_position--];
        }
    }
}
