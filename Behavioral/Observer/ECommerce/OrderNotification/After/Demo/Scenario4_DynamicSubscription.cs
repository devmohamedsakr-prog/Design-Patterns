using System;
using OrderNotification.After.Models;
using OrderNotification.After.Subjects;
using OrderNotification.After.Observers;

namespace OrderNotification.After.Demo
{
    /// <summary>
    /// Scenario 4: Dynamic Subscription/Unsubscription
    /// Add and remove observers at runtime based on conditions
    /// </summary>
    class Scenario4_DynamicSubscription
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 4: Dynamic Subscription/Unsubscription");
            Console.WriteLine("  Add/Remove observers at runtime");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var order = new Order("ORD004", "David Brown", "david@example.com",
                "+2222222222", 450, "CUST004");
            var orderSubject = new OrderSubject(order);

            Console.WriteLine($"Order: {order}\n");

            // Add Email observer
            Console.WriteLine("Subscribe: Email Observer");
            var emailObserver = new EmailObserver();
            orderSubject.Attach(emailObserver);

            // Add SMS observer
            Console.WriteLine("Subscribe: SMS Observer");
            var smsObserver = new SMSObserver();
            orderSubject.Attach(smsObserver);

            // Add Push observer
            Console.WriteLine("Subscribe: Push Observer");
            var pushObserver = new PushObserver();
            orderSubject.Attach(pushObserver);

            // Process with all observers
            Console.WriteLine("\n--- Processing with all observers ---\n");
            orderSubject.ProcessOrder();

            // Remove SMS observer
            Console.WriteLine("\n\nUnsubscribe: SMS Observer");
            orderSubject.Detach(smsObserver);

            // Ship with only Email and Push
            Console.WriteLine("\n--- Shipping without SMS Observer ---\n");
            orderSubject.ShipOrder();

            Console.WriteLine($"\nFinal Status: {orderSubject.GetOrder().Status}");
        }
    }
}
