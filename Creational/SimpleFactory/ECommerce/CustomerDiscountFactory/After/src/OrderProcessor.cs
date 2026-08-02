using System;

namespace CustomerDiscountFactory
{
    /// <summary>
    /// Processes orders using customer discount levels
    /// ✅ Uses Factory to get correct discount tier
    /// ✅ No hard-coded tier logic
    /// </summary>
    public class OrderProcessor
    {
        /// <summary>
        /// Calculate order total with discount tier benefits
        /// </summary>
        public OrderSummary ProcessOrder(Customer customer, decimal orderSubtotal)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (orderSubtotal < 0)
                throw new ArgumentException("Order total cannot be negative", nameof(orderSubtotal));

            // ✅ Factory creates appropriate discount tier
            var discountTier = CustomerDiscountLevelFactory.CreateByTierName(customer.TierLevel);

            // Calculate benefits
            var discountAmount = discountTier.GetDiscountPercentage(orderSubtotal);
            var shippingCost = discountTier.GetShippingCost(orderSubtotal);
            var loyaltyPoints = discountTier.GetLoyaltyPoints(orderSubtotal);

            var subtotalAfterDiscount = orderSubtotal - discountAmount;
            var total = subtotalAfterDiscount + shippingCost;

            return new OrderSummary
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.Name,
                Subtotal = orderSubtotal,
                DiscountTier = discountTier.TierName,
                DiscountAmount = discountAmount,
                ShippingCost = shippingCost,
                LoyaltyPointsEarned = loyaltyPoints,
                Total = total
            };
        }

        /// <summary>
        /// Check if customer qualifies for a promotion
        /// </summary>
        public bool IsEligibleForPromotion(Customer customer)
        {
            var discountTier = CustomerDiscountLevelFactory.CreateByTierName(customer.TierLevel);
            return discountTier.IsEligibleForPromotion();
        }

        /// <summary>
        /// Get tier benefits for customer
        /// </summary>
        public string GetTierBenefits(Customer customer)
        {
            var discountTier = CustomerDiscountLevelFactory.CreateByTierName(customer.TierLevel);
            return discountTier.GetBenefitsDescription();
        }

        /// <summary>
        /// Get next tier information for upgrade display
        /// </summary>
        public string GetUpgradeInfo(Customer customer)
        {
            var currentTier = CustomerDiscountLevelFactory.CreateByTierName(customer.TierLevel);
            
            if (currentTier.TierRank >= 3)
                return "You are already at our highest tier (Gold)";

            var nextTier = CustomerDiscountLevelFactory.GetNextTier(currentTier);
            return $"Upgrade to {nextTier.TierName}: {nextTier.GetBenefitsDescription()}";
        }
    }

    /// <summary>
    /// Summary of processed order with discount tier benefits
    /// </summary>
    public class OrderSummary
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal Subtotal { get; set; }
        public string DiscountTier { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public int LoyaltyPointsEarned { get; set; }
        public decimal Total { get; set; }

        public override string ToString()
        {
            return $"Order ({CustomerName} - {DiscountTier}): " +
                   $"Subtotal: ${Subtotal:F2}, " +
                   $"Discount: -${DiscountAmount:F2}, " +
                   $"Shipping: ${ShippingCost:F2}, " +
                   $"Total: ${Total:F2}, " +
                   $"Points: {LoyaltyPointsEarned}";
        }
    }

    /// <summary>
    /// Represents a customer with tier level
    /// </summary>
    public class Customer
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string TierLevel { get; set; } // "Bronze", "Silver", "Gold", "Regular"
        public decimal TotalSpent { get; set; }
        public int TotalLoyaltyPoints { get; set; }

        public override string ToString()
        {
            return $"{Name} ({TierLevel})";
        }
    }
}
