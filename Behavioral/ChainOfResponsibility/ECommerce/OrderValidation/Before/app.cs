using System;

namespace OrderValidation.Before
{
    // BEFORE: Anti-pattern - Monolithic validation
    // All validation logic in one method - hard to extend and maintain

    public class Order
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
        public string CustomerName { get; set; }
        public int CustomerAge { get; set; }
    }

    public class OrderProcessor
    {
        public void ValidateOrder(Order order)
        {
            Console.WriteLine($"\nValidating order {order.OrderId}...");

            // PROBLEM 1: All validation logic in one method
            // PROBLEM 2: Hard-coded validation order
            // PROBLEM 3: Hard to add, remove, or reorder validators
            // PROBLEM 4: Hard to test individual validators

            // Inventory check
            if (order.Quantity <= 0 || order.Quantity > 1000)
            {
                throw new InvalidOperationException("Invalid quantity");
            }
            Console.WriteLine("  ✓ Inventory check passed");

            // Payment validation
            if (string.IsNullOrEmpty(order.PaymentMethod) || 
                (order.PaymentMethod != "Credit Card" && 
                 order.PaymentMethod != "PayPal" && 
                 order.PaymentMethod != "Bank Transfer"))
            {
                throw new InvalidOperationException("Invalid payment method");
            }
            
            if (order.Amount <= 0)
            {
                throw new InvalidOperationException("Invalid amount");
            }
            Console.WriteLine("  ✓ Payment check passed");

            // Fraud detection
            if (order.Amount > 10000)
            {
                throw new InvalidOperationException("Order amount exceeds limit - potential fraud");
            }
            
            if (order.Quantity > 500)
            {
                throw new InvalidOperationException("Quantity too high - potential fraud");
            }
            Console.WriteLine("  ✓ Fraud check passed");

            // Shipping validation
            if (string.IsNullOrEmpty(order.ShippingAddress))
            {
                throw new InvalidOperationException("Shipping address required");
            }
            
            if (order.ShippingAddress.Length < 10)
            {
                throw new InvalidOperationException("Invalid shipping address");
            }
            Console.WriteLine("  ✓ Shipping check passed");

            Console.WriteLine($"✓ Order {order.OrderId} validation complete!");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Chain of Responsibility: BEFORE (Anti-pattern)");
            Console.WriteLine("  Monolithic Validation Pipeline");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var processor = new OrderProcessor();

            // Test 1: Valid order
            Console.WriteLine("--- Test 1: Valid Order ---");
            try
            {
                var order1 = new Order
                {
                    OrderId = "ORD001",
                    Amount = 500,
                    Quantity = 10,
                    PaymentMethod = "Credit Card",
                    ShippingAddress = "123 Main Street, New York, NY 10001"
                };
                processor.ValidateOrder(order1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Validation failed: {ex.Message}");
            }

            // Test 2: Invalid quantity
            Console.WriteLine("\n--- Test 2: Invalid Quantity ---");
            try
            {
                var order2 = new Order
                {
                    OrderId = "ORD002",
                    Amount = 500,
                    Quantity = 0,
                    PaymentMethod = "Credit Card",
                    ShippingAddress = "456 Oak Avenue, Los Angeles, CA 90001"
                };
                processor.ValidateOrder(order2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Validation failed: {ex.Message}");
            }

            // Test 3: Invalid payment method
            Console.WriteLine("\n--- Test 3: Invalid Payment Method ---");
            try
            {
                var order3 = new Order
                {
                    OrderId = "ORD003",
                    Amount = 500,
                    Quantity = 5,
                    PaymentMethod = "Bitcoin",
                    ShippingAddress = "789 Pine Road, Chicago, IL 60601"
                };
                processor.ValidateOrder(order3);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Validation failed: {ex.Message}");
            }

            // Test 4: High amount (fraud detection)
            Console.WriteLine("\n--- Test 4: High Amount (Fraud Detection) ---");
            try
            {
                var order4 = new Order
                {
                    OrderId = "ORD004",
                    Amount = 50000,
                    Quantity = 5,
                    PaymentMethod = "Credit Card",
                    ShippingAddress = "999 Elm Street, Houston, TX 77001"
                };
                processor.ValidateOrder(order4);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Validation failed: {ex.Message}");
            }

            // Test 5: Invalid shipping address
            Console.WriteLine("\n--- Test 5: Invalid Shipping Address ---");
            try
            {
                var order5 = new Order
                {
                    OrderId = "ORD005",
                    Amount = 500,
                    Quantity = 5,
                    PaymentMethod = "Credit Card",
                    ShippingAddress = "123"
                };
                processor.ValidateOrder(order5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Validation failed: {ex.Message}");
            }

            // Show the problem
            Console.WriteLine("\n════════════════════════════════════════════════════════════════");
            Console.WriteLine("  THE PROBLEMS WITH THIS APPROACH");
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("✗ All validation logic in ONE method (ValidateOrder)");
            Console.WriteLine("✗ Hard-coded validation order - can't reorder validators");
            Console.WriteLine("✗ Adding new validator? Must edit this method");
            Console.WriteLine("✗ Want to skip a validator? Must add nested if blocks");
            Console.WriteLine("✗ Can't reuse validators in other contexts");
            Console.WriteLine("✗ Hard to test individual validators");
            Console.WriteLine("✗ Tight coupling between processor and all validators");
            Console.WriteLine("✗ Validation logic duplicated across different orders");
            Console.WriteLine();
            Console.WriteLine("SOLUTION: Use Chain of Responsibility Pattern!");
            Console.WriteLine("- Each validator is independent handler");
            Console.WriteLine("- Chain them together dynamically");
            Console.WriteLine("- Easy to add, remove, or reorder handlers");
            Console.WriteLine("- Test each handler independently");
            Console.WriteLine("- Reuse handlers in different chains");
        }
    }
}
