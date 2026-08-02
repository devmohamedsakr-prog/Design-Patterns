using System;
using System.Threading.Tasks;
using DiscountStrategy.After.Abstracts;

namespace DiscountStrategy.After.Strategies
{
    public class FixedDiscount : IDiscountStrategy
    {
        private readonly decimal _fixedAmount;

        public FixedDiscount(decimal fixedAmount = 10) => _fixedAmount = fixedAmount;

        public string GetStrategyName() => "Fixed";

        public async Task<DiscountResult> ApplyAsync(decimal amount, int quantity)
        {
            await Task.Delay(25);
            decimal discountAmount = _fixedAmount;

            return new DiscountResult
            {
                Success = true,
                StrategyName = GetStrategyName(),
                OriginalAmount = amount,
                DiscountAmount = discountAmount,
                FinalAmount = Math.Max(0, amount - discountAmount),
                DiscountCode = $"fix_{_fixedAmount:00}"
            };
        }
    }
}
