using System;
using OrderSystem.After.Models;

namespace OrderSystem.After.Decorators
{
    /// <summary>
    /// ShippingDecorator: Adds shipping cost to order
    /// SRP: Only responsible for adding shipping fee
    /// </summary>
    public class ShippingDecorator : OrderDecorator
    {
        private decimal _shippingCost;

        public ShippingDecorator(Order order, decimal shippingCost) : base(order)
        {
            if (shippingCost < 0)
                throw new ArgumentException("Shipping cost cannot be negative");
            
            _shippingCost = shippingCost;
        }

        public override decimal GetTotal()
        {
            return _wrappedOrder.GetTotal() + _shippingCost;
        }

        public override string ToString()
        {
            return $"{_wrappedOrder} → Shipping (+${_shippingCost:F2}) = ${GetTotal():F2}";
        }
    }
}
