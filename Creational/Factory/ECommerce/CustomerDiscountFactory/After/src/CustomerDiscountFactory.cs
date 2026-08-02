using System;

namespace CustomerDiscountFactory
{
    /// <summary>
    /// Factory for creating customer discount level objects
    /// Centralizes all discount tier creation logic
    /// </summary>
    public static class CustomerDiscountLevelFactory
    {
        /// <summary>
        /// Create a discount level by tier name
        /// ✅ Centralized creation logic
        /// ✅ Clients don't know concrete types
        /// </summary>
        public static ICustomerDiscountLevel CreateByTierName(string tierName)
        {
            if (string.IsNullOrEmpty(tierName))
                throw new ArgumentNullException(nameof(tierName));

            return tierName.ToLower() switch
            {
                "bronze" => new BronzeTierLevel(),
                "silver" => new SilverTierLevel(),
                "gold" => new GoldTierLevel(),
                "regular" => new RegularCustomerLevel(),
                _ => throw new ArgumentException($"Unknown tier: {tierName}")
            };
        }

        /// <summary>
        /// Create a discount level by enum
        /// Type-safe alternative to string-based creation
        /// </summary>
        public static ICustomerDiscountLevel CreateByTierType(CustomerTierType tierType)
        {
            return tierType switch
            {
                CustomerTierType.Bronze => new BronzeTierLevel(),
                CustomerTierType.Silver => new SilverTierLevel(),
                CustomerTierType.Gold => new GoldTierLevel(),
                CustomerTierType.Regular => new RegularCustomerLevel(),
                _ => throw new ArgumentException($"Unknown tier type: {tierType}")
            };
        }

        /// <summary>
        /// Create discount level based on customer's accumulated points
        /// Automatically upgrade tier based on loyalty
        /// </summary>
        public static ICustomerDiscountLevel CreateByLoyaltyPoints(int totalPoints)
        {
            return totalPoints switch
            {
                >= 10000 => new GoldTierLevel(),      // 10k+ points = Gold
                >= 5000 => new SilverTierLevel(),     // 5k+ points = Silver
                >= 1000 => new BronzeTierLevel(),     // 1k+ points = Bronze
                _ => new RegularCustomerLevel()       // < 1k = Regular
            };
        }

        /// <summary>
        /// Create discount level based on customer's annual spending
        /// </summary>
        public static ICustomerDiscountLevel CreateByAnnualSpending(decimal annualSpending)
        {
            return annualSpending switch
            {
                >= 50000m => new GoldTierLevel(),     // $50k+ = Gold
                >= 20000m => new SilverTierLevel(),   // $20k+ = Silver
                >= 5000m => new BronzeTierLevel(),    // $5k+ = Bronze
                _ => new RegularCustomerLevel()       // < $5k = Regular
            };
        }

        /// <summary>
        /// Get all available tiers for selection
        /// Useful for UI dropdown, tier comparison
        /// </summary>
        public static ICustomerDiscountLevel[] GetAllTiers()
        {
            return new ICustomerDiscountLevel[]
            {
                new RegularCustomerLevel(),
                new BronzeTierLevel(),
                new SilverTierLevel(),
                new GoldTierLevel()
            };
        }

        /// <summary>
        /// Get the next tier above current tier
        /// For displaying upgrade opportunity
        /// </summary>
        public static ICustomerDiscountLevel GetNextTier(ICustomerDiscountLevel currentTier)
        {
            if (currentTier == null)
                throw new ArgumentNullException(nameof(currentTier));

            return currentTier.TierRank switch
            {
                0 => new BronzeTierLevel(),    // Regular -> Bronze
                1 => new SilverTierLevel(),    // Bronze -> Silver
                2 => new GoldTierLevel(),      // Silver -> Gold
                3 => new GoldTierLevel(),      // Gold (no higher)
                _ => throw new InvalidOperationException("Unknown tier rank")
            };
        }
    }

    /// <summary>
    /// Enum for type-safe tier selection
    /// </summary>
    public enum CustomerTierType
    {
        Regular = 0,
        Bronze = 1,
        Silver = 2,
        Gold = 3
    }
}
