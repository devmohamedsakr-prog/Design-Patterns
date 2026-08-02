using NUnit.Framework;
using CustomerDiscountFactory;

namespace CustomerDiscountFactory.Tests
{
    [TestFixture]
    public class CustomerDiscountFactoryTests
    {
        private OrderProcessor orderProcessor;

        [SetUp]
        public void Setup()
        {
            orderProcessor = new OrderProcessor();
        }

        #region Factory Creation Tests

        [TestFixture]
        public class FactoryCreationTests
        {
            [Test]
            public void CreateByTierName_Bronze_ReturnsBronzeTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                Assert.That(tier.TierName, Is.EqualTo("Bronze"));
            }

            [Test]
            public void CreateByTierName_Silver_ReturnsSilverTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("silver");
                Assert.That(tier.TierName, Is.EqualTo("Silver"));
            }

            [Test]
            public void CreateByTierName_Gold_ReturnsGoldTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("gold");
                Assert.That(tier.TierName, Is.EqualTo("Gold"));
            }

            [Test]
            public void CreateByTierName_Regular_ReturnsRegularTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("regular");
                Assert.That(tier.TierName, Is.EqualTo("Regular"));
            }

            [Test]
            public void CreateByTierName_CaseInsensitive_ReturnsTier()
            {
                var tierLower = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                var tierUpper = CustomerDiscountLevelFactory.CreateByTierName("BRONZE");
                var tierMixed = CustomerDiscountLevelFactory.CreateByTierName("SiLvEr");

                Assert.That(tierLower.TierName, Is.EqualTo("Bronze"));
                Assert.That(tierUpper.TierName, Is.EqualTo("Bronze"));
                Assert.That(tierMixed.TierName, Is.EqualTo("Silver"));
            }

            [Test]
            public void CreateByTierName_InvalidTier_ThrowsException()
            {
                Assert.Throws<ArgumentException>(() => 
                    CustomerDiscountLevelFactory.CreateByTierName("platinum"));
            }

            [Test]
            public void CreateByTierName_NullTier_ThrowsException()
            {
                Assert.Throws<ArgumentNullException>(() => 
                    CustomerDiscountLevelFactory.CreateByTierName(null));
            }

            [Test]
            public void CreateByTierType_Bronze_ReturnsBronzeTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierType(CustomerTierType.Bronze);
                Assert.That(tier.TierName, Is.EqualTo("Bronze"));
            }

            [Test]
            public void CreateByTierType_Gold_ReturnsGoldTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierType(CustomerTierType.Gold);
                Assert.That(tier.TierName, Is.EqualTo("Gold"));
            }
        }

        #endregion

        #region Bronze Tier Tests

        [TestFixture]
        public class BronzeTierTests
        {
            [Test]
            public void BronzeTier_Discount_Returns5Percent()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                var discount = tier.GetDiscountPercentage(100m);
                Assert.That(discount, Is.EqualTo(5m));
            }

            [Test]
            public void BronzeTier_Shipping_Returns10Dollars()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                var shipping = tier.GetShippingCost(100m);
                Assert.That(shipping, Is.EqualTo(10m));
            }

            [Test]
            public void BronzeTier_LoyaltyPoints_Returns1PointPer10Dollars()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                var points = tier.GetLoyaltyPoints(100m);
                Assert.That(points, Is.EqualTo(10));
            }

            [Test]
            public void BronzeTier_LoyaltyPoints_CalculatesCorrectly()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                Assert.That(tier.GetLoyaltyPoints(50m), Is.EqualTo(5));
                Assert.That(tier.GetLoyaltyPoints(99m), Is.EqualTo(9));
                Assert.That(tier.GetLoyaltyPoints(150m), Is.EqualTo(15));
            }

            [Test]
            public void BronzeTier_EligibleForPromotion_ReturnsTrue()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                Assert.That(tier.IsEligibleForPromotion(), Is.True);
            }
        }

        #endregion

        #region Silver Tier Tests

        [TestFixture]
        public class SilverTierTests
        {
            [Test]
            public void SilverTier_Discount_Returns10Percent()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("silver");
                var discount = tier.GetDiscountPercentage(100m);
                Assert.That(discount, Is.EqualTo(10m));
            }

            [Test]
            public void SilverTier_Shipping_Returns5Dollars()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("silver");
                var shipping = tier.GetShippingCost(100m);
                Assert.That(shipping, Is.EqualTo(5m));
            }

            [Test]
            public void SilverTier_LoyaltyPoints_Returns1PointPer5Dollars()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("silver");
                var points = tier.GetLoyaltyPoints(100m);
                Assert.That(points, Is.EqualTo(20));
            }

            [Test]
            public void SilverTier_LoyaltyPoints_CalculatesCorrectly()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("silver");
                Assert.That(tier.GetLoyaltyPoints(50m), Is.EqualTo(10));
                Assert.That(tier.GetLoyaltyPoints(75m), Is.EqualTo(15));
            }
        }

        #endregion

        #region Gold Tier Tests

        [TestFixture]
        public class GoldTierTests
        {
            [Test]
            public void GoldTier_Discount_Returns15Percent()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("gold");
                var discount = tier.GetDiscountPercentage(100m);
                Assert.That(discount, Is.EqualTo(15m));
            }

            [Test]
            public void GoldTier_Shipping_ReturnsFree()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("gold");
                var shipping = tier.GetShippingCost(100m);
                Assert.That(shipping, Is.EqualTo(0m));
            }

            [Test]
            public void GoldTier_LoyaltyPoints_Returns1PointPer2Dollars()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("gold");
                var points = tier.GetLoyaltyPoints(100m);
                Assert.That(points, Is.EqualTo(50));
            }

            [Test]
            public void GoldTier_EligibleForPromotion_ReturnsTrue()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("gold");
                Assert.That(tier.IsEligibleForPromotion(), Is.True);
            }
        }

        #endregion

        #region Order Processing Tests

        [TestFixture]
        public class OrderProcessingTests
        {
            private OrderProcessor processor;

            [SetUp]
            public void Setup()
            {
                processor = new OrderProcessor();
            }

            [Test]
            public void ProcessOrder_BronzeCustomer_CalculatesCorrectly()
            {
                var customer = new Customer { CustomerId = "C001", Name = "John", TierLevel = "bronze" };
                var summary = processor.ProcessOrder(customer, 100m);

                Assert.That(summary.DiscountAmount, Is.EqualTo(5m));
                Assert.That(summary.ShippingCost, Is.EqualTo(10m));
                Assert.That(summary.LoyaltyPointsEarned, Is.EqualTo(10));
                Assert.That(summary.Total, Is.EqualTo(105m)); // 100 - 5 + 10
            }

            [Test]
            public void ProcessOrder_SilverCustomer_CalculatesCorrectly()
            {
                var customer = new Customer { CustomerId = "C002", Name = "Jane", TierLevel = "silver" };
                var summary = processor.ProcessOrder(customer, 100m);

                Assert.That(summary.DiscountAmount, Is.EqualTo(10m));
                Assert.That(summary.ShippingCost, Is.EqualTo(5m));
                Assert.That(summary.LoyaltyPointsEarned, Is.EqualTo(20));
                Assert.That(summary.Total, Is.EqualTo(95m)); // 100 - 10 + 5
            }

            [Test]
            public void ProcessOrder_GoldCustomer_CalculatesCorrectly()
            {
                var customer = new Customer { CustomerId = "C003", Name = "Jack", TierLevel = "gold" };
                var summary = processor.ProcessOrder(customer, 100m);

                Assert.That(summary.DiscountAmount, Is.EqualTo(15m));
                Assert.That(summary.ShippingCost, Is.EqualTo(0m));
                Assert.That(summary.LoyaltyPointsEarned, Is.EqualTo(50));
                Assert.That(summary.Total, Is.EqualTo(85m)); // 100 - 15 + 0
            }

            [Test]
            public void ProcessOrder_RegularCustomer_NoDiscount()
            {
                var customer = new Customer { CustomerId = "C004", Name = "Bob", TierLevel = "regular" };
                var summary = processor.ProcessOrder(customer, 100m);

                Assert.That(summary.DiscountAmount, Is.EqualTo(0m));
                Assert.That(summary.ShippingCost, Is.EqualTo(15m));
                Assert.That(summary.LoyaltyPointsEarned, Is.EqualTo(0));
                Assert.That(summary.Total, Is.EqualTo(115m)); // 100 - 0 + 15
            }

            [Test]
            public void ProcessOrder_LargeOrder_CalculatesCorrectly()
            {
                var customer = new Customer { CustomerId = "C005", Name = "VIP", TierLevel = "gold" };
                var summary = processor.ProcessOrder(customer, 1000m);

                Assert.That(summary.DiscountAmount, Is.EqualTo(150m));
                Assert.That(summary.ShippingCost, Is.EqualTo(0m));
                Assert.That(summary.LoyaltyPointsEarned, Is.EqualTo(500));
                Assert.That(summary.Total, Is.EqualTo(850m));
            }

            [Test]
            public void ProcessOrder_NullCustomer_ThrowsException()
            {
                Assert.Throws<ArgumentNullException>(() => processor.ProcessOrder(null, 100m));
            }

            [Test]
            public void ProcessOrder_NegativeAmount_ThrowsException()
            {
                var customer = new Customer { CustomerId = "C001", Name = "John", TierLevel = "bronze" };
                Assert.Throws<ArgumentException>(() => processor.ProcessOrder(customer, -50m));
            }
        }

        #endregion

        #region Factory Methods Tests

        [TestFixture]
        public class FactoryMethodsTests
        {
            [Test]
            public void CreateByLoyaltyPoints_10000Points_ReturnsGoldTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByLoyaltyPoints(10000);
                Assert.That(tier.TierName, Is.EqualTo("Gold"));
            }

            [Test]
            public void CreateByLoyaltyPoints_5000Points_ReturnsSilverTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByLoyaltyPoints(5000);
                Assert.That(tier.TierName, Is.EqualTo("Silver"));
            }

            [Test]
            public void CreateByLoyaltyPoints_1000Points_ReturnsBronzeTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByLoyaltyPoints(1000);
                Assert.That(tier.TierName, Is.EqualTo("Bronze"));
            }

            [Test]
            public void CreateByLoyaltyPoints_500Points_ReturnsRegularTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByLoyaltyPoints(500);
                Assert.That(tier.TierName, Is.EqualTo("Regular"));
            }

            [Test]
            public void CreateByAnnualSpending_50000_ReturnsGoldTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByAnnualSpending(50000m);
                Assert.That(tier.TierName, Is.EqualTo("Gold"));
            }

            [Test]
            public void CreateByAnnualSpending_20000_ReturnsSilverTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByAnnualSpending(20000m);
                Assert.That(tier.TierName, Is.EqualTo("Silver"));
            }

            [Test]
            public void CreateByAnnualSpending_5000_ReturnsBronzeTier()
            {
                var tier = CustomerDiscountLevelFactory.CreateByAnnualSpending(5000m);
                Assert.That(tier.TierName, Is.EqualTo("Bronze"));
            }

            [Test]
            public void GetAllTiers_Returns4Tiers()
            {
                var tiers = CustomerDiscountLevelFactory.GetAllTiers();
                Assert.That(tiers.Length, Is.EqualTo(4));
                Assert.That(tiers[0].TierName, Is.EqualTo("Regular"));
                Assert.That(tiers[1].TierName, Is.EqualTo("Bronze"));
                Assert.That(tiers[2].TierName, Is.EqualTo("Silver"));
                Assert.That(tiers[3].TierName, Is.EqualTo("Gold"));
            }

            [Test]
            public void GetNextTier_FromRegular_ReturnsBronze()
            {
                var regular = CustomerDiscountLevelFactory.CreateByTierName("regular");
                var next = CustomerDiscountLevelFactory.GetNextTier(regular);
                Assert.That(next.TierName, Is.EqualTo("Bronze"));
            }

            [Test]
            public void GetNextTier_FromBronze_ReturnsSilver()
            {
                var bronze = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                var next = CustomerDiscountLevelFactory.GetNextTier(bronze);
                Assert.That(next.TierName, Is.EqualTo("Silver"));
            }

            [Test]
            public void GetNextTier_FromGold_StaysGold()
            {
                var gold = CustomerDiscountLevelFactory.CreateByTierName("gold");
                var next = CustomerDiscountLevelFactory.GetNextTier(gold);
                Assert.That(next.TierName, Is.EqualTo("Gold"));
            }
        }

        #endregion

        #region Benefits and Promotions Tests

        [TestFixture]
        public class BenefitsTests
        {
            [Test]
            public void GetBenefitsDescription_BronzeTier_ContainsDiscount()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                var benefits = tier.GetBenefitsDescription();
                Assert.That(benefits, Does.Contain("5%"));
            }

            [Test]
            public void GetBenefitsDescription_GoldTier_ContainsFreeShipping()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("gold");
                var benefits = tier.GetBenefitsDescription();
                Assert.That(benefits, Does.Contain("FREE"));
            }

            [Test]
            public void IsEligibleForPromotion_RegularCustomer_ReturnsFalse()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("regular");
                Assert.That(tier.IsEligibleForPromotion(), Is.False);
            }

            [Test]
            public void IsEligibleForPromotion_BronzeCustomer_ReturnsTrue()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                Assert.That(tier.IsEligibleForPromotion(), Is.True);
            }
        }

        #endregion

        #region Tier Rank Tests

        [TestFixture]
        public class TierRankTests
        {
            [Test]
            public void RegularTier_Rank_IsZero()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("regular");
                Assert.That(tier.TierRank, Is.EqualTo(0));
            }

            [Test]
            public void BronzeTier_Rank_IsOne()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("bronze");
                Assert.That(tier.TierRank, Is.EqualTo(1));
            }

            [Test]
            public void SilverTier_Rank_IsTwo()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("silver");
                Assert.That(tier.TierRank, Is.EqualTo(2));
            }

            [Test]
            public void GoldTier_Rank_IsThree()
            {
                var tier = CustomerDiscountLevelFactory.CreateByTierName("gold");
                Assert.That(tier.TierRank, Is.EqualTo(3));
            }
        }

        #endregion
    }
}
