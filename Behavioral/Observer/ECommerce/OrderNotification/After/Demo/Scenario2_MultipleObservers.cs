using System;
using OrderNotification.After.Models;
using OrderNotification.After.Subjects;
using OrderNotification.After.Observers;

namespace OrderNotification.After.Demo
{
    /// <summary>
    /// Scenario 2: Multiple Observers (All Notifications)
    /// Subscribe to Email, SMS, Push, Inventory, Analytics
    /// </summary>
    class Scenario2_MultipleObservers
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 2: Multiple Observers (All Channels)");
            Console.WriteLine("  Email + SMS + Push + Inventory + Analytics");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var order = new Order("ORD002", "Bob Johnson", "bob@example.com",
                "+0987654321", 250, "CUST002");
            var orderSubject = new OrderSubject(order);

            Console.WriteLine($"Order: {order}\n");
            Console.WriteLine("Subscribing multiple observers...");
            orderSubject.Attach(new EmailObserver());
            orderSubject.Attach(new SMSObserver());
            orderSubject.Attach(new PushObserver());
            orderSubject.Attach(new InventoryObserver());
            orderSubject.Attach(new AnalyticsObserver());

            Console.WriteLine("\nProcessing order (all observers notified)...\n");
            orderSubject.ProcessOrder();

            Console.WriteLine($"\nFinal Status: {orderSubject.GetOrder().Status}");
        }
    }
}
