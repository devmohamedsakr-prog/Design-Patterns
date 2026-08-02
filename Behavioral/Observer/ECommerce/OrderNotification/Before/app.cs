using System;

namespace OrderNotification.Before
{
    // BEFORE: Anti-pattern - Tight coupling to notifications

    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal Amount { get; set; }
        public OrderStatus Status { get; set; }

        public Order(string orderId, string customerName, string customerEmail, decimal amount)
        {
            OrderId = orderId;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            Amount = amount;
            Status = OrderStatus.Placed;
        }
    }

    public enum OrderStatus
    {
        Placed,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    // Anti-pattern: OrderProcessor handles all notifications
    public class OrderProcessor
    {
        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"\nProcessing order {order.OrderId}...");

            order.Status = OrderStatus.Processing;

            // PROBLEM 1: Hard-coded notification calls
            // PROBLEM 2: Tight coupling to all services
            // PROBLEM 3: If any service fails, order process fails
            // PROBLEM 4: Hard to add/remove notifications
            // PROBLEM 5: Code duplication across methods

            // Send email (tightly coupled)
            Console.WriteLine($"  [Tightly Coupled] Sending email to {order.CustomerEmail}");
            Console.WriteLine($"    Subject: Order {order.OrderId} confirmed");
            Console.WriteLine($"    Body: Thank you for your order");

            // Send SMS (tightly coupled)
            Console.WriteLine($"  [Tightly Coupled] Sending SMS to customer");
            Console.WriteLine($"    Message: Order {order.OrderId} confirmed. Amount: ${order.Amount}");

            // Send push notification (tightly coupled)
            Console.WriteLine($"  [Tightly Coupled] Sending push notification");
            Console.WriteLine($"    Title: Order Confirmed");
            Console.WriteLine($"    Body: Order {order.OrderId} is being processed");

            // Update inventory (tightly coupled)
            Console.WriteLine($"  [Tightly Coupled] Updating inventory system");
            Console.WriteLine($"    Reserved items for order {order.OrderId}");

            // Track analytics (tightly coupled)
            Console.WriteLine($"  [Tightly Coupled] Tracking analytics");
            Console.WriteLine($"    Event: order_placed, Amount: ${order.Amount}");

            Console.WriteLine($"✓ Order {order.OrderId} processed");
        }

        public void ShipOrder(Order order)
        {
            Console.WriteLine($"\nShipping order {order.OrderId}...");

            order.Status = OrderStatus.Shipped;

            // CODE DUPLICATION: Same notifications again!
            Console.WriteLine($"  [Tightly Coupled] Sending email to {order.CustomerEmail}");
            Console.WriteLine($"    Subject: Order {order.OrderId} shipped");
            Console.WriteLine($"    Body: Your order is on the way");

            Console.WriteLine($"  [Tightly Coupled] Sending SMS to customer");
            Console.WriteLine($"    Message: Order {order.OrderId} shipped");

            Console.WriteLine($"  [Tightly Coupled] Sending push notification");
            Console.WriteLine($"    Title: Order Shipped");

            Console.WriteLine($"  [Tightly Coupled] Updating inventory system");
            Console.WriteLine($"    Updated inventory for shipped order");

            Console.WriteLine($"✓ Order {order.OrderId} shipped");
        }

        public void CancelOrder(Order order)
        {
            Console.WriteLine($"\nCancelling order {order.OrderId}...");

            order.Status = OrderStatus.Cancelled;

            // MORE DUPLICATION!
            Console.WriteLine($"  [Tightly Coupled] Sending email to {order.CustomerEmail}");
            Console.WriteLine($"    Subject: Order {order.OrderId} cancelled");

            Console.WriteLine($"  [Tightly Coupled] Sending SMS to customer");
            Console.WriteLine($"    Message: Order cancelled");

            Console.WriteLine($"  [Tightly Coupled] Updating inventory system");
            Console.WriteLine($"    Released items from cancelled order");

            Console.WriteLine($"✓ Order {order.OrderId} cancelled");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Observer Pattern: BEFORE (Anti-pattern)");
            Console.WriteLine("  Tightly Coupled Order Notification System");
            Console.WriteLine("════════════════════════════════════════════════════════════════");

            var processor = new OrderProcessor();

            // Test 1: Process order
            Console.WriteLine("\n--- Test 1: Process Order ---");
            var order1 = new Order("ORD001", "Alice Smith", "alice@example.com", 150);
            processor.ProcessOrder(order1);

            // Test 2: Ship order
            Console.WriteLine("\n--- Test 2: Ship Order ---");
            processor.ShipOrder(order1);

            // Test 3: Cancel order
            Console.WriteLine("\n--- Test 3: Cancel Order ---");
            var order2 = new Order("ORD002", "Bob Jones", "bob@example.com", 200);
            processor.ProcessOrder(order2);
            processor.CancelOrder(order2);

            // Show the problem
            Console.WriteLine("\n════════════════════════════════════════════════════════════════");
            Console.WriteLine("  THE PROBLEMS WITH THIS APPROACH");
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("✗ All notifications tightly coupled to Order class");
            Console.WriteLine("✗ Order processor knows about ALL notification types");
            Console.WriteLine("✗ Adding new notification? Must modify Order processor");
            Console.WriteLine("✗ Massive code duplication across methods");
            Console.WriteLine("✗ If SMS service fails, entire order process fails");
            Console.WriteLine("✗ Hard to test notifications independently");
            Console.WriteLine("✗ Hard to disable a notification without removing code");
            Console.WriteLine("✗ Single Responsibility Principle violated");
            Console.WriteLine();
            Console.WriteLine("SOLUTION: Use Observer Pattern!");
            Console.WriteLine("- Order doesn't know about notifications");
            Console.WriteLine("- Observers subscribe to order events");
            Console.WriteLine("- Easy to add/remove observers");
            Console.WriteLine("- Each observer has single responsibility");
            Console.WriteLine("- Loose coupling between order and notifications");
        }
    }
}
