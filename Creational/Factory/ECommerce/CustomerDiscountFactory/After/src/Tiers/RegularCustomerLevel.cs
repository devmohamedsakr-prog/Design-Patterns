namespace CustomerDiscountFactory.Tiers
{
    /// <summary>
    /// Regular customer: No special tier
    /// Single Responsibility: Manage Regular customer (no tier) behavior
    /// </summary>
    public class RegularCustomerLevel : ICustomerDiscountLevel
    {
        public string TierName => "Regular";
        public int TierRank => 0;

        public decimal GetDiscountPercentage(decimal orderTotal)
        {
            // No discount for regular customers
            return 0m;
        }

        public decimal GetShippingCost(decimal orderTotal)
        {
            // Standard $15 shipping for regular customers
            return 15m;
        }

        public int GetLoyaltyPoints(decimal orderTotal)
        {
            // No loyalty points for regular customers
            return 0;
        }

        public bool IsEligibleForPromotion()
        {
            // Regular customers not eligible for special promotions
            return false;
        }

        public string GetBenefitsDescription()
        {
            return "Regular: No discount, $15 shipping, no loyalty points";
        }
    }
}
