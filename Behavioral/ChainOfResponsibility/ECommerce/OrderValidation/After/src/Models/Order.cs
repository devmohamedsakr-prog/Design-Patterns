namespace OrderValidation.After.Models
{
    /// <summary>
    /// Order: Request object passed through validation chain
    /// SRP: Only stores order data
    /// </summary>
    public class Order
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
        public string CustomerName { get; set; }
        public int CustomerAge { get; set; }
        public bool IsPremiumCustomer { get; set; }

        public Order(string orderId, decimal amount, int quantity, 
            string paymentMethod, string shippingAddress, string customerName = "Guest")
        {
            OrderId = orderId;
            Amount = amount;
            Quantity = quantity;
            PaymentMethod = paymentMethod;
            ShippingAddress = shippingAddress;
            CustomerName = customerName;
            IsPremiumCustomer = false;
        }

        public override string ToString() =>
            $"Order {OrderId} | Amount: ${Amount:F2} | Qty: {Quantity} | Customer: {CustomerName}";
    }
}
