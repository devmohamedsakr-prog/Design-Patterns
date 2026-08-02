using System;
using System.Collections.Generic;

namespace Facade.ECommerce.Checkout.Component
{
    // Subsystem 1: Inventory Management
    public class InventoryManager
    {
        private Dictionary<string, int> _stock = new();

        public InventoryManager() => _stock["PROD001"] = 100;

        public bool CheckStock(string productId, int quantity)
        {
            return _stock.ContainsKey(productId) && _stock[productId] >= quantity;
        }

        public void ReserveItems(string productId, int quantity)
        {
            if (_stock.ContainsKey(productId))
                _stock[productId] -= quantity;
        }

        public void ReleaseReservation(string productId, int quantity)
        {
            if (_stock.ContainsKey(productId))
                _stock[productId] += quantity;
        }

        public int GetAvailableStock(string productId) => _stock.ContainsKey(productId) ? _stock[productId] : 0;
    }

    // Subsystem 2: Payment Processing
    public class PaymentProcessor
    {
        public bool ValidateCard(string cardNumber)
        {
            return cardNumber.Length == 16 && cardNumber.All(char.IsDigit);
        }

        public bool ProcessPayment(string cardNumber, decimal amount)
        {
            if (!ValidateCard(cardNumber)) return false;
            return amount > 0;
        }

        public string GetTransactionId() => Guid.NewGuid().ToString("N").Substring(0, 12);
    }

    // Subsystem 3: Shipping/Logistics
    public class ShippingCalculator
    {
        public decimal CalculateShippingCost(string address, double weightKg)
        {
            if (address.Contains("Express")) return 50;
            return 10 + (decimal)(weightKg * 2);
        }

        public string SelectCarrier(string address)
        {
            return address.Contains("Local") ? "Local Courier" : "National Carrier";
        }

        public string GenerateTrackingNumber() => $"TRACK{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}";
    }

    // Subsystem 4: Tax Calculation
    public class TaxCalculator
    {
        public decimal CalculateTax(decimal subtotal, string region)
        {
            var taxRate = region switch
            {
                "California" => 0.0825m,
                "Texas" => 0.0625m,
                "NY" => 0.04m,
                _ => 0.05m
            };
            return subtotal * taxRate;
        }
    }

    // Subsystem 5: Notification System
    public class NotificationService
    {
        private List<string> _notifications = new();

        public void SendOrderConfirmation(string email, string orderId)
        {
            _notifications.Add($"[EMAIL] Order {orderId} confirmed to {email}");
        }

        public void SendShippingNotification(string email, string trackingNumber)
        {
            _notifications.Add($"[EMAIL] Shipped with tracking {trackingNumber} to {email}");
        }

        public IReadOnlyList<string> GetNotifications() => _notifications.AsReadOnly();
    }

    // Order Model
    public class Order
    {
        public string OrderId { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string TrackingNumber { get; set; }
    }

    public class CartItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    // FACADE: Simplifies checkout workflow
    public class ECommerceFacade
    {
        private InventoryManager _inventory = new();
        private PaymentProcessor _paymentProcessor = new();
        private ShippingCalculator _shippingCalculator = new();
        private TaxCalculator _taxCalculator = new();
        private NotificationService _notifications = new();

        public Order CheckoutCart(List<CartItem> items, string email, string cardNumber, string address, string region)
        {
            var order = new Order { OrderId = Guid.NewGuid().ToString("N").Substring(0, 8) };

            // Check inventory
            foreach (var item in items)
            {
                if (!_inventory.CheckStock(item.ProductId, item.Quantity))
                    return null;
            }

            // Reserve items
            foreach (var item in items)
            {
                _inventory.ReserveItems(item.ProductId, item.Quantity);
                order.Subtotal += item.Price * item.Quantity;
            }

            // Calculate costs
            order.Tax = _taxCalculator.CalculateTax(order.Subtotal, region);
            order.Shipping = _shippingCalculator.CalculateShippingCost(address, 2.5);
            order.Total = order.Subtotal + order.Tax + order.Shipping;

            // Process payment
            if (!_paymentProcessor.ProcessPayment(cardNumber, order.Total))
            {
                foreach (var item in items)
                    _inventory.ReleaseReservation(item.ProductId, item.Quantity);
                return null;
            }

            // Generate tracking
            order.TrackingNumber = _shippingCalculator.GenerateTrackingNumber();
            order.Status = "Confirmed";
            order.Items = items;

            // Send notifications
            _notifications.SendOrderConfirmation(email, order.OrderId);
            _notifications.SendShippingNotification(email, order.TrackingNumber);

            return order;
        }

        public IReadOnlyList<string> GetNotifications() => _notifications.GetNotifications();
    }
}
