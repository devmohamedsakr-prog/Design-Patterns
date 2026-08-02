namespace OrderNotification.After.Models
{
    /// <summary>
    /// Order: Subject that notifies observers about state changes
    /// SRP: Only manages order data and observer notifications
    /// </summary>
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public OrderStatus Status { get; private set; }

        public Order(string orderId, string customerName, string customerEmail, 
            string customerPhone, decimal amount, string customerId = "")
        {
            OrderId = orderId;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            CustomerPhone = customerPhone;
            Amount = amount;
            CustomerId = customerId;
            Status = OrderStatus.Placed;
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }

        public override string ToString() =>
            $"Order {OrderId} | Customer: {CustomerName} | Amount: ${Amount:F2} | Status: {Status}";
    }
}
