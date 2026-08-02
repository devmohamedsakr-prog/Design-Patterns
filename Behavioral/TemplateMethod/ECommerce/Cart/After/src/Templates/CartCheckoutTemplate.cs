using System;
using System.Collections.Generic;

namespace Cart.After.Templates
{
    /// <summary>
    /// Cart Checkout Template Method: Defines skeleton of checkout process
    /// Subclasses implement Online vs InStore checkout variations
    /// </summary>
    public abstract class CartCheckoutTemplate
    {
        /// <summary>
        /// Template Method: Final checkout algorithm (don't override!)
        /// </summary>
        public CheckoutResult ProcessCheckout(ShoppingCart cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            try
            {
                // 1. Fixed: Validate Cart
                ValidateCart(cart);

                // 2. Variable: Load Customer Data
                Customer customer = LoadCustomerData(cart.CustomerId);

                // 3. Variable: Apply Location-Based Discount
                decimal locationDiscount = ApplyLocationDiscount(cart, customer);

                // 4. Variable: Apply Volume Discount (Online: 1000+ gets discount)
                decimal volumeDiscount = ApplyVolumeDiscount(cart, customer);

                // 5. Variable: Apply Loyalty Discount
                decimal loyaltyDiscount = ApplyLoyaltyDiscount(cart, customer);

                // 6. Fixed: Calculate Tax
                decimal tax = CalculateTax(cart);

                // 7. Variable: Calculate Shipping/Handling
                decimal shippingCost = CalculateShippingHandling(cart, customer);

                // 8. Variable: Verify Payment Method
                VerifyPaymentMethod(cart, customer);

                // 9. Fixed: Generate Receipt
                string receiptNumber = GenerateReceipt(cart, locationDiscount, volumeDiscount, loyaltyDiscount, tax, shippingCost);

                // 10. Variable: Send Confirmation
                SendConfirmation(cart, customer, receiptNumber);

                decimal totalDiscount = locationDiscount + volumeDiscount + loyaltyDiscount;
                decimal subtotal = cart.Total;
                decimal finalTotal = subtotal - totalDiscount + tax + shippingCost;

                return new CheckoutResult
                {
                    Success = true,
                    ReceiptNumber = receiptNumber,
                    Subtotal = subtotal,
                    LocationDiscount = locationDiscount,
                    VolumeDiscount = volumeDiscount,
                    LoyaltyDiscount = loyaltyDiscount,
                    TotalDiscount = totalDiscount,
                    Tax = tax,
                    ShippingCost = shippingCost,
                    FinalTotal = finalTotal
                };
            }
            catch (Exception ex)
            {
                return new CheckoutResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // ============================================================
        // FIXED STEPS (Template controls these)
        // ============================================================

        protected virtual void ValidateCart(ShoppingCart cart)
        {
            if (cart.Items.Count == 0)
                throw new ArgumentException("Cart is empty");
            if (string.IsNullOrEmpty(cart.CustomerId))
                throw new ArgumentException("Customer ID required");
            if (cart.Total <= 0)
                throw new ArgumentException("Invalid cart total");
        }

        protected virtual decimal CalculateTax(ShoppingCart cart)
        {
            // Standard tax calculation (varies by region - template can override)
            return cart.Total * 0.08m;
        }

        protected virtual string GenerateReceipt(ShoppingCart cart, decimal locationDisc, decimal volumeDisc, 
            decimal loyaltyDisc, decimal tax, decimal shipping)
        {
            return $"RECEIPT-{cart.CustomerId}-{DateTime.Now:yyyyMMddHHmmss}";
        }

        // ============================================================
        // VARIABLE STEPS (Subclasses implement these)
        // ============================================================

        protected abstract Customer LoadCustomerData(string customerId);
        protected abstract decimal ApplyLocationDiscount(ShoppingCart cart, Customer customer);
        protected abstract decimal ApplyVolumeDiscount(ShoppingCart cart, Customer customer);
        protected abstract decimal ApplyLoyaltyDiscount(ShoppingCart cart, Customer customer);
        protected abstract decimal CalculateShippingHandling(ShoppingCart cart, Customer customer);
        protected abstract void VerifyPaymentMethod(ShoppingCart cart, Customer customer);
        protected abstract void SendConfirmation(ShoppingCart cart, Customer customer, string receiptNumber);
    }

    /// <summary>Shopping cart model</summary>
    public class ShoppingCart
    {
        public string CustomerId { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public decimal Total => GetTotal();

        private decimal GetTotal()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.Price * item.Quantity;
            }
            return total;
        }
    }

    /// <summary>Cart item</summary>
    public class CartItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    /// <summary>Customer data</summary>
    public class Customer
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int LoyaltyYears { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
    }

    /// <summary>Checkout result</summary>
    public class CheckoutResult
    {
        public bool Success { get; set; }
        public string ReceiptNumber { get; set; }
        public decimal Subtotal { get; set; }
        public decimal LocationDiscount { get; set; }
        public decimal VolumeDiscount { get; set; }
        public decimal LoyaltyDiscount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal FinalTotal { get; set; }
        public string ErrorMessage { get; set; }
    }
}
