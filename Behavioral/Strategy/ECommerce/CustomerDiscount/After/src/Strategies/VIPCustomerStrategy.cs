using System;

namespace CustomerDiscount.After.Strategies
{
    /// <summary>
    /// VIP Customer Strategy: 20% discount for VIP customers
    /// Single Responsibility: Handle VIP customer discounts
    /// </summary>
    public class VIPCustomerStrategy : IDiscountStrategy
    {
        public string StrategyName => "VIP Customer (20%)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // VIP customers get 20% discount
            return Math.Round(subtotal * 0.20m, 2);
        }
    }
}
