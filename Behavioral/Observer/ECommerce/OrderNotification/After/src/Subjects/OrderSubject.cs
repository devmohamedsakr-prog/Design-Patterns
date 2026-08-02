using System;
using System.Collections.Generic;
using OrderNotification.After.Models;
using OrderNotification.After.Observers;

namespace OrderNotification.After.Subjects
{
    /// <summary>
    /// OrderSubject: Subject that notifies observers about order changes
    /// SRP: Only manages order and notifies observers
    /// </summary>
    public class OrderSubject
    {
        private Order _order;
        private List<IOrderObserver> _observers;

        public OrderSubject(Order order)
        {
            _order = order;
            _observers = new List<IOrderObserver>();
        }

        /// <summary>
        /// Subscribe an observer to order events
        /// </summary>
        public void Attach(IOrderObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                Console.WriteLine($"  ✓ Observer attached: {observer.GetType().Name}");
            }
        }

        /// <summary>
        /// Unsubscribe an observer
        /// </summary>
        public void Detach(IOrderObserver observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
                Console.WriteLine($"  ✓ Observer detached: {observer.GetType().Name}");
            }
        }

        /// <summary>
        /// Notify all observers about status change
        /// </summary>
        private void NotifyObservers(string message = "")
        {
            var orderEvent = new OrderEvent(_order, message);
            
            foreach (var observer in _observers)
            {
                observer.Update(orderEvent);
            }
        }

        /// <summary>
        /// Process order and notify observers
        /// </summary>
        public void ProcessOrder()
        {
            Console.WriteLine($"\nProcessing order {_order.OrderId}...");
            _order.UpdateStatus(OrderStatus.Processing);
            NotifyObservers("Order has been placed and is being processed");
        }

        /// <summary>
        /// Ship order and notify observers
        /// </summary>
        public void ShipOrder()
        {
            Console.WriteLine($"\nShipping order {_order.OrderId}...");
            _order.UpdateStatus(OrderStatus.Shipped);
            NotifyObservers("Order has been shipped");
        }

        /// <summary>
        /// Deliver order and notify observers
        /// </summary>
        public void DeliverOrder()
        {
            Console.WriteLine($"\nDelivering order {_order.OrderId}...");
            _order.UpdateStatus(OrderStatus.Delivered);
            NotifyObservers("Order has been delivered");
        }

        /// <summary>
        /// Cancel order and notify observers
        /// </summary>
        public void CancelOrder()
        {
            Console.WriteLine($"\nCancelling order {_order.OrderId}...");
            _order.UpdateStatus(OrderStatus.Cancelled);
            NotifyObservers("Order has been cancelled");
        }

        public Order GetOrder() => _order;
    }
}
