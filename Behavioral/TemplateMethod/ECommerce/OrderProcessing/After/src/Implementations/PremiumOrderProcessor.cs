using System;
using OrderProcessing.After.Templates;

namespace OrderProcessing.After.Implementations
{
    /// <summary>
    /// Premium Order Processor: Maximum discounts and benefits
    /// Implements variable steps in template
    /// </summary>
    public class PremiumOrderProcessor : OrderProcessingTemplate
    {
        protected override decimal CalculateDiscount(Order order)
        {
            // Premium: 15% discount
            return order.Subtotal * 0.15m;
        }

        protected override decimal CalculateTax(Order order)
        {
            // Premium: No tax (waived)
            return 0m;
        }

        protected override decimal CalculateShipping(Order order)
        {
            // Premium: Free shipping
            return 0m;
        }

        protected override bool ProcessPayment(Order order)
        {
            Console.WriteLine($"✓ Processing premium payment for {order.CustomerId}");
            return true;
        }
    }
}
