namespace CustomerDiscountFactory.Tiers
{
    /// <summary>
    /// Null Object: Default tier when no tier is found
    /// Single Responsibility: Provide safe default tier behavior
    /// </summary>
    public class NullTierLevel : ICustomerDiscountLevel
    {
        public string TierName => "NullTier";
        public int TierRank => -1;

        public decimal GetDiscountPercentage(decimal orderTotal) => 0m;
        public decimal GetShippingCost(decimal orderTotal) => 0m;
        public int GetLoyaltyPoints(decimal orderTotal) => 0;
        public bool IsEligibleForPromotion() => false;
        public string GetBenefitsDescription() => "No tier assigned";
    }
}
