using System;
using System.Threading.Tasks;
using DiscountStrategy.After.Abstracts;

namespace DiscountStrategy.After.Strategies
{
    public class PercentageDiscount : IDiscountStrategy
    {
        private readonly decimal _percentage;

        public PercentageDiscount(decimal percentage = 10) => _percentage = percentage;

        public string GetStrategyName() => "Percentage";

        public async Task<DiscountResult> ApplyAsync(decimal amount, int quantity)
        {
            await Task.Delay(25);
            decimal discountAmount = amount * (_percentage / 100);

            return new DiscountResult
            {
                Success = true,
                StrategyName = GetStrategyName(),
                OriginalAmount = amount,
                DiscountAmount = discountAmount,
                FinalAmount = amount - discountAmount,
                DiscountCode = $"pct_{_percentage:00}"
            };
        }
    }
}
