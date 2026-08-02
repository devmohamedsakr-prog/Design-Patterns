using System;
using Cart.After.Templates;

namespace Cart.After.Implementations
{
    /// <summary>
    /// Online Cart Checkout: E-commerce checkout process
    /// ✅ SPECIAL: Orders over $1000 get 10% volume discount
    /// </summary>
    public class OnlineCartCheckout : CartCheckoutTemplate
    {
        protected override Customer LoadCustomerData(string customerId)
        {
            // Simulate loading online customer data
            return new Customer
            {
                CustomerId = customerId,
                Location = "Online",
                LoyaltyYears = 2,
                PaymentMethod = "CreditCard"
            };
        }

        protected override decimal ApplyLocationDiscount(ShoppingCart cart, Customer customer)
        {
            // Online: No location discount
            return 0m;
        }

        protected override decimal ApplyVolumeDiscount(ShoppingCart cart, Customer customer)
        {
            // ✅ ONLINE SPECIAL: Orders over $1000 get 10% discount
            if (cart.Total >= 1000m)
            {
                Console.WriteLine($"✓ Volume discount applied! Order ${cart.Total:F2} >= $1000");
                return cart.Total * 0.10m; // 10% discount
            }
            return 0m;
        }

        protected override decimal ApplyLoyaltyDiscount(ShoppingCart cart, Customer customer)
        {
            // 1% per loyalty year (online customers)
            return cart.Total * (customer.LoyaltyYears * 0.01m);
        }

        protected override decimal CalculateShippingHandling(ShoppingCart cart, Customer customer)
        {
            // Online: Free shipping on orders over $1000, otherwise $5
            if (cart.Total >= 1000m)
            {
                return 0m;
            }
            return 5m;
        }

        protected override void VerifyPaymentMethod(ShoppingCart cart, Customer customer)
        {
            if (string.IsNullOrEmpty(customer.PaymentMethod))
                throw new InvalidOperationException("Payment method required for online checkout");
            Console.WriteLine($"✓ Payment method verified: {customer.PaymentMethod}");
        }

        protected override void SendConfirmation(ShoppingCart cart, Customer customer, string receiptNumber)
        {
            Console.WriteLine($"✓ Online confirmation email sent to customer (Receipt: {receiptNumber})");
        }
    }
}
