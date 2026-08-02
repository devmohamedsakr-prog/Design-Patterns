using System;
using OrderNotification.After.Models;
using OrderNotification.After.Subjects;
using OrderNotification.After.Observers;

namespace OrderNotification.After.Demo
{
    /// <summary>
    /// Scenario 1: Single Observer (Email Only)
    /// Subscribe to email notifications only
    /// </summary>
    class Scenario1_SingleObserver
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 1: Single Observer (Email Notifications)");
            Console.WriteLine("  Loose Coupling - Order processes independently");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var order = new Order("ORD001", "Alice Smith", "alice@example.com",
                "+1234567890", 150, "CUST001");
            var orderSubject = new OrderSubject(order);

            Console.WriteLine($"Order: {order}\n");
            Console.WriteLine("Subscribing observers...");
            orderSubject.Attach(new EmailObserver());

            Console.WriteLine("\nProcessing order...\n");
            orderSubject.ProcessOrder();

            Console.WriteLine($"\nFinal Status: {orderSubject.GetOrder().Status}");
        }
    }
}
