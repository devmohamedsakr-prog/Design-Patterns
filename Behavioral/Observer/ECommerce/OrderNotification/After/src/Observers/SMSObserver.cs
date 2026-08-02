using System;
using OrderNotification.After.Models;

namespace OrderNotification.After.Observers
{
    /// <summary>
    /// SMSObserver: Sends SMS notifications on order events
    /// SRP: Only responsible for SMS notifications
    /// </summary>
    public class SMSObserver : IOrderObserver
    {
        public void Update(OrderEvent orderEvent)
        {
            Console.WriteLine($"  📱 [SMSObserver] Sending SMS to {orderEvent.CustomerPhone}");
            Console.WriteLine($"     Message: Order {orderEvent.OrderId} status: {orderEvent.Status}");
            Console.WriteLine($"     {orderEvent.Message}");
        }
    }
}
