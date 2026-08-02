using System;

namespace CustomerDiscount.After.Strategies
{
    /// <summary>
    /// Loyal Customer Strategy: 5% base + 1% per year of loyalty
    /// Single Responsibility: Handle loyal customer discounts based on tenure
    /// </summary>
    public class LoyalCustomerStrategy : IDiscountStrategy
    {
        public string StrategyName => "Loyal Customer (5% + 1% per year)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // 5% base + 1% per year, capped at 25%
            decimal discountRate = 0.05m + (context.Customer.YearsAsCustomer * 0.01m);
            decimal maxDiscount = subtotal * 0.25m;
            decimal discount = subtotal * discountRate;
            
            return Math.Round(Math.Min(discount, maxDiscount), 2);
        }
    }
}
