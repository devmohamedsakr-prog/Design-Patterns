namespace CustomerDiscount.After.Strategies
{
    /// <summary>
    /// Regular Customer Strategy: No discount for regular customers
    /// Single Responsibility: Handle regular customer discounts
    /// </summary>
    public class RegularCustomerStrategy : IDiscountStrategy
    {
        public string StrategyName => "Regular Customer (0%)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // Regular customers get no discount
            return 0;
        }
    }
}
