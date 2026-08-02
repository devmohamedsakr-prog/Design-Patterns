using System;

namespace CustomerDiscount.After.Strategies
{
    /// <summary>
    /// Birthday Discount Strategy: 25% discount on customer's birthday
    /// Single Responsibility: Handle birthday-based discounts
    /// </summary>
    public class BirthdayDiscountStrategy : IDiscountStrategy
    {
        public string StrategyName => "Birthday Discount (25%)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // 25% discount on customer's birthday
            if (context.Customer.IsBirthday)
            {
                return Math.Round(subtotal * 0.25m, 2);
            }
            return 0;
        }
    }
}
