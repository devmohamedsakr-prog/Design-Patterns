using System;
using OrderValidation.After.Models;
using OrderValidation.After.Handlers;

namespace OrderValidation.After.Demo
{
    /// <summary>
    /// Scenario 2: Complete Validation Chain
    /// Demonstrates all 4 validators: Inventory → Payment → Fraud → Shipping
    /// </summary>
    class Scenario2_CompleteChain
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 2: Complete Validation Chain");
            Console.WriteLine("  Chain: Inventory → Payment → Fraud → Shipping");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var chain = new InventoryHandler()
                .SetNext(new PaymentHandler())
                .SetNext(new FraudHandler())
                .SetNext(new ShippingHandler());

            var order = new Order("ORD002", 500);
            Console.WriteLine($"Order: {order}");
            Console.WriteLine($"Base Price: ${order.BasePrice:F2}\n");

            Console.WriteLine("Processing complete validation chain...");
            Console.WriteLine("  Inventory → Payment → Fraud Detection → Shipping\n");

            var result = chain.Handle(order);

            Console.WriteLine($"\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        }
    }
}
