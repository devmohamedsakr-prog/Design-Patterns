using NUnit.Framework;
using System.Threading.Tasks;
using DiscountStrategy.After.Abstracts;
using DiscountStrategy.After.Creators;

namespace DiscountStrategy.After.Tests
{
    [TestFixture]
    public class DiscountStrategyTests
    {
        [Test] public async Task PercentageDiscount_10Percent() => Assert.That((await new PercentageDiscountCreator(10).ApplyDiscountAsync(100, 1)).DiscountAmount, Is.EqualTo(10));
        [Test] public async Task PercentageDiscount_20Percent() => Assert.That((await new PercentageDiscountCreator(20).ApplyDiscountAsync(100, 1)).DiscountAmount, Is.EqualTo(20));
        [Test] public async Task PercentageDiscount_FinalAmount() => Assert.That((await new PercentageDiscountCreator(10).ApplyDiscountAsync(100, 1)).FinalAmount, Is.EqualTo(90));

        [Test] public async Task FixedDiscount_10Dollar() => Assert.That((await new FixedDiscountCreator(10).ApplyDiscountAsync(100, 1)).DiscountAmount, Is.EqualTo(10));
        [Test] public async Task FixedDiscount_25Dollar() => Assert.That((await new FixedDiscountCreator(25).ApplyDiscountAsync(100, 1)).DiscountAmount, Is.EqualTo(25));
        [Test] public async Task FixedDiscount_FinalAmount() => Assert.That((await new FixedDiscountCreator(10).ApplyDiscountAsync(100, 1)).FinalAmount, Is.EqualTo(90));

        [Test] public async Task BogoDiscount_SingleItem_NoDiscount() => Assert.That((await new BogoDiscountCreator().ApplyDiscountAsync(100, 1)).DiscountAmount, Is.EqualTo(0));
        [Test] public async Task BogoDiscount_TwoItems_OneDiscount() => Assert.That((await new BogoDiscountCreator().ApplyDiscountAsync(100, 2)).DiscountAmount, Is.GreaterThan(0));
        [Test] public async Task BogoDiscount_ThreeItems() => Assert.That((await new BogoDiscountCreator().ApplyDiscountAsync(300, 3)).DiscountAmount, Is.EqualTo(100));

        [Test]
        public async Task AllStrategies_ReturnSuccess()
        {
            var p = await new PercentageDiscountCreator().ApplyDiscountAsync(100, 1);
            var f = await new FixedDiscountCreator().ApplyDiscountAsync(100, 1);
            var b = await new BogoDiscountCreator().ApplyDiscountAsync(100, 2);
            Assert.That(p.Success && f.Success && b.Success);
        }

        [Test]
        public async Task InvalidAmount_ShouldFail() => Assert.That((await new PercentageDiscountCreator().ApplyDiscountAsync(0, 1)).Success, Is.False);

        [Test]
        public async Task ZeroQuantity_ShouldFail() => Assert.That((await new FixedDiscountCreator().ApplyDiscountAsync(100, 0)).Success, Is.False);

        [Test]
        public async Task StrategyNames_Correct()
        {
            Assert.That((await new PercentageDiscountCreator().ApplyDiscountAsync(100, 1)).StrategyName, Is.EqualTo("Percentage"));
            Assert.That((await new FixedDiscountCreator().ApplyDiscountAsync(100, 1)).StrategyName, Is.EqualTo("Fixed"));
            Assert.That((await new BogoDiscountCreator().ApplyDiscountAsync(100, 2)).StrategyName, Is.EqualTo("BOGO"));
        }

        [Test]
        public async Task HighAmount_Discounts()
        {
            var result = await new PercentageDiscountCreator(15).ApplyDiscountAsync(10000, 1);
            Assert.That(result.DiscountAmount, Is.EqualTo(1500));
        }

        [Test]
        public async Task MultipleQuantities_Bogo()
        {
            var r1 = await new BogoDiscountCreator().ApplyDiscountAsync(200, 1);
            var r2 = await new BogoDiscountCreator().ApplyDiscountAsync(200, 2);
            var r3 = await new BogoDiscountCreator().ApplyDiscountAsync(200, 4);
            
            Assert.That(r1.DiscountAmount, Is.EqualTo(0));
            Assert.That(r2.DiscountAmount, Is.GreaterThan(0));
            Assert.That(r3.DiscountAmount, Is.GreaterThan(r2.DiscountAmount));
        }

        [Test]
        public async Task PercentageVsFixed_Comparison()
        {
            var pct = await new PercentageDiscountCreator(10).ApplyDiscountAsync(1000, 1);
            var fix = await new FixedDiscountCreator(50).ApplyDiscountAsync(1000, 1);
            
            Assert.That(pct.DiscountAmount, Is.EqualTo(100));
            Assert.That(fix.DiscountAmount, Is.EqualTo(50));
            Assert.That(pct.DiscountAmount, Is.GreaterThan(fix.DiscountAmount));
        }

        [Test]
        public async Task FinalAmountNeverNegative()
        {
            var result = await new FixedDiscountCreator(200).ApplyDiscountAsync(100, 1);
            Assert.That(result.FinalAmount, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task BulkDiscounts()
        {
            int count = 0;
            for (int i = 0; i < 15; i++)
            {
                var result = await new PercentageDiscountCreator(10).ApplyDiscountAsync(100, i + 1);
                if (result.Success) count++;
            }
            Assert.That(count, Is.GreaterThanOrEqualTo(14));
        }

        [Test]
        public async Task DiscountCodes_Generated()
        {
            var pct = await new PercentageDiscountCreator(25).ApplyDiscountAsync(100, 1);
            var fix = await new FixedDiscountCreator(15).ApplyDiscountAsync(100, 1);
            var bogo = await new BogoDiscountCreator().ApplyDiscountAsync(100, 2);
            
            Assert.That(pct.DiscountCode, Is.Not.Null);
            Assert.That(fix.DiscountCode, Is.Not.Null);
            Assert.That(bogo.DiscountCode, Is.Not.Null);
        }

        [Test]
        public async Task NegativeAmount_ShouldFail() => Assert.That((await new PercentageDiscountCreator().ApplyDiscountAsync(-100, 1)).Success, Is.False);

        [Test]
        public async Task SequentialApplications()
        {
            var r1 = await new PercentageDiscountCreator(10).ApplyDiscountAsync(100, 1);
            var r2 = await new FixedDiscountCreator(20).ApplyDiscountAsync(100, 1);
            var r3 = await new BogoDiscountCreator().ApplyDiscountAsync(100, 2);
            
            Assert.That(r1.Success && r2.Success && r3.Success);
        }

        [Test]
        public async Task Percentage_EdgeCases()
        {
            var r1 = await new PercentageDiscountCreator(1).ApplyDiscountAsync(1000, 1);
            var r2 = await new PercentageDiscountCreator(99).ApplyDiscountAsync(1000, 1);
            
            Assert.That(r1.DiscountAmount, Is.EqualTo(10));
            Assert.That(r2.DiscountAmount, Is.EqualTo(990));
        }
    }
}
