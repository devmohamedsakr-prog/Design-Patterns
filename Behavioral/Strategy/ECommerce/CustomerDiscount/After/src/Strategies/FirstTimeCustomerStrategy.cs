using System;

namespace CustomerDiscount.After.Strategies
{
    /// <summary>
    /// First-Time Customer Strategy: 15% discount for new customers (first 30 days)
    /// Single Responsibility: Handle first-time customer discounts
    /// </summary>
    public class FirstTimeCustomerStrategy : IDiscountStrategy
    {
        public string StrategyName => "First-Time Customer (15%)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // 15% discount for first-time customers (joined within last 30 days)
            var daysSinceJoin = (DateTime.Now - context.Customer.JoinDate).TotalDays;
            if (daysSinceJoin <= 30)
            {
                return Math.Round(subtotal * 0.15m, 2);
            }
            return 0;
        }
    }
}
