using System;
using OrderProcessing.After.Abstracts;

namespace OrderProcessing.After.Commands
{
    public class ValidateOrderCommand : ICommand
    {
        private Order _order;
        private bool _wasValid;

        public ValidateOrderCommand(Order order)
        {
            _order = order;
        }

        public bool Execute()
        {
            _wasValid = _order.IsValid;
            if (string.IsNullOrEmpty(_order.CustomerId) || _order.Total <= 0) return false;
            _order.IsValid = true;
            return true;
        }

        public bool Undo()
        {
            _order.IsValid = _wasValid;
            return true;
        }

        public string GetDescription() => "ValidateOrder";
    }

    public class ProcessPaymentCommand : ICommand
    {
        private Order _order;
        private decimal _previousBalance;

        public ProcessPaymentCommand(Order order)
        {
            _order = order;
        }

        public bool Execute()
        {
            if (!_order.IsValid || _order.Total <= 0 || _order.AccountBalance < _order.Total) 
                return false;
            _previousBalance = _order.AccountBalance;
            _order.AccountBalance -= _order.Total;
            _order.PaymentProcessed = true;
            return true;
        }

        public bool Undo()
        {
            _order.AccountBalance = _previousBalance;
            _order.PaymentProcessed = false;
            return true;
        }

        public string GetDescription() => "ProcessPayment";
    }

    public class ReserveInventoryCommand : ICommand
    {
        private Order _order;
        private bool _wasReserved;

        public ReserveInventoryCommand(Order order)
        {
            _order = order;
        }

        public bool Execute()
        {
            if (!_order.PaymentProcessed) return false;
            _wasReserved = _order.InventoryReserved;
            _order.InventoryReserved = true;
            return true;
        }

        public bool Undo()
        {
            _order.InventoryReserved = _wasReserved;
            return true;
        }

        public string GetDescription() => "ReserveInventory";
    }

    public class ShipOrderCommand : ICommand
    {
        private Order _order;
        private bool _wasShipped;

        public ShipOrderCommand(Order order)
        {
            _order = order;
        }

        public bool Execute()
        {
            if (!_order.InventoryReserved) return false;
            _wasShipped = _order.Shipped;
            _order.Shipped = true;
            _order.ShipDate = DateTime.UtcNow;
            return true;
        }

        public bool Undo()
        {
            _order.Shipped = _wasShipped;
            _order.ShipDate = null;
            return true;
        }

        public string GetDescription() => "ShipOrder";
    }

    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public decimal Total { get; set; }
        public decimal AccountBalance { get; set; }
        public bool IsValid { get; set; }
        public bool PaymentProcessed { get; set; }
        public bool InventoryReserved { get; set; }
        public bool Shipped { get; set; }
        public DateTime? ShipDate { get; set; }
    }
}
