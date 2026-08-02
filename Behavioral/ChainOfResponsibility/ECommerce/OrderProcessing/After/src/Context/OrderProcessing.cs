using System;

namespace OrderProcessing.After.Context
{
    public class Order
    {
        public string OrderId { get; set; } = "";
        public string[] Items { get; set; } = new string[0];
        public decimal Amount { get; set; }
        public bool IsValid { get; set; } = false;
        public bool InventoryChecked { get; set; } = false;
        public bool PaymentProcessed { get; set; } = false;
        public string Status { get; set; } = "Created";
    }

    public abstract class OrderProcessor
    {
        protected OrderProcessor _nextProcessor;

        public void SetNext(OrderProcessor next) => _nextProcessor = next;

        public virtual void Process(Order order)
        {
            if (Execute(order) && _nextProcessor != null)
                _nextProcessor.Process(order);
            else if (!Execute(order))
                order.Status = "Failed";
        }

        protected abstract bool Execute(Order order);
    }

    public class OrderValidator : OrderProcessor
    {
        protected override bool Execute(Order order)
        {
            Console.WriteLine("🔍 OrderValidator: Validating order details");
            
            if (string.IsNullOrEmpty(order.OrderId) || order.Items.Length == 0)
            {
                Console.WriteLine("❌ Order validation failed");
                return false;
            }

            order.IsValid = true;
            Console.WriteLine("✓ Order validated");
            order.Status = "Validated";
            return true;
        }
    }

    public class InventoryChecker : OrderProcessor
    {
        protected override bool Execute(Order order)
        {
            Console.WriteLine("📦 InventoryChecker: Checking stock availability");
            
            if (order.Items.Length > 5)
            {
                Console.WriteLine("❌ Insufficient inventory");
                return false;
            }

            order.InventoryChecked = true;
            Console.WriteLine("✓ Inventory available");
            order.Status = "Inventory Checked";
            return true;
        }
    }

    public class PaymentProcessor : OrderProcessor
    {
        protected override bool Execute(Order order)
        {
            Console.WriteLine("💳 PaymentProcessor: Processing payment");
            
            if (order.Amount <= 0)
            {
                Console.WriteLine("❌ Payment processing failed");
                return false;
            }

            order.PaymentProcessed = true;
            Console.WriteLine($"✓ Payment processed: ${order.Amount}");
            order.Status = "Payment Processed";
            return true;
        }
    }

    public class ShippingHandler : OrderProcessor
    {
        protected override bool Execute(Order order)
        {
            Console.WriteLine("🚚 ShippingHandler: Preparing shipment");
            Console.WriteLine("✓ Order ready for shipping");
            order.Status = "Ready for Shipping";
            return true;
        }
    }

    public class OrderProcessingChain
    {
        private OrderProcessor _firstProcessor;

        public OrderProcessingChain()
        {
            var validator = new OrderValidator();
            var inventory = new InventoryChecker();
            var payment = new PaymentProcessor();
            var shipping = new ShippingHandler();

            validator.SetNext(inventory);
            inventory.SetNext(payment);
            payment.SetNext(shipping);

            _firstProcessor = validator;
        }

        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"\n📋 Processing order: {order.OrderId}");
            _firstProcessor.Process(order);
        }
    }
}
