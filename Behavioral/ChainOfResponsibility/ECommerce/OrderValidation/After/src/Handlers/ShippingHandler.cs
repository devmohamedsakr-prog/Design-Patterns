using System;
using OrderValidation.After.Models;

namespace OrderValidation.After.Handlers
{
    /// <summary>
    /// ShippingHandler: Validates shipping address
    /// SRP: Only responsible for shipping validation
    /// </summary>
    public class ShippingHandler : ValidationHandler
    {
        private int _minAddressLength = 10;
        private string[] _blacklistedRegions = { "UNKNOWN", "TEST" };

        public ShippingHandler(int minAddressLength = 10)
        {
            _minAddressLength = minAddressLength;
        }

        public override ValidationResult Handle(Order order)
        {
            Console.WriteLine($"  [Shipping] Validating address...");

            if (string.IsNullOrEmpty(order.ShippingAddress))
                return new ValidationResult(false, "Shipping address required", nameof(ShippingHandler));

            if (order.ShippingAddress.Length < _minAddressLength)
                return new ValidationResult(false, 
                    $"Shipping address too short (minimum {_minAddressLength} characters)", 
                    nameof(ShippingHandler));

            // Check for blacklisted regions
            foreach (var region in _blacklistedRegions)
            {
                if (order.ShippingAddress.ToUpper().Contains(region))
                    return new ValidationResult(false, 
                        $"Cannot ship to {region} region", 
                        nameof(ShippingHandler));
            }

            Console.WriteLine($"  ✓ Shipping check passed (address valid)");
            return PassToNext(order);
        }
    }
}
