using System;
using System.Collections.Generic;

namespace OrderLifecycle.After.Context
{
    /// <summary>
    /// Order Context: Manages order state and delegates behavior to current state
    /// States: Pending → Processing → Shipped → Delivered → Completed/Cancelled
    /// </summary>
    public class Order
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public IOrderState CurrentState { get; private set; }
        private List<string> StateHistory { get; set; } = new();

        public Order(string orderId, decimal amount)
        {
            OrderId = orderId;
            Amount = amount;
            CreatedAt = DateTime.UtcNow;
            CurrentState = new PendingState(); // Initial state
            StateHistory.Add($"[{CreatedAt:yyyy-MM-dd HH:mm:ss}] Order Created - State: Pending");
        }

        /// <summary>
        /// Transition to new state
        /// </summary>
        public void TransitionTo(IOrderState newState)
        {
            string oldState = CurrentState.GetStateName();
            CurrentState = newState;
            StateHistory.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Transitioned: {oldState} → {newState.GetStateName()}");
        }

        // Delegate operations to current state
        public bool CanPayment() => CurrentState.CanPayment(this);
        public bool CanShip() => CurrentState.CanShip(this);
        public bool CanDeliver() => CurrentState.CanDeliver(this);
        public bool CanCancel() => CurrentState.CanCancel(this);

        public bool ProcessPayment() => CurrentState.ProcessPayment(this);
        public bool ShipOrder() => CurrentState.ShipOrder(this);
        public bool DeliverOrder() => CurrentState.DeliverOrder(this);
        public bool CancelOrder() => CurrentState.CancelOrder(this);

        public string GetCurrentStateName() => CurrentState.GetStateName();
        public List<string> GetStateHistory() => new(StateHistory);
    }

    /// <summary>
    /// Order State Interface: Defines possible operations per state
    /// </summary>
    public interface IOrderState
    {
        string GetStateName();
        bool CanPayment(Order order);
        bool CanShip(Order order);
        bool CanDeliver(Order order);
        bool CanCancel(Order order);

        bool ProcessPayment(Order order);
        bool ShipOrder(Order order);
        bool DeliverOrder(Order order);
        bool CancelOrder(Order order);
    }

    // ============================================================
    // CONCRETE STATES
    // ============================================================

    /// <summary>State 1: Pending - Order created, awaiting payment</summary>
    public class PendingState : IOrderState
    {
        public string GetStateName() => "Pending";
        public bool CanPayment(Order order) => true;
        public bool CanShip(Order order) => false;
        public bool CanDeliver(Order order) => false;
        public bool CanCancel(Order order) => true;

        public bool ProcessPayment(Order order)
        {
            if (!CanPayment(order)) return false;
            order.TransitionTo(new ProcessingState());
            Console.WriteLine($"✓ Order {order.OrderId}: Payment processed. Transitioned to Processing.");
            return true;
        }

        public bool ShipOrder(Order order) => false;
        public bool DeliverOrder(Order order) => false;

        public bool CancelOrder(Order order)
        {
            if (!CanCancel(order)) return false;
            order.TransitionTo(new CancelledState());
            Console.WriteLine($"✓ Order {order.OrderId}: Cancelled from Pending state.");
            return true;
        }
    }

    /// <summary>State 2: Processing - Payment confirmed, preparing shipment</summary>
    public class ProcessingState : IOrderState
    {
        public string GetStateName() => "Processing";
        public bool CanPayment(Order order) => false;
        public bool CanShip(Order order) => true;
        public bool CanDeliver(Order order) => false;
        public bool CanCancel(Order order) => true;

        public bool ProcessPayment(Order order) => false;

        public bool ShipOrder(Order order)
        {
            if (!CanShip(order)) return false;
            order.TransitionTo(new ShippedState());
            Console.WriteLine($"✓ Order {order.OrderId}: Shipped. Transitioned to Shipped.");
            return true;
        }

        public bool DeliverOrder(Order order) => false;

        public bool CancelOrder(Order order)
        {
            if (!CanCancel(order)) return false;
            order.TransitionTo(new CancelledState());
            Console.WriteLine($"✓ Order {order.OrderId}: Cancelled from Processing state.");
            return true;
        }
    }

    /// <summary>State 3: Shipped - In transit to customer</summary>
    public class ShippedState : IOrderState
    {
        public string GetStateName() => "Shipped";
        public bool CanPayment(Order order) => false;
        public bool CanShip(Order order) => false;
        public bool CanDeliver(Order order) => true;
        public bool CanCancel(Order order) => false; // Cannot cancel after shipped

        public bool ProcessPayment(Order order) => false;
        public bool ShipOrder(Order order) => false;

        public bool DeliverOrder(Order order)
        {
            if (!CanDeliver(order)) return false;
            order.TransitionTo(new DeliveredState());
            Console.WriteLine($"✓ Order {order.OrderId}: Delivered. Transitioned to Delivered.");
            return true;
        }

        public bool CancelOrder(Order order) => false;
    }

    /// <summary>State 4: Delivered - Arrived at customer location</summary>
    public class DeliveredState : IOrderState
    {
        public string GetStateName() => "Delivered";
        public bool CanPayment(Order order) => false;
        public bool CanShip(Order order) => false;
        public bool CanDeliver(Order order) => false;
        public bool CanCancel(Order order) => false;

        public bool ProcessPayment(Order order) => false;
        public bool ShipOrder(Order order) => false;
        public bool DeliverOrder(Order order) => false;

        public bool CancelOrder(Order order) => false;

        // Automatic transition after delivery
        public void CompleteOrder(Order order)
        {
            order.TransitionTo(new CompletedState());
            Console.WriteLine($"✓ Order {order.OrderId}: Marked as Completed.");
        }
    }

    /// <summary>State 5: Completed - Order successfully fulfilled</summary>
    public class CompletedState : IOrderState
    {
        public string GetStateName() => "Completed";
        public bool CanPayment(Order order) => false;
        public bool CanShip(Order order) => false;
        public bool CanDeliver(Order order) => false;
        public bool CanCancel(Order order) => false;

        public bool ProcessPayment(Order order) => false;
        public bool ShipOrder(Order order) => false;
        public bool DeliverOrder(Order order) => false;
        public bool CancelOrder(Order order) => false;
    }

    /// <summary>State 6: Cancelled - Order was cancelled</summary>
    public class CancelledState : IOrderState
    {
        public string GetStateName() => "Cancelled";
        public bool CanPayment(Order order) => false;
        public bool CanShip(Order order) => false;
        public bool CanDeliver(Order order) => false;
        public bool CanCancel(Order order) => false;

        public bool ProcessPayment(Order order) => false;
        public bool ShipOrder(Order order) => false;
        public bool DeliverOrder(Order order) => false;
        public bool CancelOrder(Order order) => false;
    }
}
