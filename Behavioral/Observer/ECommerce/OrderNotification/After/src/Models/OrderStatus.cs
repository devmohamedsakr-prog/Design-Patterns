namespace OrderNotification.After.Models
{
    /// <summary>
    /// OrderStatus: Enumeration of order states
    /// SRP: Defines all possible order statuses
    /// </summary>
    public enum OrderStatus
    {
        Placed,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
