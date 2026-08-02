using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchDsl.After.Context
{
    public class Product
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string Brand { get; set; } = "";
        public int Stock { get; set; }
    }

    public abstract class SearchExpression
    {
        public abstract bool Evaluate(Product product);
    }

    public class CategoryFilter : SearchExpression
    {
        private string _category;

        public CategoryFilter(string category) => _category = category;

        public override bool Evaluate(Product product) => product.Category == _category;
    }

    public class BrandFilter : SearchExpression
    {
        private string _brand;

        public BrandFilter(string brand) => _brand = brand;

        public override bool Evaluate(Product product) => product.Brand == _brand;
    }

    public class PriceRange : SearchExpression
    {
        private decimal _min, _max;

        public PriceRange(decimal min, decimal max)
        {
            _min = min;
            _max = max;
        }

        public override bool Evaluate(Product product) => product.Price >= _min && product.Price <= _max;
    }

    public class InStock : SearchExpression
    {
        public override bool Evaluate(Product product) => product.Stock > 0;
    }

    public class AndExpression : SearchExpression
    {
        private SearchExpression _left, _right;

        public AndExpression(SearchExpression left, SearchExpression right)
        {
            _left = left;
            _right = right;
        }

        public override bool Evaluate(Product product) 
            => _left.Evaluate(product) && _right.Evaluate(product);
    }

    public class OrExpression : SearchExpression
    {
        private SearchExpression _left, _right;

        public OrExpression(SearchExpression left, SearchExpression right)
        {
            _left = left;
            _right = right;
        }

        public override bool Evaluate(Product product) 
            => _left.Evaluate(product) || _right.Evaluate(product);
    }

    public class NotExpression : SearchExpression
    {
        private SearchExpression _expr;

        public NotExpression(SearchExpression expr) => _expr = expr;

        public override bool Evaluate(Product product) => !_expr.Evaluate(product);
    }

    public class SearchFilterParser
    {
        public List<Product> ParseAndFilter(List<Product> products, string filterExpression)
        {
            var expression = Parse(filterExpression);
            return products.Where(p => expression.Evaluate(p)).ToList();
        }

        private SearchExpression Parse(string expr)
        {
            expr = expr.Trim();
            
            if (expr.StartsWith("category:"))
            {
                var cat = expr.Substring(9).Split(' ')[0];
                return new CategoryFilter(cat);
            }
            
            if (expr.StartsWith("brand:"))
            {
                var brand = expr.Substring(6).Split(' ')[0];
                return new BrandFilter(brand);
            }
            
            if (expr.StartsWith("price:"))
            {
                var range = expr.Substring(6);
                var parts = range.Split('-');
                if (decimal.TryParse(parts[0], out var min) && decimal.TryParse(parts[1], out var max))
                    return new PriceRange(min, max);
            }
            
            if (expr.Contains(" AND "))
            {
                var parts = expr.Split(new[] { " AND " }, StringSplitOptions.None);
                return new AndExpression(Parse(parts[0]), Parse(parts[1]));
            }
            
            if (expr.Contains(" OR "))
            {
                var parts = expr.Split(new[] { " OR " }, StringSplitOptions.None);
                return new OrExpression(Parse(parts[0]), Parse(parts[1]));
            }
            
            return new CategoryFilter("all");
        }
    }
}
