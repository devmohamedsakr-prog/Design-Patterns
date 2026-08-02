using OrderNotification.After.Models;

namespace OrderNotification.After.Observers
{
    /// <summary>
    /// IOrderObserver: Observer interface for order events
    /// SRP: Defines contract for order observers
    /// </summary>
    public interface IOrderObserver
    {
        /// <summary>
        /// Called when order status changes
        /// </summary>
        void Update(OrderEvent orderEvent);
    }
}
