using NUnit.Framework;
using TaxCalculator.After.Context;

namespace TaxCalculator.After.Tests
{
    [TestFixture]
    public class TaxCalculatorTests
    {
        [Test]
        public void PhysicalProductTax() 
        { 
            var product = new PhysicalProduct { Name = "Widget", Price = 100, Weight = 1 };
            var calc = new TaxCalculator();
            product.Accept(calc);
            Assert.That(calc.TotalTax, Is.EqualTo(10));
        }

        [Test]
        public void DigitalProductTax()
        {
            var product = new DigitalProduct { Name = "eBook", Price = 50 };
            var calc = new TaxCalculator();
            product.Accept(calc);
            Assert.That(calc.TotalTax, Is.EqualTo(2.5m));
        }

        [Test]
        public void ServiceProductTax()
        {
            var product = new ServiceProduct { Name = "Consulting", Price = 200, Hours = 2 };
            var calc = new TaxCalculator();
            product.Accept(calc);
            Assert.That(calc.TotalTax, Is.EqualTo(16));
        }

        [Test]
        public void BundleProductTax()
        {
            var bundle = new BundleProduct { Name = "Bundle" };
            bundle.Items.Add(new PhysicalProduct { Price = 100 });
            bundle.Items.Add(new DigitalProduct { Price = 50 });
            var calc = new TaxCalculator();
            bundle.Accept(calc);
            Assert.That(calc.TotalTax, Is.GreaterThan(0));
        }

        [Test]
        public void DiscountCalculator()
        {
            var product = new PhysicalProduct { Price = 100 };
            var discount = new DiscountCalculator();
            product.Accept(discount);
            Assert.That(discount.TotalDiscount, Is.EqualTo(5));
        }

        [Test]
        public void PriceExtractor()
        {
            var p1 = new PhysicalProduct { Price = 50 };
            var p2 = new DigitalProduct { Price = 25 };
            var extractor = new PriceExtractor();
            p1.Accept(extractor);
            p2.Accept(extractor);
            Assert.That(extractor.TotalPrice, Is.EqualTo(75));
            Assert.That(extractor.ItemCount, Is.EqualTo(2));
        }
    }
}
