using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.After.Context
{
    /// <summary>
    /// CartItem: Product in shopping cart
    /// </summary>
    public class CartItem
    {
        public string ProductId { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal GetTotal() => Price * Quantity;
    }

    /// <summary>
    /// CartMemento: Snapshot of shopping cart state
    /// </summary>
    public class CartMemento
    {
        public string CustomerId { get; set; } = "";
        public List<CartItem> Items { get; set; } = new();
        public DateTime SnapshotTime { get; set; }
        public string SnapshotName { get; set; } = "";

        public CartMemento(string customerId, List<CartItem> items, string name)
        {
            CustomerId = customerId;
            Items = new List<CartItem>(items.Select(i => new CartItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }));
            SnapshotTime = DateTime.Now;
            SnapshotName = name;
        }

        public override string ToString() => $"{SnapshotName} ({SnapshotTime:yyyy-MM-dd HH:mm:ss}) - {Items.Count} items";
    }

    /// <summary>
    /// ShoppingCart: Originator - manages cart state and creates mementos
    /// </summary>
    public class ShoppingCart
    {
        public string CustomerId { get; set; } = "";
        public List<CartItem> Items { get; private set; } = new();

        public ShoppingCart(string customerId)
        {
            CustomerId = customerId;
        }

        public void AddItem(CartItem item)
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
                Console.WriteLine($"  ✓ Added {item.ProductName} (${item.Price} x {item.Quantity})");
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

        public decimal GetTotal() => Items.Sum(i => i.GetTotal());

        public void ClearCart()
        {
            Items.Clear();
            Console.WriteLine($"  ✓ Cart cleared");
        }

        public CartMemento SaveSnapshot(string snapshotName)
        {
            var memento = new CartMemento(CustomerId, Items, snapshotName);
            Console.WriteLine($"📸 Cart snapshot saved: {memento}");
            return memento;
        }

        public void RestoreSnapshot(CartMemento memento)
        {
            Items = new List<CartItem>(memento.Items.Select(i => new CartItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }));
            Console.WriteLine($"↶ Cart restored from: {memento}");
        }

        public override string ToString() => $"Cart({Items.Count} items, Total: ${GetTotal():F2})";
    }

    /// <summary>
    /// CartCaretaker: Manages cart snapshots
    /// </summary>
    public class CartCaretaker
    {
        private Dictionary<string, CartMemento> _snapshots = new();
        private List<CartMemento> _history = new();

        public void SaveSnapshot(ShoppingCart cart, string snapshotName)
        {
            var memento = cart.SaveSnapshot(snapshotName);
            _snapshots[snapshotName] = memento;
            _history.Add(memento);
        }

        public void RestoreSnapshot(ShoppingCart cart, string snapshotName)
        {
            if (_snapshots.TryGetValue(snapshotName, out var memento))
            {
                cart.RestoreSnapshot(memento);
            }
            else
            {
                Console.WriteLine($"✗ Snapshot '{snapshotName}' not found");
            }
        }

        public List<string> GetAvailableSnapshots() => _snapshots.Keys.ToList();

        public CartMemento? GetSnapshot(string snapshotName) => 
            _snapshots.TryGetValue(snapshotName, out var memento) ? memento : null;

        public int GetSnapshotCount() => _snapshots.Count;

        public List<CartMemento> GetHistory() => _history;

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
    }
}
