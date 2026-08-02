namespace CustomerDiscountFactory.Tiers
{
    /// <summary>
    /// Bronze tier: Basic customer discount level
    /// Single Responsibility: Manage Bronze tier benefits
    /// </summary>
    public class BronzeTierLevel : ICustomerDiscountLevel
    {
        public string TierName => "Bronze";
        public int TierRank => 1;

        public decimal GetDiscountPercentage(decimal orderTotal)
        {
            // 5% discount for Bronze members
            return orderTotal * 0.05m;
        }

        public decimal GetShippingCost(decimal orderTotal)
        {
            // Fixed $10 shipping for Bronze
            return 10m;
        }

        public int GetLoyaltyPoints(decimal orderTotal)
        {
            // 1 point per $10 spent
            return (int)(orderTotal / 10m);
        }

        public bool IsEligibleForPromotion()
        {
            // Bronze members eligible for basic promotions
            return true;
        }

        public string GetBenefitsDescription()
        {
            return "Bronze Tier: 5% discount, standard shipping, 1 point per $10";
        }
    }
}
