using System;
using OrderValidation.After.Models;

namespace OrderValidation.After.Handlers
{
    /// <summary>
    /// FraudHandler: Detects suspicious order patterns
    /// SRP: Only responsible for fraud detection
    /// </summary>
    public class FraudHandler : ValidationHandler
    {
        private decimal _highAmountThreshold = 10000m;
        private int _highQuantityThreshold = 500;

        public FraudHandler(decimal highAmountThreshold = 10000m, int highQuantityThreshold = 500)
        {
            _highAmountThreshold = highAmountThreshold;
            _highQuantityThreshold = highQuantityThreshold;
        }

        public override ValidationResult Handle(Order order)
        {
            Console.WriteLine($"  [Fraud] Checking for suspicious activity...");

            // Check amount
            if (order.Amount > _highAmountThreshold)
                return new ValidationResult(false, 
                    $"Order amount ${order.Amount:F2} exceeds fraud threshold", 
                    nameof(FraudHandler));

            // Check quantity
            if (order.Quantity > _highQuantityThreshold)
                return new ValidationResult(false, 
                    $"Quantity {order.Quantity} exceeds fraud threshold", 
                    nameof(FraudHandler));

            // Check for unusual patterns
            if (order.Amount > 1000 && order.Quantity > 100)
                return new ValidationResult(false, 
                    "High-value bulk order detected - requires additional verification", 
                    nameof(FraudHandler));

            Console.WriteLine($"  ✓ Fraud check passed (no suspicious patterns detected)");
            return PassToNext(order);
        }
    }
}
