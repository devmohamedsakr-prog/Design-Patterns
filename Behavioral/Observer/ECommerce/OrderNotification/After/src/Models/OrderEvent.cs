using System;

namespace OrderNotification.After.Models
{
    /// <summary>
    /// OrderEvent: Data passed to observers when order changes
    /// SRP: Encapsulates order event information
    /// </summary>
    public class OrderEvent
    {
        public string OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime EventTime { get; set; }
        public string Message { get; set; }

        public OrderEvent(Order order, string message = "")
        {
            OrderId = order.OrderId;
            Status = order.Status;
            CustomerName = order.CustomerName;
            CustomerEmail = order.CustomerEmail;
            CustomerPhone = order.CustomerPhone;
            CustomerId = order.CustomerId;
            Amount = order.Amount;
            EventTime = DateTime.Now;
            Message = message;
        }

        public override string ToString() =>
            $"Event: {Status} | Order: {OrderId} | Customer: {CustomerName} | Time: {EventTime:yyyy-MM-dd HH:mm:ss}";
    }
}
