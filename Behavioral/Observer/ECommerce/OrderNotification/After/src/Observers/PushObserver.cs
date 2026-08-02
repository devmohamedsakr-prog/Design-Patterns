using System;
using OrderNotification.After.Models;

namespace OrderNotification.After.Observers
{
    /// <summary>
    /// PushObserver: Sends push notifications on order events
    /// SRP: Only responsible for push notifications
    /// </summary>
    public class PushObserver : IOrderObserver
    {
        public void Update(OrderEvent orderEvent)
        {
            Console.WriteLine($"  🔔 [PushObserver] Sending push notification to customer {orderEvent.CustomerId}");
            Console.WriteLine($"     Title: Order {orderEvent.OrderId} {orderEvent.Status}");
            Console.WriteLine($"     Body: {orderEvent.Message}");
        }
    }
}
