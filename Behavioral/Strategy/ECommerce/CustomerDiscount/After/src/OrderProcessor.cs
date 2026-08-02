using System;
using System.Collections.Generic;

namespace CustomerDiscount.After
{
    /// <summary>
    /// Order processor that demonstrates the Strategy pattern.
    /// Uses different discount strategies for different scenarios.
    /// SRP: Single Responsibility - Process orders with strategy pattern
    /// </summary>
    public class StrategyOrderProcessor
    {
        public void ProcessOrders()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║      AFTER: Strategy Pattern for Discount Calculation      ║");
            Console.WriteLine("║    Each strategy independently calculates discounts        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

            // ================================================================
            // SCENARIO 1: Regular Customer - No Discount Strategy
            // ================================================================
            Console.WriteLine("\n┌─ SCENARIO 1: Regular Customer ─┐\n");
            var regularCustomer = new Customer("C001", "Alice Johnson", CustomerType.Regular);
            var regularStrategy = new RegularCustomerStrategy();
            var order1 = new Order("ORD001", regularCustomer, regularStrategy);
            order1.AddItem(new OrderItem("Laptop", 1000m, 1));
            order1.AddItem(new OrderItem("Mouse", 50m, 2));
            order1.PrintOrder();

            // ================================================================
            // SCENARIO 2: Premium Customer - Premium Strategy
            // ================================================================
            Console.WriteLine("\n┌─ SCENARIO 2: Premium Customer (10% discount) ─┐\n");
            var premiumCustomer = new Customer("C002", "Bob Smith", CustomerType.Premium);
            var premiumStrategy = new PremiumCustomerStrategy();
            var order2 = new Order("ORD002", premiumCustomer, premiumStrategy);
            order2.AddItem(new OrderItem("Phone", 800m, 1));
            order2.AddItem(new OrderItem("Screen", 300m, 1));
            order2.PrintOrder();

            // ================================================================
            // SCENARIO 3: VIP Customer - VIP Strategy
            // ================================================================
            Console.WriteLine("\n┌─ SCENARIO 3: VIP Customer (20% discount) ─┐\n");
            var vipCustomer = new Customer("C003", "Charlie Brown", CustomerType.VIP);
            var vipStrategy = new VIPCustomerStrategy();
            var order3 = new Order("ORD003", vipCustomer, vipStrategy);
            order3.AddItem(new OrderItem("Keyboard", 200m, 1));
            order3.AddItem(new OrderItem("Cable", 25m, 4));
            order3.PrintOrder();

            // ================================================================
            // SCENARIO 4: Loyal Customer - Loyal Strategy
            // ================================================================
            Console.WriteLine("\n┌─ SCENARIO 4: Loyal Customer (5% + 1% per year) ─┐\n");
            var loyalCustomer = new Customer("C004", "Diana Prince", CustomerType.Loyal, 5);
            var loyalStrategy = new LoyalCustomerStrategy();
            var order4 = new Order("ORD004", loyalCustomer, loyalStrategy);
            order4.AddItem(new OrderItem("Monitor", 500m, 1));
            order4.AddItem(new OrderItem("Desk", 400m, 1));
            order4.PrintOrder();

            // ================================================================
            // SCENARIO 5: Volume Discount - Can be used with any customer
            // ================================================================
            Console.WriteLine("\n┌─ SCENARIO 5: Large Order (Volume Discount) ─┐\n");
            var volumeCustomer = new Customer("C005", "Eve Wilson", CustomerType.Regular);
            var volumeStrategy = new VolumeDiscountStrategy();
            var order5 = new Order("ORD005", volumeCustomer, volumeStrategy);
            order5.AddItem(new OrderItem("Pen", 1m, 15));
            order5.AddItem(new OrderItem("Notebook", 5m, 10));
            order5.AddItem(new OrderItem("Pencil", 0.5m, 20));
            order5.PrintOrder();

            // ================================================================
            // SCENARIO 6: Seasonal Discount - Dynamic based on date
            // ================================================================
            Console.WriteLine("\n┌─ SCENARIO 6: Seasonal Discount ─┐\n");
            var seasonalCustomer = new Customer("C006", "Frank Miller", CustomerType.Regular);
            var seasonalStrategy = new SeasonalDiscountStrategy();
            var order6 = new Order("ORD006", seasonalCustomer, seasonalStrategy);
            order6.AddItem(new OrderItem("T-Shirt", 30m, 2));
            order6.AddItem(new OrderItem("Shorts", 50m, 1));
            order6.PrintOrder();

            // ================================================================
            // SCENARIO 7: First-Time Customer - Special Welcome Discount
            // ================================================================
            Console.WriteLine("\n┌─ SCENARIO 7: First-Time Customer (Welcome Discount) ─┐\n");
            var newCustomer = new Customer("C007", "Grace Lee", CustomerType.Regular, 0);
            var firstTimeStrategy = new FirstTimeCustomerStrategy();
            var order7 = new Order("ORD007", newCustomer, firstTimeStrategy);
            order7.AddItem(new OrderItem("Welcome Bundle", 200m, 1));
            order7.PrintOrder();

            // ================================================================
            // SCENARIO 8: Composite Strategy - Multiple discounts combined
            // ================================================================
            Console.WriteLine("\n┌─ SCENARIO 8: Composite Strategy (Multiple Discounts) ─┐\n");
            var compositeCustomer = new Customer("C008", "Henry Davis", CustomerType.VIP, 3);
            var compositeStrategy = new CompositeDiscountStrategy(
                new VIPCustomerStrategy(),      // 20%
                new LoyalCustomerStrategy()     // 5% + 1% per year
            );
            var order8 = new Order("ORD008", compositeCustomer, compositeStrategy);
            order8.AddItem(new OrderItem("Premium Item", 500m, 1));
            order8.AddItem(new OrderItem("Accessory", 200m, 1));
            order8.PrintOrder();

            // Display benefits
            DisplayBenefits();
        }

        private void DisplayBenefits()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                STRATEGY PATTERN BENEFITS                    ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ ✅ Each strategy is independent and testable              ║");
            Console.WriteLine("║ ✅ New discount types don't require modifying existing     ║");
            Console.WriteLine("║    code (Open/Closed Principle)                           ║");
            Console.WriteLine("║ ✅ Easy to switch strategies at runtime                   ║");
            Console.WriteLine("║ ✅ Can combine multiple strategies (Composite)            ║");
            Console.WriteLine("║ ✅ Order class has single responsibility                  ║");
            Console.WriteLine("║ ✅ Strategies can be reused across different contexts    ║");
            Console.WriteLine("║ ✅ Each strategy can be developed/tested independently    ║");
            Console.WriteLine("║ ✅ Easy to understand and maintain                        ║");
            Console.WriteLine("║ ✅ Changes to one strategy don't affect others            ║");
            Console.WriteLine("║ ✅ Follows SOLID principles (OCP, SRP, DIP)              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        }
    }
}
