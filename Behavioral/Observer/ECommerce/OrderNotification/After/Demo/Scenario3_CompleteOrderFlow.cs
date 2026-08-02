using System;
using OrderNotification.After.Models;
using OrderNotification.After.Subjects;
using OrderNotification.After.Observers;

namespace OrderNotification.After.Demo
{
    /// <summary>
    /// Scenario 3: Complete Order Flow
    /// Process → Ship → Deliver with multiple observers
    /// </summary>
    class Scenario3_CompleteOrderFlow
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 3: Complete Order Flow");
            Console.WriteLine("  Process → Ship → Deliver");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var order = new Order("ORD003", "Carol Williams", "carol@example.com",
                "+1111111111", 350, "CUST003");
            var orderSubject = new OrderSubject(order);

            Console.WriteLine($"Order: {order}\n");
            Console.WriteLine("Subscribing observers...");
            orderSubject.Attach(new EmailObserver());
            orderSubject.Attach(new SMSObserver());
            orderSubject.Attach(new InventoryObserver());

            // Step 1: Process
            Console.WriteLine("\n--- Step 1: Process Order ---\n");
            orderSubject.ProcessOrder();

            // Step 2: Ship
            Console.WriteLine("\n--- Step 2: Ship Order ---\n");
            orderSubject.ShipOrder();

            // Step 3: Deliver
            Console.WriteLine("\n--- Step 3: Deliver Order ---\n");
            orderSubject.DeliverOrder();

            Console.WriteLine($"\nFinal Status: {orderSubject.GetOrder().Status}");
        }
    }
}
