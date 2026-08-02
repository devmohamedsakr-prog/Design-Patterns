using System;
using OrderNotification.After.Models;

namespace OrderNotification.After.Observers
{
    /// <summary>
    /// InventoryObserver: Updates inventory system on order events
    /// SRP: Only responsible for inventory updates
    /// </summary>
    public class InventoryObserver : IOrderObserver
    {
        public void Update(OrderEvent orderEvent)
        {
            string action = orderEvent.Status switch
            {
                OrderStatus.Processing => "Reserve items",
                OrderStatus.Shipped => "Mark items as shipped",
                OrderStatus.Delivered => "Mark items as delivered",
                OrderStatus.Cancelled => "Release reserved items",
                _ => "Update inventory"
            };

            Console.WriteLine($"  📦 [InventoryObserver] Updating inventory system");
            Console.WriteLine($"     Action: {action} for order {orderEvent.OrderId}");
            Console.WriteLine($"     Status: {orderEvent.Status}");
        }
    }
}
