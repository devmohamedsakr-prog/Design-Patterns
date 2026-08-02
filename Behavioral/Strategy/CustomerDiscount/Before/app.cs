using System;
using System.Collections.Generic;

namespace CustomerDiscount.Before
{
    // ============================================================================
    // BEFORE: Hard-coded discount logic - No Strategy Pattern
    // ============================================================================
    // PROBLEM: Discount calculations are tightly coupled to Order class
    // - Adding new discount types requires modifying existing code
    // - Difficult to test individual discount logic
    // - Violates Open/Closed Principle
    // - Hard to reuse discount logic
    // ============================================================================

    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public CustomerType Type { get; set; }
        public int YearsAsCustomer { get; set; }

        public Customer(string id, string name, CustomerType type, int yearsAsCustomer = 0)
        {
            Id = id;
            Name = name;
            Type = type;
            YearsAsCustomer = yearsAsCustomer;
        }
    }

    public enum CustomerType
    {
        Regular,
        Premium,
        VIP,
        Loyal
    }

    public class OrderItem
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public OrderItem(string productName, decimal price, int quantity)
        {
            ProductName = productName;
            Price = price;
            Quantity = quantity;
        }

        public decimal GetSubtotal() => Price * Quantity;
    }

    // ============================================================================
    // PROBLEM: All discount logic is embedded in the Order class
    // ============================================================================
    public class Order
    {
        public string OrderId { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public DateTime OrderDate { get; set; }

        public Order(string orderId, Customer customer)
        {
            OrderId = orderId;
            Customer = customer;
            OrderDate = DateTime.Now;
        }

        public void AddItem(OrderItem item)
        {
            Items.Add(item);
        }

        public decimal GetSubtotal()
        {
            decimal subtotal = 0;
            foreach (var item in Items)
            {
                subtotal += item.GetSubtotal();
            }
            return subtotal;
        }

        // ❌ PROBLEM: Discount logic is tightly coupled here
        // ❌ Adding new discount types = modifying this method
        // ❌ Hard to test individual discount strategies
        public decimal CalculateDiscount()
        {
            decimal subtotal = GetSubtotal();
            decimal discount = 0;

            // ❌ Hard-coded if-else chain for different discount types
            if (Customer.Type == CustomerType.Regular)
            {
                // Regular customers: No discount
                discount = 0;
            }
            else if (Customer.Type == CustomerType.Premium)
            {
                // Premium customers: 10% discount
                discount = subtotal * 0.10m;
            }
            else if (Customer.Type == CustomerType.VIP)
            {
                // VIP customers: 20% discount
                discount = subtotal * 0.20m;
            }
            else if (Customer.Type == CustomerType.Loyal)
            {
                // Loyal customers: 5% base + 1% per year
                discount = subtotal * (0.05m + (Customer.YearsAsCustomer * 0.01m));
                if (discount > subtotal * 0.25m) // Cap at 25%
                    discount = subtotal * 0.25m;
            }

            // ❌ What if we want to add SeasonalDiscount? 
            // ❌ What if we want CombinedDiscount?
            // ❌ We have to modify this method every time!

            return Math.Round(discount, 2);
        }

        // ❌ PROBLEM: This is repeated everywhere
        public decimal GetTotal()
        {
            return Math.Round(GetSubtotal() - CalculateDiscount(), 2);
        }

        public void PrintOrder()
        {
            Console.WriteLine($"\n╔════════════════════════════════════╗");
            Console.WriteLine($"║ Order ID: {OrderId,-25}║");
            Console.WriteLine($"║ Customer: {Customer.Name,-24}║");
            Console.WriteLine($"║ Type: {Customer.Type,-29}║");
            Console.WriteLine($"╠════════════════════════════════════╣");
            foreach (var item in Items)
            {
                decimal itemTotal = item.GetSubtotal();
                Console.WriteLine($"║ {item.ProductName,-15} ${itemTotal,8:F2}        ║");
            }
            Console.WriteLine($"╠════════════════════════════════════╣");
            Console.WriteLine($"║ Subtotal:        ${GetSubtotal(),8:F2}        ║");
            Console.WriteLine($"║ Discount:       -${CalculateDiscount(),8:F2}        ║");
            Console.WriteLine($"║ TOTAL:           ${GetTotal(),8:F2}        ║");
            Console.WriteLine($"╚════════════════════════════════════╝");
        }
    }

    // ============================================================================
    // PROBLEMS DEMONSTRATED
    // ============================================================================
    public class OrderProcessor
    {
        public void ProcessOrders()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║      BEFORE: Hard-coded Discount Logic (No Strategy)      ║");
            Console.WriteLine("║         All discount calculations in Order class          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

            // Create customers
            var regularCustomer = new Customer("C001", "Alice", CustomerType.Regular);
            var premiumCustomer = new Customer("C002", "Bob", CustomerType.Premium);
            var vipCustomer = new Customer("C003", "Charlie", CustomerType.VIP);
            var loyalCustomer = new Customer("C004", "Diana", CustomerType.Loyal, 5);

            // Create orders
            var order1 = new Order("ORD001", regularCustomer);
            order1.AddItem(new OrderItem("Laptop", 1000m, 1));
            order1.AddItem(new OrderItem("Mouse", 50m, 2));
            order1.PrintOrder();

            var order2 = new Order("ORD002", premiumCustomer);
            order2.AddItem(new OrderItem("Phone", 800m, 1));
            order2.AddItem(new OrderItem("Screen", 300m, 1));
            order2.PrintOrder();

            var order3 = new Order("ORD003", vipCustomer);
            order3.AddItem(new OrderItem("Keyboard", 200m, 1));
            order3.AddItem(new OrderItem("Cable", 25m, 4));
            order3.PrintOrder();

            var order4 = new Order("ORD004", loyalCustomer);
            order4.AddItem(new OrderItem("Monitor", 500m, 1));
            order4.AddItem(new OrderItem("Desk", 400m, 1));
            order4.PrintOrder();

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    IDENTIFIED PROBLEMS                    ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ ❌ Discount logic tightly coupled to Order class          ║");
            Console.WriteLine("║ ❌ Adding new discount types requires modifying code      ║");
            Console.WriteLine("║ ❌ Hard to test individual discount strategies            ║");
            Console.WriteLine("║ ❌ Hard to combine different discounts                    ║");
            Console.WriteLine("║ ❌ Violates Open/Closed Principle                         ║");
            Console.WriteLine("║ ❌ No reusability of discount logic                       ║");
            Console.WriteLine("║ ❌ Difficult to swap discount algorithms at runtime       ║");
            Console.WriteLine("║ ❌ Code becomes harder to maintain as discount rules      ║");
            Console.WriteLine("║    grow more complex                                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        }
    }

    public class Program
    {
        public static void Main()
        {
            var processor = new OrderProcessor();
            processor.ProcessOrders();
        }
    }
}
