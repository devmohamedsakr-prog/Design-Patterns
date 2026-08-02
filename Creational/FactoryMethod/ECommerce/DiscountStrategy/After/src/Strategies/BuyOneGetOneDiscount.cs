using System;
using System.Threading.Tasks;
using DiscountStrategy.After.Abstracts;

namespace DiscountStrategy.After.Strategies
{
    public class BuyOneGetOneDiscount : IDiscountStrategy
    {
        public string GetStrategyName() => "BOGO";

        public async Task<DiscountResult> ApplyAsync(decimal amount, int quantity)
        {
            await Task.Delay(25);
            
            // BOGO: Buy 1 get 1 free (but need 2+ items)
            decimal discountAmount = quantity < 2 ? 0 : (amount / quantity);

            return new DiscountResult
            {
                Success = true,
                StrategyName = GetStrategyName(),
                OriginalAmount = amount,
                DiscountAmount = discountAmount,
                FinalAmount = amount - discountAmount,
                DiscountCode = "BOGO_FREE"
            };
        }
    }
}
