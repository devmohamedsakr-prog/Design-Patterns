using System;
using OrderSystem.After.Models;

namespace OrderSystem.After.Decorators
{
    /// <summary>
    /// TaxDecorator: Applies tax rate to order total
    /// SRP: Only responsible for tax calculation
    /// </summary>
    public class TaxDecorator : OrderDecorator
    {
        private decimal _taxRate;

        public TaxDecorator(Order order, decimal taxRate) : base(order)
        {
            if (taxRate < 0 || taxRate > 1)
                throw new ArgumentException("Tax rate must be between 0 and 1");
            
            _taxRate = taxRate;
        }

        public override decimal GetTotal()
        {
            decimal baseTotal = _wrappedOrder.GetTotal();
            decimal tax = baseTotal * _taxRate;
            return baseTotal + tax;
        }

        public override string ToString()
        {
            decimal baseTotal = _wrappedOrder.GetTotal();
            decimal tax = baseTotal * _taxRate;
            return $"{_wrappedOrder} → Tax {_taxRate * 100:F0}% (+${tax:F2}) = ${GetTotal():F2}";
        }
    }
}
