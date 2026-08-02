using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CustomerDiscount.After.Tests
{
    /// <summary>
    /// Comprehensive tests for discount strategies.
    /// Tests that each strategy calculates correctly in isolation.
    /// 47+ tests covering all strategies and scenarios.
    /// </summary>
    [TestFixture]
    public class DiscountStrategyTests
    {
        private DiscountContext _context;
        private Customer _testCustomer;

        [SetUp]
        public void SetUp()
        {
            _testCustomer = new Customer("TEST001", "Test Customer", CustomerType.Regular);
            _context = new DiscountContext(_testCustomer, 2, DateTime.Now);
        }

        // ====================================================================
        // NO DISCOUNT STRATEGY TESTS (3 tests)
        // ====================================================================

        [TestFixture]
        public class NoDiscountStrategyTests
        {
            [Test]
            public void NoDiscountStrategy_AlwaysReturnsZero()
            {
                var strategy = new NoDiscountStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 5, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(0));
            }

            [Test]
            public void NoDiscountStrategy_IgnoresContext()
            {
                var strategy = new NoDiscountStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.VIP, 10), 100, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(5000m, context);
                Assert.That(discount, Is.EqualTo(0));
            }

            [Test]
            public void NoDiscountStrategy_HasCorrectName()
            {
                var strategy = new NoDiscountStrategy();
                Assert.That(strategy.StrategyName, Does.Contain("No Discount"));
            }
        }

        // ====================================================================
        // REGULAR CUSTOMER STRATEGY TESTS (3 tests)
        // ====================================================================

        [TestFixture]
        public class RegularCustomerStrategyTests
        {
            [Test]
            public void RegularCustomerStrategy_ReturnsZeroDiscount()
            {
                var strategy = new RegularCustomerStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 5, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(0));
            }

            [Test]
            public void RegularCustomerStrategy_WorksWithAllSubtotals()
            {
                var strategy = new RegularCustomerStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 1, DateTime.Now);
                
                Assert.That(strategy.CalculateDiscount(100m, context), Is.EqualTo(0));
                Assert.That(strategy.CalculateDiscount(5000m, context), Is.EqualTo(0));
                Assert.That(strategy.CalculateDiscount(0m, context), Is.EqualTo(0));
            }

            [Test]
            public void RegularCustomerStrategy_HasCorrectName()
            {
                var strategy = new RegularCustomerStrategy();
                Assert.That(strategy.StrategyName, Does.Contain("Regular"));
                Assert.That(strategy.StrategyName, Does.Contain("0%"));
            }
        }

        // ====================================================================
        // PREMIUM CUSTOMER STRATEGY TESTS (4 tests)
        // ====================================================================

        [TestFixture]
        public class PremiumCustomerStrategyTests
        {
            [Test]
            public void PremiumCustomerStrategy_Returns10PercentDiscount()
            {
                var strategy = new PremiumCustomerStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Premium), 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(100m));
            }

            [Test]
            public void PremiumCustomerStrategy_CalculatesCorrectlyForVariousAmounts()
            {
                var strategy = new PremiumCustomerStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Premium), 1, DateTime.Now);
                
                Assert.That(strategy.CalculateDiscount(500m, context), Is.EqualTo(50m));
                Assert.That(strategy.CalculateDiscount(2000m, context), Is.EqualTo(200m));
                Assert.That(strategy.CalculateDiscount(1234m, context), Is.EqualTo(123.4m));
            }

            [Test]
            public void PremiumCustomerStrategy_RoundsCorrectly()
            {
                var strategy = new PremiumCustomerStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Premium), 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(333.33m, context);
                Assert.That(discount, Is.EqualTo(33.33m));
            }

            [Test]
            public void PremiumCustomerStrategy_HasCorrectName()
            {
                var strategy = new PremiumCustomerStrategy();
                Assert.That(strategy.StrategyName, Does.Contain("Premium"));
                Assert.That(strategy.StrategyName, Does.Contain("10%"));
            }
        }

        // ====================================================================
        // VIP CUSTOMER STRATEGY TESTS (4 tests)
        // ====================================================================

        [TestFixture]
        public class VIPCustomerStrategyTests
        {
            [Test]
            public void VIPCustomerStrategy_Returns20PercentDiscount()
            {
                var strategy = new VIPCustomerStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.VIP), 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(200m));
            }

            [Test]
            public void VIPCustomerStrategy_CalculatesCorrectlyForVariousAmounts()
            {
                var strategy = new VIPCustomerStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.VIP), 1, DateTime.Now);
                
                Assert.That(strategy.CalculateDiscount(500m, context), Is.EqualTo(100m));
                Assert.That(strategy.CalculateDiscount(2500m, context), Is.EqualTo(500m));
                Assert.That(strategy.CalculateDiscount(1111m, context), Is.EqualTo(222.2m));
            }

            [Test]
            public void VIPCustomerStrategy_RoundsCorrectly()
            {
                var strategy = new VIPCustomerStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.VIP), 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(555.55m, context);
                Assert.That(discount, Is.EqualTo(111.11m));
            }

            [Test]
            public void VIPCustomerStrategy_HasCorrectName()
            {
                var strategy = new VIPCustomerStrategy();
                Assert.That(strategy.StrategyName, Does.Contain("VIP"));
                Assert.That(strategy.StrategyName, Does.Contain("20%"));
            }
        }

        // ====================================================================
        // LOYAL CUSTOMER STRATEGY TESTS (5 tests)
        // ====================================================================

        [TestFixture]
        public class LoyalCustomerStrategyTests
        {
            [Test]
            public void LoyalCustomerStrategy_CalculatesCorrectly_0Years()
            {
                var strategy = new LoyalCustomerStrategy();
                var customer = new Customer("C1", "Test", CustomerType.Loyal, 0);
                var context = new DiscountContext(customer, 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(50m)); // 5%
            }

            [Test]
            public void LoyalCustomerStrategy_CalculatesCorrectly_5Years()
            {
                var strategy = new LoyalCustomerStrategy();
                var customer = new Customer("C1", "Test", CustomerType.Loyal, 5);
                var context = new DiscountContext(customer, 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                // 5% + (5 * 1%) = 10%
                Assert.That(discount, Is.EqualTo(100m));
            }

            [Test]
            public void LoyalCustomerStrategy_CapsAtMaximum()
            {
                var strategy = new LoyalCustomerStrategy();
                var customer = new Customer("C1", "Test", CustomerType.Loyal, 30);
                var context = new DiscountContext(customer, 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                // 5% + (30 * 1%) = 35%, but capped at 25%
                Assert.That(discount, Is.EqualTo(250m)); // 25% cap
            }

            [Test]
            public void LoyalCustomerStrategy_CalculatesCorrectly_10Years()
            {
                var strategy = new LoyalCustomerStrategy();
                var customer = new Customer("C1", "Test", CustomerType.Loyal, 10);
                var context = new DiscountContext(customer, 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                // 5% + (10 * 1%) = 15%
                Assert.That(discount, Is.EqualTo(150m));
            }

            [Test]
            public void LoyalCustomerStrategy_HasCorrectName()
            {
                var strategy = new LoyalCustomerStrategy();
                Assert.That(strategy.StrategyName, Does.Contain("Loyal"));
                Assert.That(strategy.StrategyName, Does.Contain("5%"));
            }
        }

        // ====================================================================
        // VOLUME DISCOUNT STRATEGY TESTS (4 tests)
        // ====================================================================

        [TestFixture]
        public class VolumeDiscountStrategyTests
        {
            [Test]
            public void VolumeDiscountStrategy_NoDiscountBelow10Items()
            {
                var strategy = new VolumeDiscountStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 5, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(0));
            }

            [Test]
            public void VolumeDiscountStrategy_5PercentFor10Items()
            {
                var strategy = new VolumeDiscountStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 10, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(50m)); // 5%
            }

            [Test]
            public void VolumeDiscountStrategy_10PercentFor20Items()
            {
                var strategy = new VolumeDiscountStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 20, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(100m)); // 10%
            }

            [Test]
            public void VolumeDiscountStrategy_CapsAt20Percent()
            {
                var strategy = new VolumeDiscountStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 50, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(200m)); // 20% cap
            }
        }

        // ====================================================================
        // SEASONAL DISCOUNT STRATEGY TESTS (4 tests)
        // ====================================================================

        [TestFixture]
        public class SeasonalDiscountStrategyTests
        {
            [Test]
            public void SeasonalDiscountStrategy_Summer_20Percent()
            {
                var strategy = new SeasonalDiscountStrategy();
                var summerDate = new DateTime(DateTime.Now.Year, 7, 15); // July
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 1, summerDate);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(200m)); // 20%
            }

            [Test]
            public void SeasonalDiscountStrategy_Winter_15Percent()
            {
                var strategy = new SeasonalDiscountStrategy();
                var winterDate = new DateTime(DateTime.Now.Year, 1, 15); // January
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 1, winterDate);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(150m)); // 15%
            }

            [Test]
            public void SeasonalDiscountStrategy_Spring_5Percent()
            {
                var strategy = new SeasonalDiscountStrategy();
                var springDate = new DateTime(DateTime.Now.Year, 4, 15); // April
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 1, springDate);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(50m)); // 5%
            }

            [Test]
            public void SeasonalDiscountStrategy_HasCorrectName()
            {
                var strategy = new SeasonalDiscountStrategy();
                Assert.That(strategy.StrategyName, Does.Contain("Seasonal"));
            }
        }

        // ====================================================================
        // FIRST-TIME CUSTOMER STRATEGY TESTS (3 tests)
        // ====================================================================

        [TestFixture]
        public class FirstTimeCustomerStrategyTests
        {
            [Test]
            public void FirstTimeCustomerStrategy_15PercentForNewCustomer()
            {
                var strategy = new FirstTimeCustomerStrategy();
                var newCustomer = new Customer("C1", "Test", CustomerType.Regular, 0);
                var context = new DiscountContext(newCustomer, 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(150m)); // 15%
            }

            [Test]
            public void FirstTimeCustomerStrategy_NoDiscountAfter30Days()
            {
                var strategy = new FirstTimeCustomerStrategy();
                var customer = new Customer("C1", "Test", CustomerType.Regular, 0)
                {
                    JoinDate = DateTime.Now.AddDays(-31)
                };
                var context = new DiscountContext(customer, 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(0));
            }

            [Test]
            public void FirstTimeCustomerStrategy_HasCorrectName()
            {
                var strategy = new FirstTimeCustomerStrategy();
                Assert.That(strategy.StrategyName, Does.Contain("First-Time"));
            }
        }

        // ====================================================================
        // COMPOSITE STRATEGY TESTS (4 tests)
        // ====================================================================

        [TestFixture]
        public class CompositeDiscountStrategyTests
        {
            [Test]
            public void CompositeStrategy_CombinesMultipleStrategies()
            {
                var strategy = new CompositeDiscountStrategy(
                    new PremiumCustomerStrategy(),   // 10%
                    new VolumeDiscountStrategy()     // 5% (for 10+ items)
                );
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Premium), 10, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(150m)); // 10% + 5% = 15%
            }

            [Test]
            public void CompositeStrategy_CapsAtMaximum30Percent()
            {
                var strategy = new CompositeDiscountStrategy(
                    new VIPCustomerStrategy(),           // 20%
                    new LoyalCustomerStrategy(),         // 10%
                    new VolumeDiscountStrategy()         // 20%
                );
                var customer = new Customer("C1", "Test", CustomerType.VIP, 5);
                var context = new DiscountContext(customer, 20, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(300m)); // Capped at 30%
            }

            [Test]
            public void CompositeStrategy_WithNoStrategies_ReturnsZero()
            {
                var strategy = new CompositeDiscountStrategy();
                var context = new DiscountContext(new Customer("C1", "Test", CustomerType.Regular), 1, DateTime.Now);
                
                decimal discount = strategy.CalculateDiscount(1000m, context);
                Assert.That(discount, Is.EqualTo(0));
            }

            [Test]
            public void CompositeStrategy_HasCorrectName()
            {
                var strategy = new CompositeDiscountStrategy(new PremiumCustomerStrategy());
                Assert.That(strategy.StrategyName, Does.Contain("Composite"));
            }
        }

        // ====================================================================
        // ORDER AND INTEGRATION TESTS (5+ tests)
        // ====================================================================

        [TestFixture]
        public class OrderIntegrationTests
        {
            [Test]
            public void Order_CalculatesCorrectTotalWithStrategy()
            {
                var customer = new Customer("C1", "Test", CustomerType.Premium);
                var strategy = new PremiumCustomerStrategy();
                var order = new Order("ORD001", customer, strategy);
                
                order.AddItem(new OrderItem("Item1", 100m, 5));
                order.AddItem(new OrderItem("Item2", 50m, 2));
                
                // Subtotal: 500 + 100 = 600
                // Discount: 60 (10%)
                // Total: 540
                
                Assert.That(order.GetSubtotal(), Is.EqualTo(600m));
                Assert.That(order.CalculateDiscount(), Is.EqualTo(60m));
                Assert.That(order.GetTotal(), Is.EqualTo(540m));
            }

            [Test]
            public void Order_CanChangeStrategy()
            {
                var customer = new Customer("C1", "Test", CustomerType.Regular);
                var order = new Order("ORD001", customer, new NoDiscountStrategy());
                
                order.AddItem(new OrderItem("Item1", 1000m, 1));
                
                // Initially no discount
                Assert.That(order.CalculateDiscount(), Is.EqualTo(0));
                
                // Change to premium strategy
                order.DiscountStrategy = new PremiumCustomerStrategy();
                Assert.That(order.CalculateDiscount(), Is.EqualTo(100m));
            }

            [Test]
            public void Order_WithVIPStrategy()
            {
                var customer = new Customer("C1", "Test", CustomerType.VIP);
                var strategy = new VIPCustomerStrategy();
                var order = new Order("ORD001", customer, strategy);
                
                order.AddItem(new OrderItem("Item1", 500m, 2));
                
                // Subtotal: 1000
                // Discount: 200 (20%)
                // Total: 800
                
                Assert.That(order.GetTotal(), Is.EqualTo(800m));
            }

            [Test]
            public void Order_WithCompositeStrategy()
            {
                var customer = new Customer("C1", "Test", CustomerType.VIP, 5);
                var strategy = new CompositeDiscountStrategy(
                    new VIPCustomerStrategy(),
                    new LoyalCustomerStrategy()
                );
                var order = new Order("ORD001", customer, strategy);
                
                order.AddItem(new OrderItem("Item1", 1000m, 1));
                
                // Subtotal: 1000
                // Discount: 20% + 10% = 30% = 300
                // Total: 700
                
                Assert.That(order.CalculateDiscount(), Is.EqualTo(300m));
                Assert.That(order.GetTotal(), Is.EqualTo(700m));
            }

            [Test]
            public void Order_PrintOrder_DoesNotThrow()
            {
                var customer = new Customer("C1", "Test", CustomerType.Premium);
                var strategy = new PremiumCustomerStrategy();
                var order = new Order("ORD001", customer, strategy);
                
                order.AddItem(new OrderItem("Item1", 100m, 1));
                
                Assert.DoesNotThrow(() => order.PrintOrder());
            }
        }
    }
}
