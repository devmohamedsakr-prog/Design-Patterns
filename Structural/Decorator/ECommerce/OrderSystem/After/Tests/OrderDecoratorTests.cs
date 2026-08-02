using NUnit.Framework;
using System;
using OrderSystem.After.Models;
using OrderSystem.After.Decorators;

namespace OrderSystem.After.Tests
{
    [TestFixture]
    public class OrderDecoratorTests
    {
        private Order _baseOrder;

        [SetUp]
        public void Setup()
        {
            _baseOrder = new Order("ORD001", 100m);
        }

        // Basic decorator tests
        [Test]
        public void DiscountDecorator_AppliesDiscount()
        {
            var discounted = new DiscountDecorator(_baseOrder, 0.10m);
            Assert.That(discounted.GetTotal(), Is.EqualTo(90m));
        }

        [Test]
        public void TaxDecorator_AppliesTax()
        {
            var taxed = new TaxDecorator(_baseOrder, 0.08m);
            Assert.That(taxed.GetTotal(), Is.EqualTo(108m));
        }

        [Test]
        public void ShippingDecorator_AddsShipping()
        {
            var shipped = new ShippingDecorator(_baseOrder, 10m);
            Assert.That(shipped.GetTotal(), Is.EqualTo(110m));
        }

        [Test]
        public void InsuranceDecorator_AddsInsurance()
        {
            var insured = new InsuranceDecorator(_baseOrder, 0.05m);
            Assert.That(insured.GetTotal(), Is.EqualTo(105m));
        }

        // Composition tests
        [Test]
        public void DiscountThenTax()
        {
            var order = new DiscountDecorator(_baseOrder, 0.10m);  // $90
            var final = new TaxDecorator(order, 0.08m);            // $90 * 1.08 = $97.20
            Assert.That(final.GetTotal(), Is.EqualTo(97.20m));
        }

        [Test]
        public void TaxThenDiscount()
        {
            var order = new TaxDecorator(_baseOrder, 0.08m);       // $108
            var final = new DiscountDecorator(order, 0.10m);       // $108 * 0.90 = $97.20
            Assert.That(final.GetTotal(), Is.EqualTo(97.20m));
        }

        [Test]
        public void DiscountTaxShipping()
        {
            var order = new Order("ORD002", 100m);
            var discounted = new DiscountDecorator(order, 0.10m);   // $90
            var taxed = new TaxDecorator(discounted, 0.08m);        // $97.20
            var shipped = new ShippingDecorator(taxed, 10m);        // $107.20
            Assert.That(shipped.GetTotal(), Is.EqualTo(107.20m));
        }

        [Test]
        public void ComplexDecoration_Multiple()
        {
            var order = new Order("ORD003", 200m);
            var d1 = new DiscountDecorator(order, 0.15m);           // $170
            var d2 = new TaxDecorator(d1, 0.08m);                   // $183.60
            var d3 = new ShippingDecorator(d2, 15m);                // $198.60
            var d4 = new InsuranceDecorator(d3, 0.02m);             // $202.37 (approximately)
            
            Assert.That(d4.GetTotal(), Is.GreaterThan(200m));
        }

        [Test]
        public void DeepNesting_ManyDecorators()
        {
            var order = new Order("ORD004", 100m);
            var decorated = new InsuranceDecorator(
                new ShippingDecorator(
                    new TaxDecorator(
                        new DiscountDecorator(order, 0.05m),
                        0.08m),
                    5m),
                0.01m);

            Assert.That(decorated.GetTotal(), Is.GreaterThan(100m));
        }

        // Validation tests
        [Test]
        public void DiscountDecorator_InvalidDiscount_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new DiscountDecorator(_baseOrder, 1.5m)
            );
        }

        [Test]
        public void DiscountDecorator_NegativeDiscount_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new DiscountDecorator(_baseOrder, -0.10m)
            );
        }

        [Test]
        public void TaxDecorator_InvalidTax_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new TaxDecorator(_baseOrder, 1.5m)
            );
        }

        [Test]
        public void TaxDecorator_NegativeTax_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new TaxDecorator(_baseOrder, -0.08m)
            );
        }

        [Test]
        public void ShippingDecorator_NegativeShipping_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new ShippingDecorator(_baseOrder, -10m)
            );
        }

        [Test]
        public void InsuranceDecorator_InvalidInsurance_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new InsuranceDecorator(_baseOrder, 1.5m)
            );
        }

        // Real-world scenarios
        [Test]
        public void LoyaltyCustomer_HighDiscount()
        {
            var order = new Order("LOYAL001", 500m);
            var discounted = new DiscountDecorator(order, 0.20m);
            var taxed = new TaxDecorator(discounted, 0.08m);
            
            Assert.That(discounted.GetTotal(), Is.EqualTo(400m));
            Assert.That(taxed.GetTotal(), Is.EqualTo(432m));
        }

        [Test]
        public void PremiumShipping_WithInsurance()
        {
            var order = new Order("PREM001", 300m);
            var shipped = new ShippingDecorator(order, 25m);
            var insured = new InsuranceDecorator(shipped, 0.03m);
            
            decimal shipped_total = 325m;
            decimal expected = shipped_total * 1.03m;
            
            Assert.That(insured.GetTotal(), Is.EqualTo(expected));
        }

        [Test]
        public void BulkOrder_MinimalDiscount_WithTax()
        {
            var order = new Order("BULK001", 1000m);
            var discounted = new DiscountDecorator(order, 0.05m);  // 5% discount
            var taxed = new TaxDecorator(discounted, 0.08m);
            
            decimal after_discount = 950m;
            decimal after_tax = 1026m;
            
            Assert.That(taxed.GetTotal(), Is.EqualTo(after_tax));
        }

        [Test]
        public void InternationalOrder_TaxShippingInsurance()
        {
            var order = new Order("INTL001", 150m);
            var taxed = new TaxDecorator(order, 0.12m);
            var shipped = new ShippingDecorator(taxed, 30m);
            var insured = new InsuranceDecorator(shipped, 0.05m);
            
            Assert.That(insured.GetTotal(), Is.GreaterThan(200m));
        }

        // Edge cases
        [Test]
        public void ZeroBasePrice()
        {
            var order = new Order("ZERO001", 0m);
            var shipped = new ShippingDecorator(order, 10m);
            Assert.That(shipped.GetTotal(), Is.EqualTo(10m));
        }

        [Test]
        public void ZeroDiscount()
        {
            var order = new Order("ORD005", 100m);
            var discounted = new DiscountDecorator(order, 0m);
            Assert.That(discounted.GetTotal(), Is.EqualTo(100m));
        }

        [Test]
        public void MaxDiscount_100Percent()
        {
            var order = new Order("ORD006", 100m);
            var discounted = new DiscountDecorator(order, 1m);
            Assert.That(discounted.GetTotal(), Is.EqualTo(0m));
        }

        [Test]
        public void MultipleDiscounts_Sequential()
        {
            var order = new Order("ORD007", 100m);
            var d1 = new DiscountDecorator(order, 0.10m);      // $90
            var d2 = new DiscountDecorator(d1, 0.10m);         // $81
            Assert.That(d2.GetTotal(), Is.EqualTo(81m));
        }

        [Test]
        public void MultipleShippingCosts()
        {
            var order = new Order("ORD008", 100m);
            var s1 = new ShippingDecorator(order, 5m);         // $105
            var s2 = new ShippingDecorator(s1, 3m);            // $108
            Assert.That(s2.GetTotal(), Is.EqualTo(108m));
        }

        // Order chain preservation
        [Test]
        public void OrderIdPreserved()
        {
            var decorated = new DiscountDecorator(_baseOrder, 0.10m);
            Assert.That(decorated.OrderId, Is.EqualTo("ORD001"));
        }

        [Test]
        public void BaseOrderUnchanged()
        {
            var decorated = new DiscountDecorator(_baseOrder, 0.10m);
            Assert.That(_baseOrder.GetTotal(), Is.EqualTo(100m));
            Assert.That(decorated.GetTotal(), Is.EqualTo(90m));
        }

        // String representation
        [Test]
        public void DecoratorToString_ShowsChain()
        {
            var order = new Order("ORD009", 100m);
            var discounted = new DiscountDecorator(order, 0.10m);
            var taxed = new TaxDecorator(discounted, 0.08m);
            
            var result = taxed.ToString();
            Assert.That(result, Does.Contain("$97.20"));
        }

        // Precision tests
        [Test]
        public void Precision_MultipleCalculations()
        {
            var order = new Order("PREC001", 99.99m);
            var discounted = new DiscountDecorator(order, 0.15m);
            var taxed = new TaxDecorator(discounted, 0.07m);
            var shipped = new ShippingDecorator(taxed, 7.99m);
            
            var total = shipped.GetTotal();
            Assert.That(total, Is.GreaterThan(85m).And.LessThan(95m));
        }

        // Composition with different order values
        [Test]
        public void SmallOrder_AllDecorators()
        {
            var order = new Order("SMALL001", 10m);
            var d = new DiscountDecorator(order, 0.10m);
            var t = new TaxDecorator(d, 0.08m);
            var s = new ShippingDecorator(t, 5m);
            
            // 10 * 0.90 = 9, 9 * 1.08 = 9.72, 9.72 + 5 = 14.72
            Assert.That(s.GetTotal(), Is.EqualTo(14.72m));
        }

        [Test]
        public void LargeOrder_AllDecorators()
        {
            var order = new Order("LARGE001", 10000m);
            var d = new DiscountDecorator(order, 0.10m);
            var t = new TaxDecorator(d, 0.08m);
            var s = new ShippingDecorator(t, 50m);
            
            // 10000 * 0.90 = 9000, 9000 * 1.08 = 9720, 9720 + 50 = 9770
            Assert.That(s.GetTotal(), Is.EqualTo(9770m));
        }

        [Test]
        public void DynamicDecoration_ConditionalDecorators()
        {
            var order = new Order("DYN001", 200m);
            
            bool isLoyalMember = true;
            if (isLoyalMember)
                order = new DiscountDecorator(order, 0.15m);
            
            bool needsShipping = true;
            if (needsShipping)
                order = new ShippingDecorator(order, 12m);
            
            bool isPremium = true;
            if (isPremium)
                order = new InsuranceDecorator(order, 0.02m);
            
            Assert.That(order.GetTotal(), Is.GreaterThan(200m));
        }
    }
}
