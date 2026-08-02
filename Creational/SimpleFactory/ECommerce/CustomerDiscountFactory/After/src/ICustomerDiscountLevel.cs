namespace CustomerDiscountFactory
{
    /// <summary>
    /// Represents a customer discount level with tier-specific benefits
    /// </summary>
    public interface ICustomerDiscountLevel
    {
        string TierName { get; }
        int TierRank { get; }
        
        /// <summary>Calculate discount percentage for order</summary>
        decimal GetDiscountPercentage(decimal orderTotal);
        
        /// <summary>Calculate shipping cost</summary>
        decimal GetShippingCost(decimal orderTotal);
        
        /// <summary>Calculate loyalty points earned</summary>
        int GetLoyaltyPoints(decimal orderTotal);
        
        /// <summary>Check if customer qualifies for special promotions</summary>
        bool IsEligibleForPromotion();
        
        /// <summary>Get tier benefits description</summary>
        string GetBenefitsDescription();
    }
}
