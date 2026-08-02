namespace CustomerDiscountFactory.Tiers
{
    /// <summary>
    /// Silver tier: Regular customer discount level
    /// Single Responsibility: Manage Silver tier benefits
    /// </summary>
    public class SilverTierLevel : ICustomerDiscountLevel
    {
        public string TierName => "Silver";
        public int TierRank => 2;

        public decimal GetDiscountPercentage(decimal orderTotal)
        {
            // 10% discount for Silver members
            return orderTotal * 0.10m;
        }

        public decimal GetShippingCost(decimal orderTotal)
        {
            // Reduced $5 shipping for Silver
            return 5m;
        }

        public int GetLoyaltyPoints(decimal orderTotal)
        {
            // 1 point per $5 spent (2x Bronze)
            return (int)(orderTotal / 5m);
        }

        public bool IsEligibleForPromotion()
        {
            // Silver members eligible for standard + exclusive promotions
            return true;
        }

        public string GetBenefitsDescription()
        {
            return "Silver Tier: 10% discount, $5 shipping, 1 point per $5, exclusive promotions";
        }
    }
}
