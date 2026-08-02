namespace CustomerDiscount.After.Strategies
{
    /// <summary>
    /// No Discount Strategy: Returns zero discount
    /// Single Responsibility: Handle no-discount scenario
    /// </summary>
    public class NoDiscountStrategy : IDiscountStrategy
    {
        public string StrategyName => "No Discount";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            return 0;
        }
    }
}
