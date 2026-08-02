using OrderSystem.After.Models;

namespace OrderSystem.After.Decorators
{
    /// <summary>
    /// OrderDecorator: Base decorator class for order pricing features
    /// SRP: Provides common decorator interface for wrapping orders
    /// </summary>
    public abstract class OrderDecorator : Order
    {
        protected Order _wrappedOrder;

        public OrderDecorator(Order order) : base(order.OrderId, order.BasePrice)
        {
            _wrappedOrder = order;
        }

        public override decimal GetTotal() => _wrappedOrder.GetTotal();
    }
}
