using System;
using OrderSystem.After.Models;

namespace OrderSystem.After.Decorators
{
    /// <summary>
    /// InsuranceDecorator: Adds order protection insurance
    /// SRP: Only responsible for calculating insurance fee
    /// </summary>
    public class InsuranceDecorator : OrderDecorator
    {
        private decimal _insuranceRate;

        public InsuranceDecorator(Order order, decimal insuranceRate) : base(order)
        {
            if (insuranceRate < 0 || insuranceRate > 1)
                throw new ArgumentException("Insurance rate must be between 0 and 1");
            
            _insuranceRate = insuranceRate;
        }

        public override decimal GetTotal()
        {
            decimal baseTotal = _wrappedOrder.GetTotal();
            decimal insurance = baseTotal * _insuranceRate;
            return baseTotal + insurance;
        }

        public override string ToString()
        {
            decimal baseTotal = _wrappedOrder.GetTotal();
            decimal insurance = baseTotal * _insuranceRate;
            return $"{_wrappedOrder} → Insurance {_insuranceRate * 100:F0}% (+${insurance:F2}) = ${GetTotal():F2}";
        }
    }
}
