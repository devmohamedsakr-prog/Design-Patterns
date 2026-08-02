using System;
using OrderSystem.After.Models;

namespace OrderSystem.After.Decorators
{
    /// <summary>
    /// DiscountDecorator: Applies percentage-based discount to order
    /// SRP: Only responsible for discount calculation
    /// </summary>
    public class DiscountDecorator : OrderDecorator
    {
        private decimal _discountPercent;

        public DiscountDecorator(Order order, decimal discountPercent) : base(order)
        {
            if (discountPercent < 0 || discountPercent > 1)
                throw new ArgumentException("Discount percent must be between 0 and 1");
            
            _discountPercent = discountPercent;
        }

        public override decimal GetTotal()
        {
            decimal baseTotal = _wrappedOrder.GetTotal();
            decimal discount = baseTotal * _discountPercent;
            return baseTotal - discount;
        }

        public override string ToString()
        {
            decimal baseTotal = _wrappedOrder.GetTotal();
            decimal discount = baseTotal * _discountPercent;
            return $"{_wrappedOrder} → Discount {_discountPercent * 100:F0}% (-${discount:F2}) = ${GetTotal():F2}";
        }
    }
}
