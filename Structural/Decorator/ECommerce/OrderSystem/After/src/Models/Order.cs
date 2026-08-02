namespace OrderSystem.After.Models
{
    /// <summary>
    /// Order: Core order entity with base price
    /// SRP: Only responsible for storing order identity and base price
    /// </summary>
    public class Order
    {
        public string OrderId { get; set; }
        public decimal BasePrice { get; set; }

        public Order(string orderId, decimal basePrice)
        {
            OrderId = orderId;
            BasePrice = basePrice;
        }

        public virtual decimal GetTotal() => BasePrice;

        public override string ToString() =>
            $"Order {OrderId}: ${GetTotal():F2}";
    }
}
