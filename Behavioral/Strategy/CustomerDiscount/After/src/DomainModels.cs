using System;
using System.Collections.Generic;

namespace CustomerDiscount.After
{
    /// <summary>
    /// Domain models for Customer Discount system.
    /// SRP: Single Responsibility - Define data models only
    /// </summary>

    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public CustomerType Type { get; set; }
        public int YearsAsCustomer { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsBirthday { get; set; }

        public Customer(string id, string name, CustomerType type, int yearsAsCustomer = 0)
        {
            Id = id;
            Name = name;
            Type = type;
            YearsAsCustomer = yearsAsCustomer;
            JoinDate = DateTime.Now.AddYears(-yearsAsCustomer);
            IsBirthday = false;
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

    public class Order
    {
        public string OrderId { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public DateTime OrderDate { get; set; }
        public IDiscountStrategy DiscountStrategy { get; set; }

        public Order(string orderId, Customer customer, IDiscountStrategy discountStrategy)
        {
            OrderId = orderId;
            Customer = customer;
            OrderDate = DateTime.Now;
            DiscountStrategy = discountStrategy;
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

        // ✅ Strategy pattern: Discount logic delegated to strategy
        public decimal CalculateDiscount()
        {
            if (DiscountStrategy == null)
                return 0;

            var context = new DiscountContext(Customer, Items.Count, OrderDate);
            return DiscountStrategy.CalculateDiscount(GetSubtotal(), context);
        }

        public decimal GetTotal()
        {
            return Math.Round(GetSubtotal() - CalculateDiscount(), 2);
        }

        public void PrintOrder()
        {
            string strategyInfo = DiscountStrategy?.StrategyName ?? "No Discount";
            Console.WriteLine($"\n╔════════════════════════════════════╗");
            Console.WriteLine($"║ Order ID: {OrderId,-25}║");
            Console.WriteLine($"║ Customer: {Customer.Name,-24}║");
            Console.WriteLine($"║ Strategy: {strategyInfo,-24}║");
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
}
