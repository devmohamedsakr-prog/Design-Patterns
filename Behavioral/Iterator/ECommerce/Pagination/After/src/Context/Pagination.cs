using System;
using System.Collections.Generic;
using System.Linq;

namespace Pagination.After.Context
{
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public override string ToString() => $"#{Id} {Name} - ${Price}";
    }

    public class OffsetBasedPaginator : IIterator<List<Product>>
    {
        private List<Product> _products;
        private int _pageSize;
        private int _offset = 0;

        public OffsetBasedPaginator(List<Product> products, int pageSize = 10)
        {
            _products = products;
            _pageSize = pageSize;
        }

        public bool HasNext() => _offset < _products.Count;

        public List<Product> Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more pages");
            var page = _products.Skip(_offset).Take(_pageSize).ToList();
            _offset += _pageSize;
            return page;
        }

        public int GetCurrentPage() => _offset / _pageSize;
        public int GetTotalPages() => (int)Math.Ceiling((double)_products.Count / _pageSize);
    }

    public class CursorBasedPaginator : IIterator<List<Product>>
    {
        private List<Product> _products;
        private int _pageSize;
        private int _cursorPosition = 0;

        public CursorBasedPaginator(List<Product> products, int pageSize = 10)
        {
            _products = products;
            _pageSize = pageSize;
        }

        public bool HasNext() => _cursorPosition < _products.Count;

        public List<Product> Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more pages");
            var page = _products.Skip(_cursorPosition).Take(_pageSize).ToList();
            if (page.Count > 0)
                _cursorPosition = _products.IndexOf(page.Last()) + 1;
            return page;
        }

        public int GetCurrentCursor() => _cursorPosition;
    }

    public class FilteredPaginator : IIterator<List<Product>>
    {
        private List<Product> _filteredProducts;
        private int _pageSize;
        private int _offset = 0;

        public FilteredPaginator(List<Product> products, int pageSize = 10, decimal? minPrice = null, decimal? maxPrice = null)
        {
            _pageSize = pageSize;
            _filteredProducts = products.Where(p => 
                (!minPrice.HasValue || p.Price >= minPrice) &&
                (!maxPrice.HasValue || p.Price <= maxPrice)
            ).ToList();
        }

        public bool HasNext() => _offset < _filteredProducts.Count;

        public List<Product> Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more pages");
            var page = _filteredProducts.Skip(_offset).Take(_pageSize).ToList();
            _offset += _pageSize;
            return page;
        }

        public int GetTotalResults() => _filteredProducts.Count;
    }

    public class ReversePaginator : IIterator<List<Product>>
    {
        private List<Product> _products;
        private int _pageSize;
        private int _position;

        public ReversePaginator(List<Product> products, int pageSize = 10)
        {
            _products = products;
            _pageSize = pageSize;
            _position = (int)Math.Ceiling((double)products.Count / pageSize) - 1;
        }

        public bool HasNext() => _position >= 0;

        public List<Product> Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more pages");
            var startIndex = Math.Max(0, _position * _pageSize);
            var endIndex = Math.Min(((_position + 1) * _pageSize), _products.Count);
            var page = _products.GetRange(startIndex, endIndex - startIndex);
            _position--;
            return page;
        }
    }
}
