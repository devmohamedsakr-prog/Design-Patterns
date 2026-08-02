using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductFactory
{
    /// <summary>
    /// Processes shopping cart using factory-created products
    /// ✅ Uses Factory to get correct product types
    /// ✅ No hard-coded product type logic
    /// </summary>
    public class CartProcessor
    {
        /// <summary>
        /// Calculate cart total with product-specific rules
        /// </summary>
        public CartSummary ProcessCart(List<CartItem> items, string customerLocation)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("Cart must contain at least one item");

            var summary = new CartSummary();
            decimal totalBeforeTax = 0;
            decimal totalShipping = 0;
            decimal totalTax = 0;

            foreach (var item in items)
            {
                // ✅ Factory creates appropriate product type
                var product = ProductFactoryClass.CreateByType(item.ProductType, item.Properties);

                var itemSubtotal = product.Price * item.Quantity;
                var itemShipping = product.CalculateShippingCost(item.Weight) * item.Quantity;
                var itemTax = product.CalculateTax(customerLocation) * item.Quantity;

                totalBeforeTax += itemSubtotal;
                totalShipping += itemShipping;
                totalTax += itemTax;

                summary.Items.Add(new CartItemSummary
                {
                    ProductName = product.Name,
                    ProductType = item.ProductType,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = itemSubtotal,
                    Shipping = itemShipping,
                    Tax = itemTax,
                    Total = itemSubtotal + itemShipping + itemTax,
                    DeliveryEstimate = product.GetDeliveryEstimate(),
                    FulfillmentMethod = product.GetFulfillmentMethod()
                });
            }

            summary.Subtotal = totalBeforeTax;
            summary.Shipping = totalShipping;
            summary.Tax = totalTax;
            summary.Total = totalBeforeTax + totalShipping + totalTax;

            return summary;
        }

        /// <summary>
        /// Get product details for display
        /// </summary>
        public string GetProductDetails(string productType, Dictionary<string, object> properties)
        {
            var product = ProductFactoryClass.CreateByType(productType, properties);
            return product.GetProductTypeDescription();
        }

        /// <summary>
        /// Check if product has special handling
        /// </summary>
        public bool RequiresSpecialHandling(string productType)
        {
            return productType.ToLower() switch
            {
                "digital" => false,     // No special handling
                "service" => true,      // Requires scheduling
                "subscription" => true, // Requires billing setup
                "bundle" => true,       // Requires validation
                "physical" => false,    // Standard handling
                _ => false
            };
        }
    }

    /// <summary>Item in shopping cart</summary>
    public class CartItem
    {
        public string ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>Summary of item in cart</summary>
    public class CartItemSummary
    {
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string DeliveryEstimate { get; set; }
        public string FulfillmentMethod { get; set; }
    }

    /// <summary>Summary of entire cart</summary>
    public class CartSummary
    {
        public List<CartItemSummary> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public override string ToString()
        {
            var itemCount = Items.Sum(i => i.Quantity);
            return $"Cart: {itemCount} items, " +
                   $"Subtotal: ${Subtotal:F2}, " +
                   $"Shipping: ${Shipping:F2}, " +
                   $"Tax: ${Tax:F2}, " +
                   $"Total: ${Total:F2}";
        }
    }
}
