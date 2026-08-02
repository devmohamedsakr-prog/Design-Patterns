using System;
using OrderNotification.After.Models;

namespace OrderNotification.After.Observers
{
    /// <summary>
    /// AnalyticsObserver: Tracks order metrics for analytics
    /// SRP: Only responsible for analytics tracking
    /// </summary>
    public class AnalyticsObserver : IOrderObserver
    {
        public void Update(OrderEvent orderEvent)
        {
            string eventName = orderEvent.Status switch
            {
                OrderStatus.Processing => "order_placed",
                OrderStatus.Shipped => "order_shipped",
                OrderStatus.Delivered => "order_delivered",
                OrderStatus.Cancelled => "order_cancelled",
                _ => "order_event"
            };

            Console.WriteLine($"  📊 [AnalyticsObserver] Tracking analytics event");
            Console.WriteLine($"     Event: {eventName}");
            Console.WriteLine($"     Order Value: ${orderEvent.Amount:F2}");
            Console.WriteLine($"     Customer: {orderEvent.CustomerName}");
            Console.WriteLine($"     Timestamp: {orderEvent.EventTime:yyyy-MM-dd HH:mm:ss}");
        }
    }
}
