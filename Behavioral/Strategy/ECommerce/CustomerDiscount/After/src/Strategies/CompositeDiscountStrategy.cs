using System;
using System.Linq;

namespace CustomerDiscount.After.Strategies
{
    /// <summary>
    /// Composite Discount Strategy: Combines multiple strategies
    /// Single Responsibility: Orchestrate multiple discount strategies
    /// </summary>
    public class CompositeDiscountStrategy : IDiscountStrategy
    {
        private readonly IDiscountStrategy[] _strategies;

        public string StrategyName => "Composite Discount (Combined)";

        public CompositeDiscountStrategy(params IDiscountStrategy[] strategies)
        {
            _strategies = strategies;
        }

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // Combine multiple discount strategies
            decimal totalDiscount = 0;
            
            foreach (var strategy in _strategies)
            {
                if (strategy is not CompositeDiscountStrategy)
                {
                    totalDiscount += strategy.CalculateDiscount(subtotal, context);
                }
            }

            // Cap total discount at 30% of subtotal
            decimal maxDiscount = subtotal * 0.30m;
            return Math.Round(Math.Min(totalDiscount, maxDiscount), 2);
        }
    }
}
