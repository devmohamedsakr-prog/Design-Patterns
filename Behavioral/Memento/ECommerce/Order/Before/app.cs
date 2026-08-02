using System;
using System.Collections.Generic;
using System.Linq;

namespace Order.Before
{
    /// <summary>
    /// BEFORE: Order Management WITHOUT Memento Pattern
    /// Problem: Cannot undo/rollback order state changes
    /// </summary>
    
    public enum OrderStatus
    {
        Created,
        Confirmed,
        PaymentVerified,
        InventoryReserved,
        Picked,
        Packaged,
        Shipped,
        Delivered,
        Cancelled
    }

    public class OrderItem
    {
        public string ProductId { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal GetTotal() => UnitPrice * Quantity;
    }

    public class ShippingAddress
    {
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string PostalCode { get; set; } = "";

        public override string ToString() => $"{Street}, {City}, {Country} {PostalCode}";
    }

    public class OrderBefore
    {
        public string OrderId { get; set; } = "";
        public string CustomerId { get; set; } = "";
        public List<OrderItem> Items { get; private set; } = new();
        public OrderStatus Status { get; private set; } = OrderStatus.Created;
        public ShippingAddress ShippingAddress { get; set; } = new();
        public string ShippingMethod { get; set; } = "Standard";
        public decimal ShippingCost { get; private set; } = 10m;
        private List<string> _statusLog = new();

        public OrderBefore(string orderId, string customerId)
        {
            OrderId = orderId;
            CustomerId = customerId;
            _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Order created");
        }

        public void AddItem(OrderItem item)
        {
            var existing = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Updated {item.ProductName} quantity to {existing.Quantity}");
            }
            else
            {
                Items.Add(item);
                _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Added {item.ProductName}");
            }
            Console.WriteLine($"  ✓ Added/Updated {item.ProductName}");
        }

        public void RemoveItem(string productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
                _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Removed {item.ProductName}");
                Console.WriteLine($"  ✓ Removed {item.ProductName}");
            }
        }

        public decimal GetSubtotal() => Items.Sum(i => i.GetTotal());

        public decimal GetTotal() => GetSubtotal() + ShippingCost;

        public void SetShippingMethod(string method)
        {
            ShippingMethod = method;
            ShippingCost = method switch
            {
                "Express" => 25m,
                "International" => 50m,
                _ => 10m  // Standard
            };
            _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Shipping method changed to {method}");
            Console.WriteLine($"  ✓ Shipping method set to {method} (${ShippingCost})");
        }

        public void ConfirmOrder()
        {
            Status = OrderStatus.Confirmed;
            _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Order confirmed");
            Console.WriteLine($"  ✓ Order confirmed");
        }

        public void VerifyPayment()
        {
            Status = OrderStatus.PaymentVerified;
            _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Payment verified");
            Console.WriteLine($"  ✓ Payment verified");
        }

        public void ReserveInventory()
        {
            Status = OrderStatus.InventoryReserved;
            _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Inventory reserved");
            Console.WriteLine($"  ✓ Inventory reserved");
        }

        public void PickItems()
        {
            Status = OrderStatus.Picked;
            _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Items picked from warehouse");
            Console.WriteLine($"  ✓ Items picked");
        }

        public void PackageOrder()
        {
            Status = OrderStatus.Packaged;
            _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Order packaged");
            Console.WriteLine($"  ✓ Order packaged");
        }

        public void ShipOrder()
        {
            Status = OrderStatus.Shipped;
            _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Order shipped");
            Console.WriteLine($"  ✓ Order shipped");
        }

        public void CancelOrder()
        {
            Status = OrderStatus.Cancelled;
            _statusLog.Add($"[{DateTime.Now:HH:mm:ss}] Order cancelled");
            Console.WriteLine($"  ✓ Order cancelled");
        }

        public void DisplayStatusLog()
        {
            Console.WriteLine("  Status Log:");
            foreach (var log in _statusLog)
            {
                Console.WriteLine($"    {log}");
            }
        }

        // ❌ PROBLEM: No way to undo or rollback to previous states!
        // - Admins cannot recover accidentally shipped orders
        // - Cannot compare different fulfillment strategies
        // - Manual logs don't help restore actual state
        // - System failures leave order in inconsistent state

        public override string ToString() => $"Order({OrderId}, {Status}, Items: {Items.Count}, Total: ${GetTotal():F2})";
    }

    /// <summary>
    /// APPLICATION 1: Order Processing WITHOUT Memento (STRUGGLES)
    /// Scenario: Admin accidentally ships order before payment verified
    /// </summary>
    public class OrderProcessingWithoutMemento
    {
        public static void Demo()
        {
            Console.WriteLine("\n=== APPLICATION 1: Order Processing WITHOUT Memento ===");
            Console.WriteLine("Scenario: Admin accidentally ships order too early\n");

            var order = new OrderBefore("ORD-001", "CUST-001");

            // Build order
            Console.WriteLine("1️⃣ Building order:");
            order.AddItem(new OrderItem { ProductId = "LAPTOP", ProductName = "Laptop", UnitPrice = 999.99m, Quantity = 1 });
            order.AddItem(new OrderItem { ProductId = "MOUSE", ProductName = "Mouse", UnitPrice = 29.99m, Quantity = 2 });
            Console.WriteLine($"   {order}\n");

            // Process order
            Console.WriteLine("2️⃣ Processing order:");
            order.ConfirmOrder();
            Console.WriteLine("   ✓ Order confirmed");

            // MISTAKE: Ship before payment verified
            Console.WriteLine("\n3️⃣ Admin accidentally ships order (BEFORE payment verified!):");
            order.ShipOrder();
            Console.WriteLine($"   {order}\n");

            // Try to recover
            Console.WriteLine("4️⃣ Admin realizes mistake - needs to UNDO shipment:");
            Console.WriteLine("   ❌ PROBLEM: NO WAY TO UNDO!");
            Console.WriteLine("   - Status log exists but doesn't help");
            order.DisplayStatusLog();
            Console.WriteLine("   - Manual system intervention required");
            Console.WriteLine("   - Operational complexity and cost!");
            Console.WriteLine("   - Customer needs to be contacted manually!\n");
        }
    }

    /// <summary>
    /// APPLICATION 2: Order Fulfillment Strategy Comparison WITHOUT Memento (STRUGGLES)
    /// Scenario: Admin wants to compare shipping methods to find best strategy
    /// </summary>
    public class OrderStrategyComparisonWithoutMemento
    {
        public static void Demo()
        {
            Console.WriteLine("\n=== APPLICATION 2: Strategy Comparison WITHOUT Memento ===");
            Console.WriteLine("Scenario: Admin comparing shipping strategies\n");

            // Strategy 1: Standard Shipping
            Console.WriteLine("1️⃣ Building order with Standard shipping:");
            var order = new OrderBefore("ORD-002", "CUST-002");
            order.AddItem(new OrderItem { ProductId = "PHONE", ProductName = "iPhone", UnitPrice = 999m, Quantity = 1 });
            order.AddItem(new OrderItem { ProductId = "CASE", ProductName = "Case", UnitPrice = 25m, Quantity = 1 });
            order.SetShippingMethod("Standard");
            Console.WriteLine($"   Standard Total: ${order.GetTotal():F2}\n");

            // Strategy 2: Express Shipping
            Console.WriteLine("2️⃣ Switching to Express shipping:");
            order.SetShippingMethod("Express");
            Console.WriteLine($"   Express Total: ${order.GetTotal():F2}\n");

            // Strategy 3: International Shipping
            Console.WriteLine("3️⃣ Switching to International shipping:");
            order.SetShippingMethod("International");
            Console.WriteLine($"   International Total: ${order.GetTotal():F2}\n");

            // Problem: Can't compare anymore!
            Console.WriteLine("4️⃣ Admin wants to compare all three options SIDE-BY-SIDE:");
            Console.WriteLine("   ❌ PROBLEM: NO SNAPSHOTS!");
            Console.WriteLine("   - Standard shipping info is lost");
            Console.WriteLine("   - Cannot easily switch back and forth");
            Console.WriteLine("   - Would need multiple browser tabs or manual notes");
            Console.WriteLine("   - Poor decision-making capability!\n");
        }
    }
}
