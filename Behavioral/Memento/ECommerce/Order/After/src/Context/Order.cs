using System;
using System.Collections.Generic;
using System.Linq;

namespace Order.After.Context
{
    /// <summary>
    /// OrderStatus: Represents stages in order lifecycle
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

    /// <summary>
    /// OrderItem: Product in order
    /// </summary>
    public class OrderItem
    {
        public string ProductId { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal GetTotal() => UnitPrice * Quantity;
    }

    /// <summary>
    /// ShippingAddress: Delivery address
    /// </summary>
    public class ShippingAddress
    {
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string PostalCode { get; set; } = "";

        public override string ToString() => $"{Street}, {City}, {Country} {PostalCode}";
    }

    /// <summary>
    /// OrderMemento: Immutable snapshot of order state
    /// Captures complete order at a point in time
    /// </summary>
    public class OrderMemento
    {
        public string OrderId { get; set; } = "";
        public string CustomerId { get; set; } = "";
        public List<OrderItem> Items { get; set; } = new();
        public OrderStatus Status { get; set; }
        public ShippingAddress ShippingAddress { get; set; } = new();
        public string ShippingMethod { get; set; } = "";
        public decimal ShippingCost { get; set; }
        public DateTime SnapshotTime { get; set; }
        public string SnapshotName { get; set; } = "";

        public OrderMemento(
            string orderId,
            string customerId,
            List<OrderItem> items,
            OrderStatus status,
            ShippingAddress address,
            string shippingMethod,
            decimal shippingCost,
            string name)
        {
            OrderId = orderId;
            CustomerId = customerId;
            Items = new List<OrderItem>(items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }));
            Status = status;
            ShippingAddress = new ShippingAddress
            {
                Street = address.Street,
                City = address.City,
                Country = address.Country,
                PostalCode = address.PostalCode
            };
            ShippingMethod = shippingMethod;
            ShippingCost = shippingCost;
            SnapshotTime = DateTime.Now;
            SnapshotName = name;
        }

        public decimal GetSubtotal() => Items.Sum(i => i.GetTotal());

        public decimal GetTotal() => GetSubtotal() + ShippingCost;

        public override string ToString() => 
            $"{SnapshotName} ({SnapshotTime:yyyy-MM-dd HH:mm:ss}) - Status: {Status}, Items: {Items.Count}, Total: ${GetTotal():F2}";
    }

    /// <summary>
    /// Order: Originator - manages order state and creates mementos
    /// </summary>
    public class Order
    {
        public string OrderId { get; set; } = "";
        public string CustomerId { get; set; } = "";
        public List<OrderItem> Items { get; private set; } = new();
        public OrderStatus Status { get; private set; } = OrderStatus.Created;
        public ShippingAddress ShippingAddress { get; set; } = new();
        public string ShippingMethod { get; set; } = "Standard";
        public decimal ShippingCost { get; private set; } = 10m;

        public Order(string orderId, string customerId)
        {
            OrderId = orderId;
            CustomerId = customerId;
        }

        public void AddItem(OrderItem item)
        {
            var existing = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                Console.WriteLine($"  ✓ Updated {item.ProductName} quantity to {existing.Quantity}");
            }
            else
            {
                Items.Add(item);
                Console.WriteLine($"  ✓ Added {item.ProductName} (${item.UnitPrice} x {item.Quantity})");
            }
        }

        public void RemoveItem(string productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
                Console.WriteLine($"  ✓ Removed {item.ProductName}");
            }
        }

        public decimal GetSubtotal() => Items.Sum(i => i.GetTotal());

        public decimal GetTotal() => GetSubtotal() + ShippingCost;

        public void SetShippingAddress(ShippingAddress address)
        {
            ShippingAddress = address;
            Console.WriteLine($"  ✓ Shipping address set to {address}");
        }

        public void SetShippingMethod(string method)
        {
            ShippingMethod = method;
            ShippingCost = method switch
            {
                "Express" => 25m,
                "International" => 50m,
                _ => 10m  // Standard
            };
            Console.WriteLine($"  ✓ Shipping method set to {method} (${ShippingCost})");
        }

        public void ConfirmOrder()
        {
            Status = OrderStatus.Confirmed;
            Console.WriteLine($"  ✓ Order confirmed");
        }

        public void VerifyPayment()
        {
            Status = OrderStatus.PaymentVerified;
            Console.WriteLine($"  ✓ Payment verified");
        }

        public void ReserveInventory()
        {
            Status = OrderStatus.InventoryReserved;
            Console.WriteLine($"  ✓ Inventory reserved");
        }

        public void PickItems()
        {
            Status = OrderStatus.Picked;
            Console.WriteLine($"  ✓ Items picked from warehouse");
        }

        public void PackageOrder()
        {
            Status = OrderStatus.Packaged;
            Console.WriteLine($"  ✓ Order packaged");
        }

        public void ShipOrder()
        {
            Status = OrderStatus.Shipped;
            Console.WriteLine($"  ✓ Order shipped");
        }

        public void DeliverOrder()
        {
            Status = OrderStatus.Delivered;
            Console.WriteLine($"  ✓ Order delivered");
        }

        public void CancelOrder()
        {
            Status = OrderStatus.Cancelled;
            Console.WriteLine($"  ✓ Order cancelled");
        }

        public OrderMemento SaveSnapshot(string snapshotName)
        {
            var memento = new OrderMemento(
                OrderId,
                CustomerId,
                Items,
                Status,
                ShippingAddress,
                ShippingMethod,
                ShippingCost,
                snapshotName);
            Console.WriteLine($"📸 Order snapshot saved: {memento}");
            return memento;
        }

        public void RestoreSnapshot(OrderMemento memento)
        {
            Items = new List<OrderItem>(memento.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }));
            Status = memento.Status;
            ShippingAddress = new ShippingAddress
            {
                Street = memento.ShippingAddress.Street,
                City = memento.ShippingAddress.City,
                Country = memento.ShippingAddress.Country,
                PostalCode = memento.ShippingAddress.PostalCode
            };
            ShippingMethod = memento.ShippingMethod;
            ShippingCost = memento.ShippingCost;
            Console.WriteLine($"↶ Order restored from: {memento}");
        }

        public override string ToString() => 
            $"Order({OrderId}, {Status}, Items: {Items.Count}, Total: ${GetTotal():F2})";
    }

    /// <summary>
    /// OrderCaretaker: Manages order snapshots
    /// Handles save/restore/delete/list operations
    /// </summary>
    public class OrderCaretaker
    {
        private Dictionary<string, OrderMemento> _snapshots = new();
        private List<OrderMemento> _history = new();

        public void SaveSnapshot(Order order, string snapshotName)
        {
            var memento = order.SaveSnapshot(snapshotName);
            _snapshots[snapshotName] = memento;
            _history.Add(memento);
        }

        public void RestoreSnapshot(Order order, string snapshotName)
        {
            if (_snapshots.TryGetValue(snapshotName, out var memento))
            {
                order.RestoreSnapshot(memento);
            }
            else
            {
                Console.WriteLine($"✗ Snapshot '{snapshotName}' not found");
            }
        }

        public List<string> GetAvailableSnapshots() => _snapshots.Keys.ToList();

        public OrderMemento? GetSnapshot(string snapshotName) => 
            _snapshots.TryGetValue(snapshotName, out var memento) ? memento : null;

        public int GetSnapshotCount() => _snapshots.Count;

        public List<OrderMemento> GetHistory() => _history;

        public void DeleteSnapshot(string snapshotName)
        {
            if (_snapshots.Remove(snapshotName))
            {
                Console.WriteLine($"🗑 Snapshot '{snapshotName}' deleted");
            }
        }

        public void ListSnapshots()
        {
            if (_snapshots.Count == 0)
            {
                Console.WriteLine("  (No snapshots saved)");
                return;
            }

            foreach (var kvp in _snapshots)
            {
                Console.WriteLine($"  • {kvp.Key}: {kvp.Value}");
            }
        }

        public decimal CompareOrderTotals(string snapshot1, string snapshot2)
        {
            var mem1 = GetSnapshot(snapshot1);
            var mem2 = GetSnapshot(snapshot2);

            if (mem1 == null || mem2 == null)
            {
                return 0m;
            }

            return Math.Abs(mem1.GetTotal() - mem2.GetTotal());
        }
    }
}
