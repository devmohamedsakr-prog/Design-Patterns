using System;

namespace CustomerDiscount.After.Strategies
{
    /// <summary>
    /// Premium Customer Strategy: 10% discount for premium customers
    /// Single Responsibility: Handle premium customer discounts
    /// </summary>
    public class PremiumCustomerStrategy : IDiscountStrategy
    {
        public string StrategyName => "Premium Customer (10%)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // Premium customers get 10% discount
            return Math.Round(subtotal * 0.10m, 2);
        }
    }
}
