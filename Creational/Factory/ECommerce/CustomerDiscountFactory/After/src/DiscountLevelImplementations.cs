namespace CustomerDiscountFactory
{
    /// <summary>
    /// Null Object: Default tier when no tier is found
    /// ✅ Prevents null reference exceptions
    /// ✅ Provides safe default behavior
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

    /// <summary>Bronze tier: Basic customer discount level</summary>
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

    /// <summary>Silver tier: Regular customer discount level</summary>
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

    /// <summary>Gold tier: Premium customer discount level</summary>
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

    /// <summary>Regular customer: No special tier</summary>
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
