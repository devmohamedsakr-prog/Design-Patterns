using System;
using OrderValidation.After.Models;
using OrderValidation.After.Handlers;

namespace OrderValidation.After.Demo
{
    /// <summary>
    /// Scenario 3: Validation Fails at Fraud Detection
    /// Demonstrates chain termination when fraud is detected
    /// </summary>
    class Scenario3_FailsAtFraud
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 3: Validation Fails at Fraud Detection");
            Console.WriteLine("  Reason: Order amount exceeds fraud threshold");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var chain = new InventoryHandler()
                .SetNext(new PaymentHandler())
                .SetNext(new FraudHandler(highAmountThreshold: 1000))
                .SetNext(new ShippingHandler());

            var order = new Order("ORD003", 50000);
            Console.WriteLine($"Order: {order}");
            Console.WriteLine($"Base Price: ${order.BasePrice:F2}");
            Console.WriteLine($"⚠️  Alert: Amount exceeds fraud threshold ($1000)\n");

            Console.WriteLine("Processing validation chain...");
            Console.WriteLine("  Inventory ✓ → Payment ✓ → Fraud Detection ✗ (STOP)\n");

            var result = chain.Handle(order);

            Console.WriteLine($"\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        }
    }
}
