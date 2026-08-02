using System;

namespace CustomerDiscount.After.Strategies
{
    /// <summary>
    /// Seasonal Discount Strategy: Discount based on time of year
    /// Single Responsibility: Handle seasonal discount calculations
    /// </summary>
    public class SeasonalDiscountStrategy : IDiscountStrategy
    {
        public string StrategyName => "Seasonal Discount";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // Apply discounts based on season
            // Summer (June-August): 20% discount
            // Winter (December-February): 15% discount
            // Other: 5% discount
            
            int month = context.OrderDate.Month;
            decimal discountRate = 0.05m; // Default

            if (month >= 6 && month <= 8)
            {
                discountRate = 0.20m; // Summer
            }
            else if (month == 12 || month <= 2)
            {
                discountRate = 0.15m; // Winter
            }

            return Math.Round(subtotal * discountRate, 2);
        }
    }
}
