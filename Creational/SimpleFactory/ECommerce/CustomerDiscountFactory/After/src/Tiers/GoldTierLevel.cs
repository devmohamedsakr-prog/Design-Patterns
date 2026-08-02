namespace CustomerDiscountFactory.Tiers
{
    /// <summary>
    /// Gold tier: Premium customer discount level
    /// Single Responsibility: Manage Gold tier benefits
    /// </summary>
    public class GoldTierLevel : ICustomerDiscountLevel
    {
        public string TierName => "Gold";
        public int TierRank => 3;

        public decimal GetDiscountPercentage(decimal orderTotal)
        {
            // 15% discount for Gold members
            return orderTotal * 0.15m;
        }

        public decimal GetShippingCost(decimal orderTotal)
        {
            // Free shipping for Gold members
            return 0m;
        }

        public int GetLoyaltyPoints(decimal orderTotal)
        {
            // 1 point per $2 spent (5x Bronze, 2.5x Silver)
            return (int)(orderTotal / 2m);
        }

        public bool IsEligibleForPromotion()
        {
            // Gold members eligible for all promotions including VIP events
            return true;
        }

        public string GetBenefitsDescription()
        {
            return "Gold Tier: 15% discount, FREE shipping, 1 point per $2, VIP promotions, priority support";
        }
    }
}
