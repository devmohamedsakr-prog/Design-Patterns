using System;
using System.Threading.Tasks;

namespace DiscountStrategy.After.Abstracts
{
    public abstract class DiscountCreator
    {
        protected abstract IDiscountStrategy CreateDiscountStrategy();

        public async Task<DiscountResult> ApplyDiscountAsync(decimal amount, int quantity)
        {
            try
            {
                if (amount <= 0 || quantity < 1)
                    return new DiscountResult { Success = false, Message = "Invalid amount or quantity" };

                IDiscountStrategy strategy = CreateDiscountStrategy();
                DiscountResult result = await strategy.ApplyAsync(amount, quantity);

                LogDiscount(strategy.GetStrategyName(), amount, result.Success ? "SUCCESS" : "FAILED");
                return result;
            }
            catch (Exception ex)
            {
                return new DiscountResult { Success = false, Message = ex.Message };
            }
        }

        protected virtual void LogDiscount(string strategy, decimal amount, string status)
        {
            Console.WriteLine($"[LOG] Strategy: {strategy}, Amount: ${amount:F2}, Status: {status}");
        }
    }

    public interface IDiscountStrategy
    {
        Task<DiscountResult> ApplyAsync(decimal amount, int quantity);
        string GetStrategyName();
    }

    public class DiscountResult
    {
        public bool Success { get; set; }
        public string StrategyName { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string DiscountCode { get; set; }
        public string Message { get; set; }
    }
}
