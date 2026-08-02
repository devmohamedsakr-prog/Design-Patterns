using System;
using OrderValidation.After.Models;
using OrderValidation.After.Handlers;

namespace OrderValidation.After.Demo
{
    /// <summary>
    /// Scenario 1: Simple Validation Chain
    /// Demonstrates basic 2-handler validation: Inventory → Payment
    /// </summary>
    class Scenario1_SimpleValidation
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 1: Simple Validation Chain");
            Console.WriteLine("  Validation Chain: Inventory → Payment → Final");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var chain = new InventoryHandler()
                .SetNext(new PaymentHandler());

            var order = new Order("ORD001", 500);
            Console.WriteLine($"Order: {order}");
            Console.WriteLine($"Base Price: ${order.BasePrice:F2}\n");

            Console.WriteLine("Processing validation chain...\n");
            var result = chain.Handle(order);

            Console.WriteLine($"\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        }
    }
}
