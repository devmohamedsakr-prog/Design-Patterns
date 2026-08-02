using System;
using System.Collections.Generic;

namespace ProductFactory.Products
{
    /// <summary>
    /// Bundle product: Multiple items sold together
    /// Single Responsibility: Manage bundle product specifics
    /// </summary>
    public class BundleProduct : IProduct
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<string> BundleItems { get; set; } = new();
        public decimal BundleDiscount { get; set; } // Percentage discount

        public decimal CalculateShippingCost(decimal weight = 0)
        {
            // Bundle shipping - typically weight-based
            return weight > 0 ? (decimal)(Math.Ceiling(weight) * 5m) : 10m;
        }

        public decimal CalculateTax(string location)
        {
            // Apply tax to bundle price
            return location?.ToLower() switch
            {
                "ca" => Price * 0.0725m,
                "ny" => Price * 0.08m,
                _ => Price * 0.07m
            };
        }

        public string GetFulfillmentMethod() => "Bundle Fulfillment";
        public bool IsInStock() => true;
        public string GetDeliveryEstimate() => "3-5 business days";
        public string GetProductTypeDescription() => 
            $"Bundle - {BundleItems.Count} items, {BundleDiscount}% discount";
    }
}
