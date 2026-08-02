using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Before
{
    /// <summary>
    /// BEFORE: Shopping Cart WITHOUT Memento Pattern
    /// Problem: Cannot undo/restore previous cart states
    /// </summary>
    public class CartItem
    {
        public string ProductId { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal GetTotal() => Price * Quantity;
    }

    public class ShoppingCartBefore
    {
        public string CustomerId { get; set; } = "";
        public List<CartItem> Items { get; private set; } = new();
        private List<string> _actionLog = new();

        public ShoppingCartBefore(string customerId)
        {
            CustomerId = customerId;
        }

        public void AddItem(CartItem item)
        {
            var existing = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                _actionLog.Add($"[{DateTime.Now:HH:mm:ss}] Updated {item.ProductName} quantity to {existing.Quantity}");
            }
            else
            {
                Items.Add(item);
                _actionLog.Add($"[{DateTime.Now:HH:mm:ss}] Added {item.ProductName} (${item.Price} x {item.Quantity})");
            }
            Console.WriteLine($"  ✓ Added/Updated {item.ProductName}");
        }

        public void RemoveItem(string productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
                _actionLog.Add($"[{DateTime.Now:HH:mm:ss}] Removed {item.ProductName}");
                Console.WriteLine($"  ✓ Removed {item.ProductName}");
            }
        }

        public decimal GetTotal() => Items.Sum(i => i.GetTotal());

        public void ClearCart()
        {
            Items.Clear();
            _actionLog.Add($"[{DateTime.Now:HH:mm:ss}] Cart cleared");
            Console.WriteLine($"  ✓ Cart cleared");
        }

        public void DisplayActionLog()
        {
            Console.WriteLine("  Action Log:");
            foreach (var action in _actionLog)
            {
                Console.WriteLine($"    {action}");
            }
        }

        // ❌ PROBLEM: No way to undo or restore to previous states!
        // - Users cannot recover accidentally deleted items
        // - Cannot compare different cart configurations
        // - Manual logs don't help restore actual state
        // - Lost if browser crashes or session ends

        public override string ToString() => $"Cart({Items.Count} items, Total: ${GetTotal():F2})";
    }

    /// <summary>
    /// APPLICATION 1: E-Commerce Store WITHOUT Memento (STRUGGLES)
    /// Scenario: User adds items, clears cart by mistake, wants to undo
    /// </summary>
    public class ECommerceStoreWithoutMemento
    {
        public static void Demo()
        {
            Console.WriteLine("\n=== APPLICATION 1: E-Commerce WITHOUT Memento ===");
            Console.WriteLine("Scenario: User accidentally clears cart\n");

            var cart = new ShoppingCartBefore("CUST001");

            // Build cart
            Console.WriteLine("1️⃣ Adding items to cart:");
            cart.AddItem(new CartItem { ProductId = "LAPTOP", ProductName = "Laptop", Price = 999.99m, Quantity = 1 });
            cart.AddItem(new CartItem { ProductId = "MOUSE", ProductName = "Mouse", Price = 29.99m, Quantity = 2 });
            cart.AddItem(new CartItem { ProductId = "KEYBOARD", ProductName = "Keyboard", Price = 79.99m, Quantity = 1 });
            Console.WriteLine($"   {cart}\n");

            // User clears cart by mistake
            Console.WriteLine("2️⃣ User accidentally clears cart:");
            cart.ClearCart();
            Console.WriteLine($"   {cart}\n");

            // Try to recover
            Console.WriteLine("3️⃣ User wants to UNDO (restore previous cart):");
            Console.WriteLine("   ❌ PROBLEM: NO WAY TO UNDO!");
            Console.WriteLine("   - Action log exists but doesn't help");
            cart.DisplayActionLog();
            Console.WriteLine("   - User must manually re-add all items");
            Console.WriteLine("   - Wasted time, frustration, potential lost sale!\n");
        }
    }

    /// <summary>
    /// APPLICATION 2: Shopping Cart Comparison WITHOUT Memento (STRUGGLES)
    /// Scenario: User wants to compare two different shopping options
    /// </summary>
    public class ShoppingCartComparisonWithoutMemento
    {
        public static void Demo()
        {
            Console.WriteLine("\n=== APPLICATION 2: Cart Comparison WITHOUT Memento ===");
            Console.WriteLine("Scenario: User wants to compare gaming PC vs workstation builds\n");

            // Option 1: Gaming Build
            Console.WriteLine("1️⃣ Building Gaming PC option:");
            var gamingCart = new ShoppingCartBefore("CUST002");
            gamingCart.AddItem(new CartItem { ProductId = "GPU1", ProductName = "RTX 4090", Price = 1599.99m, Quantity = 1 });
            gamingCart.AddItem(new CartItem { ProductId = "CPU1", ProductName = "Intel i9-13900K", Price = 699.99m, Quantity = 1 });
            gamingCart.AddItem(new CartItem { ProductId = "RAM1", ProductName = "64GB DDR5", Price = 399.99m, Quantity = 1 });
            Console.WriteLine($"   Gaming Total: ${gamingCart.GetTotal():F2}\n");

            // Try to switch to workstation
            Console.WriteLine("2️⃣ Modifying cart to Workstation option:");
            gamingCart.ClearCart();
            gamingCart.AddItem(new CartItem { ProductId = "GPU2", ProductName = "RTX 6000 Ada", Price = 6799.99m, Quantity = 1 });
            gamingCart.AddItem(new CartItem { ProductId = "CPU2", ProductName = "Xeon Platinum", Price = 3999.99m, Quantity = 1 });
            gamingCart.AddItem(new CartItem { ProductId = "RAM2", ProductName = "256GB DDR5", Price = 1999.99m, Quantity = 1 });
            Console.WriteLine($"   Workstation Total: ${gamingCart.GetTotal():F2}\n");

            // Problem: Can't compare anymore!
            Console.WriteLine("3️⃣ User wants to compare both options SIDE-BY-SIDE:");
            Console.WriteLine("   ❌ PROBLEM: NO SNAPSHOTS!");
            Console.WriteLine("   - Gaming build info is lost");
            Console.WriteLine("   - Cannot easily switch back and forth");
            Console.WriteLine("   - Would need separate browser windows or manual notes");
            Console.WriteLine("   - Poor user experience!\n");
        }
    }
}
