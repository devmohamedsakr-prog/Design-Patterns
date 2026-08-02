using System;
using OrderValidation.After.Models;

namespace OrderValidation.After.Handlers
{
    /// <summary>
    /// InventoryHandler: Validates order quantity against inventory
    /// SRP: Only responsible for inventory validation
    /// </summary>
    public class InventoryHandler : ValidationHandler
    {
        private int _maxQuantity = 1000;
        private int _minQuantity = 1;

        public InventoryHandler(int maxQuantity = 1000, int minQuantity = 1)
        {
            _maxQuantity = maxQuantity;
            _minQuantity = minQuantity;
        }

        public override ValidationResult Handle(Order order)
        {
            Console.WriteLine($"  [Inventory] Checking quantity: {order.Quantity}...");

            if (order.Quantity < _minQuantity)
                return new ValidationResult(false, $"Quantity must be at least {_minQuantity}", nameof(InventoryHandler));

            if (order.Quantity > _maxQuantity)
                return new ValidationResult(false, $"Quantity exceeds maximum of {_maxQuantity}", nameof(InventoryHandler));

            Console.WriteLine($"  ✓ Inventory check passed (quantity {order.Quantity} valid)");
            return PassToNext(order);
        }
    }
}
