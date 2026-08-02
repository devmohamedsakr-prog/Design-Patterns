using System;

namespace CustomerDiscount.After
{
    /// <summary>
    /// Collection of concrete discount strategy implementations.
    /// Each strategy encapsulates a specific discount algorithm.
    /// SRP: Single Responsibility - Each class implements one discount type
    /// </summary>

    // ========================================================================
    // STRATEGY 1: No Discount
    // ========================================================================
    public class NoDiscountStrategy : IDiscountStrategy
    {
        public string StrategyName => "No Discount";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            return 0;
        }
    }

    // ========================================================================
    // STRATEGY 2: Regular Customer Strategy
    // ========================================================================
    public class RegularCustomerStrategy : IDiscountStrategy
    {
        public string StrategyName => "Regular Customer (0%)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // Regular customers get no discount
            return 0;
        }
    }

    // ========================================================================
    // STRATEGY 3: Premium Customer Strategy
    // ========================================================================
    public class PremiumCustomerStrategy : IDiscountStrategy
    {
        public string StrategyName => "Premium Customer (10%)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // Premium customers get 10% discount
            return Math.Round(subtotal * 0.10m, 2);
        }
    }

    // ========================================================================
    // STRATEGY 4: VIP Customer Strategy
    // ========================================================================
    public class VIPCustomerStrategy : IDiscountStrategy
    {
        public string StrategyName => "VIP Customer (20%)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // VIP customers get 20% discount
            return Math.Round(subtotal * 0.20m, 2);
        }
    }

    // ========================================================================
    // STRATEGY 5: Loyal Customer Strategy
    // ========================================================================
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

    // ========================================================================
    // STRATEGY 6: Volume Discount Strategy
    // ========================================================================
    public class VolumeDiscountStrategy : IDiscountStrategy
    {
        public string StrategyName => "Volume Discount (5% per 10+ items)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // 5% discount for every 10 items, max 20%
            int itemCount = context.ItemCount;
            decimal discountRate = (itemCount / 10) * 0.05m;
            decimal maxDiscount = 0.20m;
            
            discountRate = Math.Min(discountRate, maxDiscount);
            return Math.Round(subtotal * discountRate, 2);
        }
    }

    // ========================================================================
    // STRATEGY 7: Seasonal Discount Strategy
    // ========================================================================
    public class SeasonalDiscountStrategy : IDiscountStrategy
    {
        public string StrategyName => "Seasonal Discount";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // Apply discounts based on season
            // Summer (June-August): 20% discount
            // Winter (December-February): 15% discount
            // Other: 5% discount
            
            int month = context.OrderDate.Month;
            decimal discountRate = 0.05m; // Default

            if (month >= 6 && month <= 8)
            {
                discountRate = 0.20m; // Summer
            }
            else if (month == 12 || month <= 2)
            {
                discountRate = 0.15m; // Winter
            }

            return Math.Round(subtotal * discountRate, 2);
        }
    }

    // ========================================================================
    // STRATEGY 8: Birthday Discount Strategy
    // ========================================================================
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

    // ========================================================================
    // STRATEGY 9: First-Time Customer Strategy
    // ========================================================================
    public class FirstTimeCustomerStrategy : IDiscountStrategy
    {
        public string StrategyName => "First-Time Customer (15%)";

        public decimal CalculateDiscount(decimal subtotal, DiscountContext context)
        {
            // 15% discount for first-time customers (joined within last 30 days)
            var daysSinceJoin = (DateTime.Now - context.Customer.JoinDate).TotalDays;
            if (daysSinceJoin <= 30)
            {
                return Math.Round(subtotal * 0.15m, 2);
            }
            return 0;
        }
    }

    // ========================================================================
    // STRATEGY 10: Composite/Combined Strategy
    // ========================================================================
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
