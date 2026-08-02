using System;
using OrderValidation.After.Models;

namespace OrderValidation.After.Handlers
{
    /// <summary>
    /// PaymentHandler: Validates payment method and amount
    /// SRP: Only responsible for payment validation
    /// </summary>
    public class PaymentHandler : ValidationHandler
    {
        private readonly string[] _validMethods = { "Credit Card", "PayPal", "Bank Transfer", "Apple Pay", "Google Pay" };
        private decimal _minAmount = 0.01m;
        private decimal _maxAmount = 999999m;

        public PaymentHandler(decimal minAmount = 0.01m, decimal maxAmount = 999999m)
        {
            _minAmount = minAmount;
            _maxAmount = maxAmount;
        }

        public override ValidationResult Handle(Order order)
        {
            Console.WriteLine($"  [Payment] Checking payment method: {order.PaymentMethod}...");

            if (string.IsNullOrEmpty(order.PaymentMethod))
                return new ValidationResult(false, "Payment method required", nameof(PaymentHandler));

            if (!IsValidPaymentMethod(order.PaymentMethod))
                return new ValidationResult(false, $"Invalid payment method: {order.PaymentMethod}", nameof(PaymentHandler));

            if (order.Amount < _minAmount)
                return new ValidationResult(false, $"Amount must be at least ${_minAmount:F2}", nameof(PaymentHandler));

            if (order.Amount > _maxAmount)
                return new ValidationResult(false, $"Amount exceeds maximum of ${_maxAmount:F2}", nameof(PaymentHandler));

            Console.WriteLine($"  ✓ Payment check passed (method: {order.PaymentMethod}, amount: ${order.Amount:F2})");
            return PassToNext(order);
        }

        private bool IsValidPaymentMethod(string method)
        {
            foreach (var validMethod in _validMethods)
            {
                if (validMethod.Equals(method, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
