using System;
using OrderValidation.After.Models;
using OrderValidation.After.Handlers;

namespace OrderValidation.After.Demo
{
    /// <summary>
    /// Scenario 4: Different Validator Order
    /// Demonstrates chain flexibility: Fraud → Payment → Inventory → Shipping
    /// </summary>
    class Scenario4_DifferentOrder
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 4: Different Validator Order");
            Console.WriteLine("  Chain: Fraud → Payment → Inventory → Shipping");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var chain = new FraudHandler()
                .SetNext(new PaymentHandler())
                .SetNext(new InventoryHandler())
                .SetNext(new ShippingHandler());

            var order = new Order("ORD004", 500);
            Console.WriteLine($"Order: {order}");
            Console.WriteLine($"Base Price: ${order.BasePrice:F2}\n");

            Console.WriteLine("Processing validation chain with different order...");
            Console.WriteLine("  Fraud Detection First (High-priority for security)");
            Console.WriteLine("  Then: Payment → Inventory → Shipping\n");

            var result = chain.Handle(order);

            Console.WriteLine($"\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Flexibility: Chain order can change without code modification");
            Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        }
    }
}
