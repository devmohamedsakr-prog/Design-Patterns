using System;
using Cart.After.Templates;

namespace Cart.After.Implementations
{
    /// <summary>
    /// InStore Cart Checkout: Physical store checkout process
    /// No volume discount (different business model)
    /// </summary>
    public class InStoreCartCheckout : CartCheckoutTemplate
    {
        protected override Customer LoadCustomerData(string customerId)
        {
            // Simulate loading in-store customer data
            return new Customer
            {
                CustomerId = customerId,
                Location = "InStore",
                LoyaltyYears = 1,
                PaymentMethod = "Cash"
            };
        }

        protected override decimal ApplyLocationDiscount(ShoppingCart cart, Customer customer)
        {
            // InStore: 2% in-store only discount
            return cart.Total * 0.02m;
        }

        protected override decimal ApplyVolumeDiscount(ShoppingCart cart, Customer customer)
        {
            // InStore: No volume discount (different promotion strategy)
            return 0m;
        }

        protected override decimal ApplyLoyaltyDiscount(ShoppingCart cart, Customer customer)
        {
            // 0.5% per loyalty year (in-store customers)
            return cart.Total * (customer.LoyaltyYears * 0.005m);
        }

        protected override decimal CalculateShippingHandling(ShoppingCart cart, Customer customer)
        {
            // InStore: No shipping cost (pickup)
            return 0m;
        }

        protected override void VerifyPaymentMethod(ShoppingCart cart, Customer customer)
        {
            // InStore: Accept cash, card, check
            Console.WriteLine($"✓ Payment method verified: {customer.PaymentMethod}");
        }

        protected override void SendConfirmation(ShoppingCart cart, Customer customer, string receiptNumber)
        {
            Console.WriteLine($"✓ Receipt printed (Receipt: {receiptNumber})");
        }
    }
}
