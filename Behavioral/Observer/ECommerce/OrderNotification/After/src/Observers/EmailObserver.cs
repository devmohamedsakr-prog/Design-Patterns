using System;
using OrderNotification.After.Models;

namespace OrderNotification.After.Observers
{
    /// <summary>
    /// EmailObserver: Sends email notifications on order events
    /// SRP: Only responsible for email notifications
    /// </summary>
    public class EmailObserver : IOrderObserver
    {
        public void Update(OrderEvent orderEvent)
        {
            Console.WriteLine($"  📧 [EmailObserver] Sending email to {orderEvent.CustomerEmail}");
            Console.WriteLine($"     Subject: Order {orderEvent.OrderId} - {orderEvent.Status}");
            Console.WriteLine($"     Body: {orderEvent.Message}");
            Console.WriteLine($"     Amount: ${orderEvent.Amount:F2}");
        }
    }
}
