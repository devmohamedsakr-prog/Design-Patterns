using NUnit.Framework;
using System.Threading.Tasks;
using ShippingMethod.After.Abstracts;
using ShippingMethod.After.Creators;

namespace ShippingMethod.After.Tests
{
    [TestFixture]
    public class ShippingMethodTests
    {
        [Test]
        public async Task StandardShipping_CalculateCost() => Assert.That((await new StandardShippingCreator().CalculateShippingAsync(10, "NYC", 100)).Cost, Is.EqualTo(5));

        [Test]
        public async Task ExpressShipping_CalculateCost() => Assert.That((await new ExpressShippingCreator().CalculateShippingAsync(10, "NYC", 100)).Cost, Is.EqualTo(15));

        [Test]
        public async Task OvernightShipping_CalculateCost() => Assert.That((await new OvernightShippingCreator().CalculateShippingAsync(10, "NYC", 100)).Cost, Is.EqualTo(30));

        [Test]
        public async Task StandardShipping_DeliveryDays() => Assert.That((await new StandardShippingCreator().CalculateShippingAsync(5, "LA", 50)).DeliveryDays, Is.EqualTo(5));

        [Test]
        public async Task ExpressShipping_DeliveryDays() => Assert.That((await new ExpressShippingCreator().CalculateShippingAsync(5, "LA", 50)).DeliveryDays, Is.EqualTo(2));

        [Test]
        public async Task OvernightShipping_DeliveryDays() => Assert.That((await new OvernightShippingCreator().CalculateShippingAsync(5, "LA", 50)).DeliveryDays, Is.EqualTo(1));

        [Test]
        public async Task StandardShipping_GeneratesTrackingId() => Assert.That((await new StandardShippingCreator().CalculateShippingAsync(5, "CHI", 50)).TrackingId, Does.StartWith("std_"));

        [Test]
        public async Task ExpressShipping_GeneratesTrackingId() => Assert.That((await new ExpressShippingCreator().CalculateShippingAsync(5, "CHI", 50)).TrackingId, Does.StartWith("exp_"));

        [Test]
        public async Task OvernightShipping_GeneratesTrackingId() => Assert.That((await new OvernightShippingCreator().CalculateShippingAsync(5, "CHI", 50)).TrackingId, Does.StartWith("ovr_"));

        [Test]
        public async Task AllMethods_ReturnSuccess() 
        {
            var r1 = await new StandardShippingCreator().CalculateShippingAsync(5, "NYC", 100);
            var r2 = await new ExpressShippingCreator().CalculateShippingAsync(5, "NYC", 100);
            var r3 = await new OvernightShippingCreator().CalculateShippingAsync(5, "NYC", 100);
            Assert.That(r1.Success && r2.Success && r3.Success);
        }

        [Test]
        public async Task InvalidWeight_ShouldFail() => Assert.That((await new StandardShippingCreator().CalculateShippingAsync(0, "NYC", 100)).Success, Is.False);

        [Test]
        public async Task NullDestination_ShouldFail() => Assert.That((await new StandardShippingCreator().CalculateShippingAsync(5, null, 100)).Success, Is.False);

        [Test]
        public async Task DifferentWeights_CalculateDifferentCosts()
        {
            var r1 = await new StandardShippingCreator().CalculateShippingAsync(5, "NYC", 100);
            var r2 = await new StandardShippingCreator().CalculateShippingAsync(10, "NYC", 100);
            Assert.That(r1.Cost, Is.LessThan(r2.Cost));
        }

        [Test]
        public async Task MethodNames_Correct()
        {
            Assert.That((await new StandardShippingCreator().CalculateShippingAsync(5, "NYC", 100)).MethodName, Is.EqualTo("Standard"));
            Assert.That((await new ExpressShippingCreator().CalculateShippingAsync(5, "NYC", 100)).MethodName, Is.EqualTo("Express"));
            Assert.That((await new OvernightShippingCreator().CalculateShippingAsync(5, "NYC", 100)).MethodName, Is.EqualTo("Overnight"));
        }

        [Test]
        public async Task MultipleDestinations_AllSucceed()
        {
            var destinations = new[] { "NYC", "LA", "CHI", "SEA", "DEN" };
            foreach (var dest in destinations)
            {
                var result = await new StandardShippingCreator().CalculateShippingAsync(10, dest, 100);
                Assert.That(result.Success);
            }
        }

        [Test]
        public async Task VaryingWeights_AllSucceed()
        {
            var weights = new[] { 0.5m, 1m, 5m, 10m, 50m };
            foreach (var weight in weights)
            {
                var result = await new ExpressShippingCreator().CalculateShippingAsync(weight, "NYC", 100);
                Assert.That(result.Success);
                Assert.That(result.Cost, Is.GreaterThan(0));
            }
        }

        [Test]
        public async Task FactoryMethod_CreatorsDifferent()
        {
            var s = await new StandardShippingCreator().CalculateShippingAsync(10, "NYC", 100);
            var e = await new ExpressShippingCreator().CalculateShippingAsync(10, "NYC", 100);
            var o = await new OvernightShippingCreator().CalculateShippingAsync(10, "NYC", 100);
            
            Assert.That(s.Cost, Is.LessThan(e.Cost));
            Assert.That(e.Cost, Is.LessThan(o.Cost));
        }

        [Test]
        public async Task HighValue_Packages_StillCalculate()
        {
            var result = await new OvernightShippingCreator().CalculateShippingAsync(50, "NYC", 50000);
            Assert.That(result.Success);
            Assert.That(result.Cost, Is.GreaterThan(100));
        }

        [Test]
        public async Task Sequential_ShippingCalculations()
        {
            var c1 = new StandardShippingCreator();
            var c2 = new ExpressShippingCreator();
            var c3 = new OvernightShippingCreator();

            var r1 = await c1.CalculateShippingAsync(5, "NYC", 100);
            var r2 = await c2.CalculateShippingAsync(5, "LA", 200);
            var r3 = await c3.CalculateShippingAsync(5, "CHI", 150);

            Assert.That(r1.Success && r2.Success && r3.Success);
        }

        [Test]
        public async Task Tracking_IdsUnique()
        {
            var creator = new StandardShippingCreator();
            var ids = new System.Collections.Generic.HashSet<string>();
            
            for (int i = 0; i < 10; i++)
            {
                var result = await creator.CalculateShippingAsync(5, $"City{i}", 100);
                ids.Add(result.TrackingId);
            }
            
            Assert.That(ids.Count, Is.EqualTo(10));
        }

        [Test]
        public async Task EmptyDestination_ShouldFail() => Assert.That((await new StandardShippingCreator().CalculateShippingAsync(5, "", 100)).Success, Is.False);

        [Test]
        public async Task NegativeWeight_ShouldFail() => Assert.That((await new ExpressShippingCreator().CalculateShippingAsync(-5, "NYC", 100)).Success, Is.False);

        [Test]
        public async Task AllMethods_HaveMessages()
        {
            var r1 = await new StandardShippingCreator().CalculateShippingAsync(5, "NYC", 100);
            var r2 = await new ExpressShippingCreator().CalculateShippingAsync(5, "NYC", 100);
            var r3 = await new OvernightShippingCreator().CalculateShippingAsync(5, "NYC", 100);
            
            Assert.That(r1.Message, Is.Not.Null);
            Assert.That(r2.Message, Is.Not.Null);
            Assert.That(r3.Message, Is.Not.Null);
        }

        [Test]
        public async Task Cost_Comparison_Standard_LessThan_Express() 
        {
            var s = await new StandardShippingCreator().CalculateShippingAsync(10, "NYC", 100);
            var e = await new ExpressShippingCreator().CalculateShippingAsync(10, "NYC", 100);
            Assert.That(s.Cost, Is.LessThan(e.Cost));
        }

        [Test]
        public async Task Cost_Comparison_Express_LessThan_Overnight()
        {
            var e = await new ExpressShippingCreator().CalculateShippingAsync(10, "NYC", 100);
            var o = await new OvernightShippingCreator().CalculateShippingAsync(10, "NYC", 100);
            Assert.That(e.Cost, Is.LessThan(o.Cost));
        }

        [Test]
        public async Task Bulk_Shipments()
        {
            int successCount = 0;
            for (int i = 0; i < 20; i++)
            {
                var result = await new StandardShippingCreator().CalculateShippingAsync(5, $"City{i}", 100);
                if (result.Success) successCount++;
            }
            Assert.That(successCount, Is.GreaterThanOrEqualTo(18));
        }
    }
}
