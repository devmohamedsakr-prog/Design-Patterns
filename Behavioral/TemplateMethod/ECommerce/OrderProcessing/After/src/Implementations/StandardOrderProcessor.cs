using System;
using OrderProcessing.After.Templates;

namespace OrderProcessing.After.Implementations
{
    /// <summary>
    /// Standard Order Processor: Normal discounts and costs
    /// Implements variable steps in template
    /// </summary>
    public class StandardOrderProcessor : OrderProcessingTemplate
    {
        protected override decimal CalculateDiscount(Order order)
        {
            // Standard: 5% discount
            return order.Subtotal * 0.05m;
        }

        protected override decimal CalculateTax(Order order)
        {
            // Standard: 8% tax
            return order.Subtotal * 0.08m;
        }

        protected override decimal CalculateShipping(Order order)
        {
            // Standard: $5 shipping
            return 5m;
        }

        protected override bool ProcessPayment(Order order)
        {
            Console.WriteLine($"✓ Processing standard payment for {order.CustomerId}");
            return true;
        }
    }
}
