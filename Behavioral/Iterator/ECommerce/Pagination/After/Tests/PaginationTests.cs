using NUnit.Framework;
using Pagination.After.Context;
using System.Collections.Generic;

namespace Pagination.After.Tests
{
    [TestFixture]
    public class PaginationTests
    {
        private List<Product> _products;

        [SetUp]
        public void Setup()
        {
            _products = new List<Product>();
            for (int i = 1; i <= 25; i++)
                _products.Add(new Product { Id = i, Name = $"Product{i}", Price = 10.0m * i, Stock = i * 10 });
        }

        [Test]
        public void OffsetBased_HasNext() => Assert.That(new OffsetBasedPaginator(_products, 10).HasNext(), Is.True);

        [Test]
        public void OffsetBased_PageSize()
        {
            var paginator = new OffsetBasedPaginator(_products, 10);
            var page = paginator.Next();
            Assert.That(page.Count, Is.EqualTo(10));
        }

        [Test]
        public void OffsetBased_TotalPages()
        {
            var paginator = new OffsetBasedPaginator(_products, 10);
            Assert.That(paginator.GetTotalPages(), Is.EqualTo(3));
        }

        [Test]
        public void CursorBased_Navigation()
        {
            var paginator = new CursorBasedPaginator(_products, 5);
            var page1 = paginator.Next();
            Assert.That(page1.Count, Is.EqualTo(5));
        }

        [Test]
        public void Filtered_Pagination()
        {
            var paginator = new FilteredPaginator(_products, 5, minPrice: 50, maxPrice: 200);
            var page = paginator.Next();
            Assert.That(page.Count, Is.LessThanOrEqualTo(5));
        }

        [Test]
        public void Reverse_Pagination()
        {
            var paginator = new ReversePaginator(_products, 5);
            Assert.That(paginator.HasNext(), Is.True);
            var page = paginator.Next();
            Assert.That(page.Count, Is.GreaterThan(0));
        }

        [Test]
        public void MultiplePages()
        {
            var paginator = new OffsetBasedPaginator(_products, 10);
            var page1 = paginator.Next();
            var page2 = paginator.Next();
            var page3 = paginator.Next();
            Assert.That(page1[0].Id, Is.Not.EqualTo(page2[0].Id));
        }
    }
}
