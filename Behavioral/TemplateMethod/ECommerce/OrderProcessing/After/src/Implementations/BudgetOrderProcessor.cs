using System;
using OrderProcessing.After.Templates;

namespace OrderProcessing.After.Implementations
{
    /// <summary>
    /// Budget Order Processor: Minimal discounts, standard costs
    /// Implements variable steps in template
    /// </summary>
    public class BudgetOrderProcessor : OrderProcessingTemplate
    {
        protected override decimal CalculateDiscount(Order order)
        {
            // Budget: No discount
            return 0m;
        }

        protected override decimal CalculateTax(Order order)
        {
            // Budget: 8% tax
            return order.Subtotal * 0.08m;
        }

        protected override decimal CalculateShipping(Order order)
        {
            // Budget: $10 shipping
            return 10m;
        }

        protected override bool ProcessPayment(Order order)
        {
            Console.WriteLine($"✓ Processing budget payment for {order.CustomerId}");
            return true;
        }
    }
}
