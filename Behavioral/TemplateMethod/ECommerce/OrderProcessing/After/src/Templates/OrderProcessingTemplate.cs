using System;

namespace OrderProcessing.After.Templates
{
    /// <summary>
    /// Template Method: Defines skeleton of order processing algorithm
    /// Subclasses implement variable steps (Premium, Standard, Budget, etc.)
    /// </summary>
    public abstract class OrderProcessingTemplate
    {
        /// <summary>
        /// Template Method: Final algorithm (don't override!)
        /// Defines fixed order of operations
        /// </summary>
        public OrderResult ProcessOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            try
            {
                // 1. Fixed: Validate Order
                ValidateOrder(order);

                // 2. Variable: Calculate Discount
                decimal discount = CalculateDiscount(order);

                // 3. Variable: Calculate Tax  
                decimal tax = CalculateTax(order);

                // 4. Variable: Calculate Shipping
                decimal shipping = CalculateShipping(order);

                // 5. Fixed: Verify Payment
                VerifyPayment(order);

                // 6. Variable: Process Payment
                bool paymentProcessed = ProcessPayment(order);

                if (!paymentProcessed)
                    throw new InvalidOperationException("Payment processing failed");

                // 7. Fixed: Generate Invoice
                string invoiceNumber = GenerateInvoice(order, discount, tax, shipping);

                // 8. Fixed: Send Confirmation
                SendConfirmation(order, invoiceNumber);

                return new OrderResult
                {
                    Success = true,
                    OrderId = order.OrderId,
                    InvoiceNumber = invoiceNumber,
                    Discount = discount,
                    Tax = tax,
                    Shipping = shipping,
                    FinalTotal = (order.Subtotal - discount + tax + shipping)
                };
            }
            catch (Exception ex)
            {
                return new OrderResult
                {
                    Success = false,
                    OrderId = order.OrderId,
                    ErrorMessage = ex.Message
                };
            }
        }

        // ============================================================
        // FIXED STEPS (Template controls these)
        // ============================================================

        protected virtual void ValidateOrder(Order order)
        {
            if (order.Subtotal <= 0)
                throw new ArgumentException("Order subtotal must be positive");
            if (string.IsNullOrEmpty(order.CustomerId))
                throw new ArgumentException("Customer ID required");
        }

        protected virtual void VerifyPayment(Order order)
        {
            if (string.IsNullOrEmpty(order.PaymentMethod))
                throw new ArgumentException("Payment method required");
        }

        protected virtual string GenerateInvoice(Order order, decimal discount, decimal tax, decimal shipping)
        {
            return $"INV-{order.OrderId}-{DateTime.Now:yyyyMMdd}";
        }

        protected virtual void SendConfirmation(Order order, string invoiceNumber)
        {
            Console.WriteLine($"✓ Confirmation sent to {order.CustomerId} (Invoice: {invoiceNumber})");
        }

        // ============================================================
        // VARIABLE STEPS (Subclasses implement these)
        // ============================================================

        protected abstract decimal CalculateDiscount(Order order);
        protected abstract decimal CalculateTax(Order order);
        protected abstract decimal CalculateShipping(Order order);
        protected abstract bool ProcessPayment(Order order);
    }

    /// <summary>Order data model</summary>
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public decimal Subtotal { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int ItemCount { get; set; }
    }

    /// <summary>Order processing result</summary>
    public class OrderResult
    {
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal FinalTotal { get; set; }
        public string ErrorMessage { get; set; }
    }
}
