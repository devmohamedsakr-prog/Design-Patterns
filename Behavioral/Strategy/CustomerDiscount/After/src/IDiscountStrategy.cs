using System;

namespace CustomerDiscount.After
{
    /// <summary>
    /// Strategy interface for discount calculations.
    /// Each implementation represents a different discount strategy.
    /// SRP: Single Responsibility - Define the contract only
    /// </summary>
    public interface IDiscountStrategy
    {
        string StrategyName { get; }
        decimal CalculateDiscount(decimal subtotal, DiscountContext context);
    }

    /// <summary>
    /// Context information passed to discount strategies.
    /// Contains data needed for discount calculations.
    /// </summary>
    public class DiscountContext
    {
        public Customer Customer { get; set; }
        public int ItemCount { get; set; }
        public DateTime OrderDate { get; set; }

        public DiscountContext(Customer customer, int itemCount, DateTime orderDate)
        {
            Customer = customer;
            ItemCount = itemCount;
            OrderDate = orderDate;
        }
    }
}
