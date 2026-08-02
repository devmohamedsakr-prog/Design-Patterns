using System;

namespace CustomerDiscount.After.Strategies
{
    /// <summary>
    /// Volume Discount Strategy: 5% per 10+ items ordered
    /// Single Responsibility: Handle volume-based discounts
    /// </summary>
    public class VolumeDiscountStrategy : IDiscountStrategy
    {
        public string StrategyName => "Volume Discount (5% per 10+ items)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // 5% discount for every 10 items, max 20%
            int itemCount = context.ItemCount;
            decimal discountRate = (itemCount / 10) * 0.05m;
            decimal maxDiscount = 0.20m;
            
            discountRate = Math.Min(discountRate, maxDiscount);
            return Math.Round(subtotal * discountRate, 2);
        }
    }
}
